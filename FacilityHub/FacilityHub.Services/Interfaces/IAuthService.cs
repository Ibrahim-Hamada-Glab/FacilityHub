using FacilityHub.Services.Dtos;

namespace FacilityHub.Services.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<AuthResponse>> Login(LoginContext loginContext, CancellationToken cancellationToken);
    Task<ServiceResult<AuthResponse>> Register(RegisterDto registerDto, CancellationToken cancellationToken);
    Task<ServiceResult<UserInfo>> Me(string userId, CancellationToken cancellationToken);
    Task<ServiceResult<AuthResponse>> RefreshToken(string accessToken, string refreshToken, string ipAddress, string userAgent, CancellationToken cancellationToken);
    Task<ServiceResult> ForgotPassword(ForgotPasswordDto dto, CancellationToken cancellationToken);
    Task<ServiceResult> ResetPassword(ResetPasswordDto dto, CancellationToken cancellationToken);
    Task<ServiceResult> VerifyEmail(VerifyEmailDto dto, CancellationToken cancellationToken);
    Task<ServiceResult> ChangePassword(string userId, ChangePasswordDto dto, CancellationToken cancellationToken);
}