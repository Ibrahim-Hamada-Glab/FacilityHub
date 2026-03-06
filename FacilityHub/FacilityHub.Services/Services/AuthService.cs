using System.IdentityModel.Tokens.Jwt;
using System.Net;
using FacilityHub.Core.Contracts;
using FacilityHub.Core.Entities;
using FacilityHub.Services.Dtos;
using FacilityHub.Services.helper;
using FacilityHub.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FacilityHub.Services.Services;

public class AuthService(
    IUnitOfWork unitOfWork,
    ILogger<AuthService> logger,
    SignInManager<AppUser> signInManager,
    UserManager<AppUser> userManager,
    ITokenService tokenService,
    IRefreshTokenService refreshTokenService) : IAuthService
{
    public async Task<ServiceResult<AuthResponse>> Login(LoginContext loginDto, CancellationToken cancellationToken)
    {
        try
        {
            return await unitOfWork.ExecuteInTransaction(async () =>
            {
                logger.LogInformation("Attempting to login");
                var user = await userManager.FindByEmailAsync(loginDto.LoginDto.Email);
                if (user == null)
                {
                    logger.LogInformation("User not found");
                    return ServiceResult<AuthResponse>.Failed(
                        "The email address or password you entered was incorrect",
                        "AUTHENTICATION_FAILED",
                        new[] { "The email address or password you entered was incorrect" });
                }

                var res = await signInManager.CheckPasswordSignInAsync(user, loginDto.LoginDto.Password,
                    true);

                if (!res.Succeeded)
                {
                    if (res.IsLockedOut)
                    {
                        var lockoutEnd = await userManager.GetLockoutEndDateAsync(user);

                        if (lockoutEnd.HasValue && lockoutEnd.Value > DateTimeOffset.UtcNow)
                        {
                            var remaining = lockoutEnd.Value - DateTimeOffset.UtcNow;
                            return ServiceResult<AuthResponse>.Failed(
                                $"Account is locked. Try again in {Math.Ceiling(remaining.TotalMinutes)} minutes.",
                                "ACCOUNT_LOCKED",
                                new[] { $"Account locked until {lockoutEnd.Value:HH:mm:ss UTC}" },
                                HttpStatusCode.TooManyRequests);
                        }
                    }

                    unitOfWork.LoginActivityRepository.Add(loginDto.ToLoginActivity(user.Id, false));
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    return ServiceResult<AuthResponse>.Failed("The email address or password you entered was incorrect",
                        "AUTHENTICATION_FAILED", new[] { "The email address or password you entered was incorrect" },
                        HttpStatusCode.Unauthorized);
                }

                var newRefreshToken = await refreshTokenService.GenerateTokenAsync(loginDto.Ipaddress ?? string.Empty,
                    loginDto.UserAgent ?? string.Empty, user.Id, cancellationToken);
                if (!newRefreshToken.IsSuccess || newRefreshToken.Data == null)
                    throw new ServiceException(newRefreshToken.Message, newRefreshToken.ErrorCode,
                        newRefreshToken.Errors, newRefreshToken.StatusCode);
                var (token, expiresAt) = await tokenService.GenerateTokenAsync(user);
                var authResponse = user.ToAuthResponse(token, newRefreshToken.Data, expiresAt,
                    newRefreshToken.Data.ExpiresAt);
                unitOfWork.LoginActivityRepository.Add(loginDto.ToLoginActivity(user.Id, true));
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<AuthResponse>.Success(authResponse);
            }, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Login failed with unexpected error"); // ← use logger
            return ServiceResult<AuthResponse>.Failed("InternalServerError", "SYSTEM_ERROR", new[] { "Server Error" },
                HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult<UserInfo>> Me(string userId, CancellationToken cancellationToken)
    {
        return await unitOfWork.ExecuteInTransaction(async () =>
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return ServiceResult<UserInfo>.Failed("User not found", "USER_NOT_FOUND", new[] { "User not found" },
                    HttpStatusCode.NotFound);
            return ServiceResult<UserInfo>.Success(new UserInfo(user.Id, user.Email, user.FullName, user.AvatarUrl,
                new List<string> { user.Role.ToString() }, new List<string>()));
        }, cancellationToken);
    }

    public async Task<ServiceResult<AuthResponse>> RefreshToken(string accessToken, string refreshToken,
        string ipAddress, string userAgent, CancellationToken cancellationToken)
    {
        try
        {
            return await unitOfWork.ExecuteInTransaction(async () =>
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(accessToken);
                var userId = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.NameId)?.Value;
                if (userId == null)
                    throw new ServiceException("Invalid token", "INVALID_TOKEN", new[] { "Invalid token" },
                        HttpStatusCode.Unauthorized);

                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                    throw new ServiceException("User not found", "USER_NOT_FOUND", new[] { "User not found" },
                        HttpStatusCode.NotFound);
                var refreshTokenEntity =
                    await refreshTokenService.RotateAsync(refreshToken, userId, ipAddress, userAgent,
                        cancellationToken);
                if (!refreshTokenEntity.IsSuccess || refreshTokenEntity.Data == null)
                    throw new ServiceException(refreshTokenEntity.Message, refreshTokenEntity.ErrorCode,
                        refreshTokenEntity.Errors, refreshTokenEntity.StatusCode);

                var (newToken, newTokenExpiresAt) = await tokenService.GenerateTokenAsync(user);
                var newAuthResponse = user.ToAuthResponse(newToken, refreshTokenEntity.Data, newTokenExpiresAt,
                    refreshTokenEntity.Data.ExpiresAt);
                return ServiceResult<AuthResponse>.Success(newAuthResponse);
            }, cancellationToken);
        }
        catch (ServiceException ex)
        {
            logger.LogError(ex, "Refresh token failed with unexpected error");
            return ServiceResult<AuthResponse>.Failed(ex.Message, ex.ErrorCode, ex.Errors, ex.StatusCode);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Refresh token failed with unexpected error");
            return ServiceResult<AuthResponse>.Failed("InternalServerError", "SYSTEM_ERROR", new[] { "Server Error" },
                HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult<AuthResponse>> Register(RegisterDto registerDto,
        CancellationToken cancellationToken)
    {
        try
        {
            return await unitOfWork.ExecuteInTransaction(async () =>
            {
                logger.LogInformation("Attempting to register user with email: {Email}", registerDto.Email);

                if (await userManager.FindByEmailAsync(registerDto.Email) is not null)
                {
                    logger.LogWarning("Registration failed: email already exists - {Email}", registerDto.Email);
                    return ServiceResult<AuthResponse>.Failed(
                        "An account with this email address already exists.",
                        "EMAIL_ALREADY_EXISTS",
                        new[] { "An account with this email address already exists." },
                        HttpStatusCode.Conflict);
                }

                var user = registerDto.ToAppUser();
                var res = await userManager.CreateAsync(user, registerDto.Password);

                if (!res.Succeeded)
                {
                    var errors = res.Errors.Select(e => e.Description).ToArray();
                    logger.LogWarning("Registration failed for {Email}: {Errors}", registerDto.Email,
                        string.Join(", ", errors));
                    return ServiceResult<AuthResponse>.Failed(
                        "Registration failed.",
                        "REGISTRATION_FAILED",
                        errors);
                }


                var (token, expiresAt) = await tokenService.GenerateTokenAsync(user);
                // TODO: Generate Refresh Token
                var authResponse =
                    user.ToAuthResponse(token, new RefreshToken(), expiresAt, DateTime.UtcNow.AddDays(7));

                logger.LogInformation("User registered successfully: {Email}", registerDto.Email);
                return ServiceResult<AuthResponse>.Success(authResponse);
            }, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Registration failed with unexpected error for {Email}", registerDto.Email);
            return ServiceResult<AuthResponse>.Failed(
                "InternalServerError",
                "SYSTEM_ERROR",
                new[] { "Server Error" },
                HttpStatusCode.InternalServerError);
        }
    }
}