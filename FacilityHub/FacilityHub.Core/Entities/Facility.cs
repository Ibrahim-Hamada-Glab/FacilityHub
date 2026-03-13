using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FacilityHub.Core.Enums;

namespace FacilityHub.Core.Entities;

public class Facility : BaseEntity
{
    [Required] [MaxLength(256)] public string Name { get; set; } = string.Empty;

    [Required] [MaxLength(256)] public long CodeNumber { get; set; } = 0;

    public string Code => $"FAC-{CodeNumber:D6}";

    [Required] [MaxLength(256)] public string Address { get; set; } = string.Empty;

    [Required] [MaxLength(256)] public string City { get; set; } = string.Empty;

    [Required] public FacilityType Type { get; set; } = FacilityType.Office;

    [Required] public int TotalFloors { get; set; } = 0;

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalArea { get; set; } = 0;

    [Required] public FacilityStatus Status { get; set; } = FacilityStatus.Active;


    public string? ManagerId { get; set; }

    [ForeignKey("ManagerId")] public virtual AppUser? Manager { get; set; }

    public string? ImageUrl { get; set; }

    public string CreatedById { get; set; }

    [ForeignKey("CreatedById")] public virtual AppUser CreatedBy { get; set; }
}