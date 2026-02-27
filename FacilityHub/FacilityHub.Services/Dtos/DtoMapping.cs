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
    public static AuthResponse ToAuthResponse(this AppUser user , string token)
    => new AuthResponse(token , token , DateTime.UtcNow , new UserInfo(
        user.Id,user.Email , user.FullName , user.AvatarUrl , new List<string>() , new List<string>()));

}