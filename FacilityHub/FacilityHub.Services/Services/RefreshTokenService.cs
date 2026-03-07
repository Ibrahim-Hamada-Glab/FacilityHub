using System.Net;
using FacilityHub.Core.Contracts;
using FacilityHub.Core.Entities;
using FacilityHub.Services.helper;
using FacilityHub.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FacilityHub.Services.Services;

public class RefreshTokenService(
    IUnitOfWork unitOfWork,
    ILogger<RefreshTokenService> logger,
    UserManager<AppUser> userManager,
    ITokenService tokenService)
    : IRefreshTokenService
{
    #region Private Methods

    private async Task<List<RefreshToken>> GetRefreshTokensAsync(AppUser user)
    {
        logger.LogInformation("Getting refresh tokens for user {userId}", user.Id);
        var refreshTokens = user.RefreshTokens.Where(x => x.IsActive).ToList();
        logger.LogInformation("Found {count} active refresh tokens for user {userId}", refreshTokens.Count, user.Id);
        // print the refresh tokens
        logger.LogInformation("Refresh tokens: {refreshTokens}", JsonConvert.SerializeObject(refreshTokens));
        // print the refresh tokens in a readable format
        logger.LogInformation("Refresh tokens: {refreshTokens}", string.Join(", ", refreshTokens.Select(x => x.Token)));
        return refreshTokens;
    }

    #endregion

    #region Public Methods

    public async Task<ServiceResult<RefreshToken?>> GenerateTokenAsync(string ipAddress, string userAgent,
        string userId,
        CancellationToken cancellationToken)
    {
        try
        {
            return await unitOfWork.ExecuteInTransaction(async () =>
            {
                var newRefreshToken = await tokenService.GenerateRefreshTokenAsync(userAgent, ipAddress);
                unitOfWork.RefreshTokenRepository.Add(newRefreshToken);
                var user = await userManager.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
                if (user == null)
                    throw new ServiceException($"User {userId} not found",
                        "USER_NOT_FOUND", new[] { $"User {userId} not found" }, HttpStatusCode.NotFound);
                user.RefreshTokens.Add(newRefreshToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return ServiceResult<RefreshToken?>.Success(newRefreshToken, "Refresh token saved successfully");
            }, cancellationToken);
        }

        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured while generating refresh token", cancellationToken);
            return ServiceResult<RefreshToken?>.Failed("An error occured while generating refresh token",
                "GENERATE_REFRESH_TOKEN_ERROR", new[]
                {
                    "An error occured while generating refresh token"
                }, HttpStatusCode.InternalServerError);
        }
    }


    public async Task<ServiceResult> RevokeAllAsync(string UserId, CancellationToken cancellationToken)
    {
        try
        {
            return await unitOfWork.ExecuteInTransaction(async () =>
            {
                var user = await userManager.Users.Where(x => x.Id == UserId).Include(x => x.RefreshTokens)
                    .FirstOrDefaultAsync();
                if (user == null)
                    throw new ServiceException($"User {UserId} not found",
                        "USER_NOT_FOUND", new[] { $"User {UserId} not found" }, HttpStatusCode.NotFound);

                var refreshTokens = await GetRefreshTokensAsync(user);
                if (refreshTokens.Count == 0)
                {
                    logger.LogWarning("No active refresh tokens found for user {userId}", user.Id);
                    throw new ServiceException($"No active refresh tokens found for user {user.Id}",
                        "NO_ACTIVE_REFRESH_TOKENS", new[] { $"No active refresh tokens found for user {user.Id}" },
                        HttpStatusCode.NotFound);
                }

                foreach (var refreshToken in refreshTokens)
                    refreshToken.IsRevoked = true;

                await unitOfWork.SaveChangesAsync(cancellationToken);
                return ServiceResult.Success($"{refreshTokens.Count} refresh tokens revoked successfully");
            }, cancellationToken);
        }
        catch (ServiceException ex)
        {
            logger.LogError(ex, "An error occured while revoking all refresh tokens for user {userId}", UserId);
            return ServiceResult.Failed(ex.Message, ex.ErrorCode, ex.Errors, ex.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured while revoking all refresh tokens for user {userId}", UserId);
            return ServiceResult.Failed($"An error occured while revoking all refresh tokens for user {UserId}",
                "REVOKING_ALL_REFRESH_TOKENS_ERROR",
                new[] { $"An error occured while revoking all refresh tokens for user {UserId}" },
                HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult<RefreshToken?>> RotateAsync(string token, string userId, string ipAddress,
        string userAgent, CancellationToken cancellationToken)
    {
        try
        {
            return await unitOfWork.ExecuteInTransaction(async () =>
            {
                var user = await userManager.Users.Where(x => x.Id == userId).Include(x => x.RefreshTokens)
                    .FirstOrDefaultAsync();
                if (user == null)
                    throw new ServiceException($"User {userId} not found",
                        "USER_NOT_FOUND", new[] { $"User {userId} not found" }, HttpStatusCode.NotFound);
                var activeRefreshTokens = await GetRefreshTokensAsync(user);
                if (activeRefreshTokens.Count == 0)
                    throw new ServiceException($"No active refresh tokens found for user {userId}",
                        "NO_ACTIVE_REFRESH_TOKENS", new[] { $"No active refresh tokens found for user {userId}" },
                        HttpStatusCode.NotFound);

                var refreshToken = activeRefreshTokens.FirstOrDefault(x => x.Token == token);
                logger.LogInformation("Refresh token: {refreshToken}", JsonConvert.SerializeObject(refreshToken));
                if (refreshToken == null)
                    throw new ServiceException($"Refresh token {token} not found",
                        "REFRESH_TOKEN_NOT_FOUND", new[] { $"Refresh token {token} not found" },
                        HttpStatusCode.NotFound);
                if (refreshToken.IsRevoked)
                    throw new ServiceException($"Refresh token {token} is revoked",
                        "REFRESH_TOKEN_REVOKED", new[] { $"Refresh token {token} is revoked" },
                        HttpStatusCode.Unauthorized);
                if (refreshToken.IsUsed)
                    throw new ServiceException($"Refresh token {token} is used",
                        "REFRESH_TOKEN_USED", new[] { $"Refresh token {token} is used" },
                        HttpStatusCode.Unauthorized);
                refreshToken.IsUsed = true;
                user.RefreshTokens.Add(refreshToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var newRefreshToken = await tokenService.GenerateRefreshTokenAsync(userAgent, ipAddress);
                unitOfWork.RefreshTokenRepository.Add(newRefreshToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return ServiceResult<RefreshToken?>.Success(newRefreshToken,
                    $"New refresh token generated for user {userId}");
            }, cancellationToken);
        }
        catch (ServiceException ex)
        {
            logger.LogError(ex, "An error occured while rotating refresh token for user {userId}", userId);
            return ServiceResult<RefreshToken?>.Failed(ex.Message, ex.ErrorCode, ex.Errors, ex.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured while rotating refresh token for user {userId}", userId);
            return ServiceResult<RefreshToken?>.Failed(
                $"An error occured while rotating refresh token for user {userId}", "ROTATING_REFRESH_TOKEN_ERROR",
                new[] { $"An error occured while rotating refresh token for user {userId}" },
                HttpStatusCode.InternalServerError);
        }
    }

    #endregion
}