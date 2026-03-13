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
        Role = registerDto.Role,
        UserName = registerDto.Email,
        IsActive = true

    };

    public static FacilityViewDto ToFacilityViewDto(this Facility facility) => new(
        facility.Id,
        facility.Name,
        facility.Code,
        facility.Address,
        facility.City,
        facility.Type.ToString(),
        facility.Status.ToString(),
        facility.TotalFloors,
        facility.TotalArea

    );
    
    public static Facility ToFacility(this CreateFacilityDto dto , string createdById) => new()
    {
        Name = dto.Name,
        City = dto.City,
        Address = dto.Address,
        Type = dto.Type,
        Status = dto.Status,
        TotalFloors = dto.TotalFloors,
        TotalArea = dto.TotalArea,
        CreatedById = createdById

    };
}