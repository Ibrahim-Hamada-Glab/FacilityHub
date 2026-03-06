 using System.IdentityModel.Tokens.Jwt;
 using System.Security.Claims;
 using System.Text;
 using FacilityHub.Core.Contracts;
using FacilityHub.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using JWT;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;
using System.Security.Cryptography;


namespace FacilityHub.Services;

public class JwtTokenService : ITokenService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly  JwtToken _tokenOptions;

    public JwtTokenService(UserManager<AppUser> userManager, IOptions<JwtToken> tokenOptions)
    {
        _userManager = userManager;
        _tokenOptions = tokenOptions.Value;
    }


    public async Task<RefreshToken> GenerateRefreshTokenAsync(string userAgent, string ipAddress)
    {
        var randomNumber = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }
        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomNumber),
            ExpiresAt = DateTime.UtcNow.AddDays(_tokenOptions.RefreshTokenExpirationDays),
            IsRevoked = false,
            IsUsed = false,
            UserAgent = userAgent,
            IpAddress = ipAddress,
            FamilyId =  Guid.NewGuid().ToString(),
         };
    }

    public async Task<(string token, DateTime expiresAt)> GenerateTokenAsync(AppUser user)
    {
         var userClaims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.NameId , user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email , user.Email),
            new Claim(JwtRegisteredClaimNames.Name , user.FullName),
            new Claim(JwtRegisteredClaimNames.GivenName , user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName , user.LastName),
            new Claim(ClaimTypes.Role , user.Role.ToString()),
          
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.Key));
        var credentials =  new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
         
        var token = new JwtSecurityToken(
            issuer: _tokenOptions.Issuer,
            audience: _tokenOptions.Audience,
            claims: userClaims,
            expires: DateTime.UtcNow.AddMinutes(_tokenOptions.AccessTokenExpirationMinutes),
            signingCredentials: credentials
        );
        return (new JwtSecurityTokenHandler().WriteToken(token), DateTime.UtcNow.AddMinutes(_tokenOptions.AccessTokenExpirationMinutes) );

     }
}