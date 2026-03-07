using FacilityHub.Core.Contracts;
using FacilityHub.Infra.Repository;
using FacilityHub.Infra.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FacilityHub.Infra;

public static class ServiceExtension
{
    public static IServiceCollection AddInfraServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>) ,  typeof(GenericRepository<>));
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        return services;
    }
}

 