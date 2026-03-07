using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityHub.Core.Entities;
using FacilityHub.Core.Enums;

namespace FacilityHub.Services.Dtos;

public record LoginDto([DataType(DataType.EmailAddress)] string Email, [MaxLength(100)] string Password);

public record AuthResponse(
    // Tokens
    string AccessToken,
    [property: JsonIgnore] RefreshToken? RefreshToken,
    DateTime ExpiresAt,
    DateTime RefreshTokenExpiresAt,

    // User
    UserInfo User
);

public record UserInfo(
    string Id,
    string Email,
    string FullName,
    string? AvatarUrl,
    IList<string> Roles,
    IList<string> Permissions // e.g., ["facilities.read", "workorders.write"]
);

public record RefreshTokenRequest(string Token);

public class RegisterDto
{
    [Required] [MaxLength(50)] public string FirstName { get; set; } = string.Empty;

    [Required] [MaxLength(50)] public string LastName { get; set; } = string.Empty;

    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;


    [Required]
    [DataType(DataType.Password)]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }

    public UserRole Role { get; set; }
}

public class ForgotPasswordDto{
    [EmailAddress][Required] public string Email{ get; set; } = string.Empty;
}


public class ResetPasswordDto{
    [EmailAddress][Required] public string Email{ get; set; } = string.Empty;
    [Required] public string Token{ get; set; } = string.Empty;
    [Required][DataType(DataType.Password)] public string NewPassword{ get; set; } = string.Empty;
    [Required][DataType(DataType.Password)][Compare("NewPassword")] public string ConfirmNewPassword{ get; set; } = string.Empty;
}



public class VerifyEmailDto{
    [Required] public string UserId{ get; set; } = string.Empty;
    [Required] public string Token{ get; set; } = string.Empty;
}

public class ChangePasswordDto{
    [Required][DataType(DataType.Password)] public string CurrentPassword{ get; set; } = string.Empty;
    [Required][DataType(DataType.Password)] public string NewPassword{ get; set; } = string.Empty;
    [Required][DataType(DataType.Password)][Compare("NewPassword")] public string ConfirmNewPassword{ get; set; } = string.Empty;
}