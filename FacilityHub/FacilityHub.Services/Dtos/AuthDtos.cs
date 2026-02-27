using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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