using FacilityHub.Services.Interfaces;
using FacilityHub.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FacilityHub.Services;

public static class ServiceExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {


        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        
        return services;    
    }
}