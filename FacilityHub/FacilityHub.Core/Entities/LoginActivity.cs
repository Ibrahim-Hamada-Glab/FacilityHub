using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacilityHub.Core.Entities;

public class LoginActivity : BaseEntity
{
    public string UserId  { get; set; }
    [Required]
    [MaxLength(100)]
    public string? IpAddress { get; set; }
    [Required]
    [MaxLength(100)]
    public string? UserAgent { get; set; }
    
    [Required]
    public bool IsSuccess { get; set; }
}