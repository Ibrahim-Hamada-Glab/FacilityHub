using FacilityHub.Services.Dtos;

namespace FacilityHub.Services.Interfaces;

public interface IAuthService
{
 
    public Task<ServiceResult<AuthResponse>> Login(LoginContext loginContext, CancellationToken cancellationToken);
}