using FacilityHub.Services.Dtos;

namespace FacilityHub.Services.Interfaces;

public interface IAuthService
{

    public Task<ServiceResult<AuthResponse>> Login(LoginContext loginContext, CancellationToken cancellationToken);
    public Task<ServiceResult<AuthResponse>> Register(RegisterDto registerDto, CancellationToken cancellationToken);
    public Task<ServiceResult<UserInfo>> Me(string userId, CancellationToken cancellationToken);
    public Task<ServiceResult<AuthResponse>> RefreshToken(string accessToken, string refreshToken, string ipAddress, string userAgent, CancellationToken cancellationToken);
}