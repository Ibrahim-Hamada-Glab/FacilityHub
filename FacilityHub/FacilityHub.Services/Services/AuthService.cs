using System.Net;
using FacilityHub.Core.Contracts;
using FacilityHub.Core.Entities;
using FacilityHub.Services.Dtos;
using FacilityHub.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FacilityHub.Services.Services;

public class AuthService(IUnitOfWork unitOfWork , ILogger<AuthService> logger , UserManager<AppUser> userManager , ITokenService tokenService ) : IAuthService
{
    public async Task<ServiceResult<AuthResponse>> Login(LoginContext loginDto,CancellationToken cancellationToken )
    {
        try
        {
            return await unitOfWork.ExecuteInTransaction(async () =>
            {
                bool res;

                logger.LogInformation("Attempting to login");
                var user = await userManager.FindByEmailAsync(loginDto.LoginDto.Email);
                if (user == null)
                {
                    logger.LogInformation("User not found");
                    return ServiceResult<AuthResponse>.Failed("User not found" , "NOTFOUND" , new []{"User not found"} , HttpStatusCode.NotFound);
                    
                }

                res = await userManager.CheckPasswordAsync(user, loginDto.LoginDto.Password);
                if (!res)
                {
                    unitOfWork.LoginActivityRepository.Add(loginDto.ToLoginActivity(user.Id, false));
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                    
                    return ServiceResult<AuthResponse>.Failed("The email address or password you entered was incorrect" ,"AUTHENTICATION_FAILED" , new []{"The email address or password you entered was incorrect"} );
          
                }
                var token = await tokenService.GenerateTokenAsync(user);
                var authResponse = user.ToAuthResponse(token);
                return ServiceResult<AuthResponse>.Success(authResponse);


            },cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ServiceResult<AuthResponse>.Failed("InternalServerError" , "SYSTEM_ERROR" , new []{"Server Error"} ,HttpStatusCode.InternalServerError);
        }
        
        
    }
    
}