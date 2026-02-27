using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FacilityHub.Core.Enums;

namespace FacilityHub.Services.Dtos;

 
    public record LoginDto([DataType(DataType.EmailAddress)]string Email, [MaxLength(100)]string Password);

    public record AuthResponse(
        // Tokens
        string AccessToken,
       
        [property: JsonIgnore] string RefreshToken,

        DateTime ExpiresAt,

        // User
        UserInfo User
    );

    public record UserInfo(
        string Id,
        string Email,
        string FullName,
        string? AvatarUrl,
        IList<string> Roles,
        IList<string> Permissions  // e.g., ["facilities.read", "workorders.write"]
    );

    public class RegisterDto
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? AvatarUrl { get; set; }

        public UserRole Role { get; set; }
    }

     
    