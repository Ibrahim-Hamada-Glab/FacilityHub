using System.ComponentModel.DataAnnotations;
using FacilityHub.Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace FacilityHub.Core.Entities;

public class AppUser : IdentityUser
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;
    [MaxLength(50)]
    [Required]
    public string LastName { get; set; } =  string.Empty;
    public string FullName  =>  $"{FirstName} {LastName}";
    

    [Required] public UserRole Role { get; set; } = UserRole.Viewer;
    public bool IsActive { get; set; }
    [MaxLength(50)]
    public string? AvatarUrl { get; set; }
    
    public virtual ICollection<LoginActivity>  LoginActivities { get; set; } =  new List<LoginActivity>();
    public virtual ICollection<RefreshToken>  RefreshTokens { get; set; } =  new List<RefreshToken>();
}