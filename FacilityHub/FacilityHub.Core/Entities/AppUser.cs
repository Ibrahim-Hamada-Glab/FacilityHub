using System.ComponentModel.DataAnnotations;
using FacilityHub.Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace FacilityHub.Core.Entities;

public class AppUser : IdentityUser
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }
    [MaxLength(50)]
    [Required]
    public string LastName { get; set; }
    public string FullName  =>  $"{FirstName} {LastName}";
    
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required] public UserRole Role { get; set; } = UserRole.Viewer;
    public bool IsActive { get; set; }
    public string? AvatarUrl { get; set; }
    
    public virtual ICollection<LoginActivity>  LoginActivities { get; set; } =  new List<LoginActivity>();
}