using System.Net;
using FacilityHub.Core.Contracts;
using FacilityHub.Core.Entities;
using FacilityHub.Services.helper;
using FacilityHub.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FacilityHub.Services.Services;

public class RefreshTokenService(
    IUnitOfWork unitOfWork,
    ILogger<RefreshTokenService> logger,
    UserManager<AppUser> userManager,
    ITokenService tokenService)
    : IRefreshTokenService
{
    #region Public Methods
    public async Task<ServiceResult> GenerateTokenAsync(string ipAddress, string userAgent,
        CancellationToken cancellationToken)
    {
        try
        {
            return await unitOfWork.ExecuteInTransaction<ServiceResult>(async () =>
            {
                var newRefreshToken = await tokenService.GenerateRefreshTokenAsync(userAgent, ipAddress);
                unitOfWork.RefreshTokenRepository.Add(newRefreshToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return ServiceResult.Success("Refresh token saved successfully");
            }, cancellationToken);
        }
       
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured while generating refresh token", cancellationToken);
            return ServiceResult.Failed($"An error occured while generating refresh token", "GENERATE_REFRESH_TOKEN_ERROR", new[] { $"An error occured while generating refresh token" }, HttpStatusCode.InternalServerError);
        }
    }

 
    public async Task<ServiceResult> RevokeAllAsync(string UserId, CancellationToken cancellationToken)
    {
        try
        {
            return await unitOfWork.ExecuteInTransaction<ServiceResult>(async () =>
            {
                var refreshTokens = await GetRefreshTokensAsync(UserId);
                if (refreshTokens.Count == 0)
                {
                    logger.LogWarning("No active refresh tokens found for user {userId}", UserId);
                    throw new ServiceException($"No active refresh tokens found for user {UserId}",
                        "NO_ACTIVE_REFRESH_TOKENS", new[] { $"No active refresh tokens found for user {UserId}" },
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
            return await unitOfWork.ExecuteInTransaction<ServiceResult<RefreshToken?>>(async () =>
            {
                var activeRefreshTokens = await GetRefreshTokensAsync(userId);
                if (activeRefreshTokens.Count == 0)
                    throw new ServiceException("No active refresh tokens found for user {userId}",
                        "NO_ACTIVE_REFRESH_TOKENS", new[] { $"No active refresh tokens found for user {userId}" },
                        HttpStatusCode.NotFound);
                var refreshToken = activeRefreshTokens.FirstOrDefault(x => x.Token == token);
                if (refreshToken == null)
                    throw new ServiceException($"Refresh token {token} not found",
                        "REFRESH_TOKEN_NOT_FOUND", new[] { $"Refresh token {token} not found" },
                        HttpStatusCode.NotFound);
                refreshToken.IsUsed = true;
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

    #region Private Methods

    private async Task<List<RefreshToken>> GetRefreshTokensAsync(string userId)
    {
        var user = await userManager.Users.Include(x => x.RefreshTokens).FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null) throw new ServiceException($"User {userId} not found",
            "USER_NOT_FOUND", new[] { $"User {userId} not found" }, HttpStatusCode.NotFound);
        return user.RefreshTokens.Where(x => x.IsActive).ToList();
    }

    #endregion
}