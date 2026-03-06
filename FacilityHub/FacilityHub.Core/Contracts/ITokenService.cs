using FacilityHub.Core.Entities;

namespace FacilityHub.Core.Contracts;

public interface ITokenService
{
 
    public Task<string> GenerateTokenAsync(AppUser user);
    public Task<RefreshToken> GenerateRefreshTokenAsync(string userAgent, string ipAddress);
}