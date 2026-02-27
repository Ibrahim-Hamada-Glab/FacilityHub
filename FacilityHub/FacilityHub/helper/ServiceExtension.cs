using System.Security.Claims;
using System.Text;
using FacilityHub.Core.Contracts;
using FacilityHub.Core.Entities;
using FacilityHub.Infra;
using FacilityHub.Services;
using Microsoft.AspNetCore.Identity;

using JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FacilityHub.helper;

public static class ServiceExtension
{
    public static IServiceCollection AddApiServices(this IServiceCollection services , IConfiguration  config)
    {
        services.Configure<JwtToken>(config.GetSection("Jwt"));

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(config.GetConnectionString("DefaultConnection") , sqlOptions =>
            {
                sqlOptions.MigrationsAssembly("FacilityHub.Infra");
                sqlOptions.EnableRetryOnFailure(3 , TimeSpan.FromSeconds(5) ,null);
            })  ;
        });
          services.AddIdentityCore<AppUser>(o =>
            {
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = true;
                o.Password.RequireUppercase = true;
                o.Password.RequireNonAlphanumeric = true;
                o.Password.RequiredLength = 8;
                o.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        }).AddJwtBearer(options =>
        {
            var jwtOptions =  config.GetSection("Jwt").Get<JwtToken>()!;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.FromSeconds(10),
                NameClaimType = ClaimTypes.Name,

            };
            options.Events = new JwtBearerEvents
            {
            
                OnMessageReceived = context =>
                {
                    var token = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(token) && path.StartsWithSegments("/hubs"))
                        context.Token = token;

                    return Task.CompletedTask;
                },
            };
            options.RequireHttpsMetadata = true;
            options.SaveToken = true;
        });
        services.AddControllers();
        services.AddOpenApi();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddAuthorization();
        services.AddSignalR();
        return services;
    }
}