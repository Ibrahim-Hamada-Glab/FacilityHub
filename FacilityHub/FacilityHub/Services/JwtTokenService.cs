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

    public async Task<string> GenerateTokenAsync(AppUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var userClaims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.NameId , user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email , user.Email),
            new Claim(JwtRegisteredClaimNames.Name , user.FullName),
            new Claim(JwtRegisteredClaimNames.GivenName , user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName , user.LastName),
            new Claim(ClaimTypes.Role , roles[0]),
          
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
        return new JwtSecurityTokenHandler().WriteToken(token);

     }
}