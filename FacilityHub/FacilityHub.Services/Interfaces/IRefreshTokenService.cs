using FacilityHub.Core.Entities;

namespace FacilityHub.Services.Interfaces;

public interface IRefreshTokenService
{
    Task<ServiceResult> GenerateTokenAsync(string ipAddress, string userAgent, CancellationToken cancellationToken);
     Task<ServiceResult> RevokeAllAsync(string UserId, CancellationToken cancellationToken);

    Task<ServiceResult<RefreshToken?>> RotateAsync(string token, string userId, string ipAddress, string userAgent,
        CancellationToken cancellationToken);
}