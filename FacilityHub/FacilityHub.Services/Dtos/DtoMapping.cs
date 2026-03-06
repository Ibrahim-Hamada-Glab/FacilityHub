using FacilityHub.Core.Entities;

namespace FacilityHub.Services.Dtos;

public  static class DtoMapping
{

    public static LoginActivity ToLoginActivity(this LoginContext loginDto, string userId, bool isSuccess) => new()
    {
        IpAddress = loginDto.Ipaddress,
        UserAgent = loginDto.UserAgent,
        UserId = userId,
        IsSuccess = isSuccess

    };
    //TODO: Add Needed Parameters ToAuthReponse
    public static AuthResponse ToAuthResponse(this AppUser user , string token ,RefreshToken refreshToken ,DateTime expiresAt, DateTime refreshTokenExpiresAt) 
    => new(token , refreshToken , expiresAt, refreshTokenExpiresAt, new UserInfo(
        user.Id,user.Email , user.FullName , user.AvatarUrl , new List<string> { user.Role.ToString() } , new List<string>()));

    public static AppUser ToAppUser(this RegisterDto registerDto) => new()
    {
        FirstName = registerDto.FirstName,
        LastName = registerDto.LastName,
        Email = registerDto.Email,
        AvatarUrl = registerDto.AvatarUrl,
        Role =  registerDto.Role,
        UserName = registerDto.Email,
        IsActive = true
        
    };
}