using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FacilityHub.Core.Enums;

namespace FacilityHub.Services.Dtos
{
    public record FacilityViewDto(
        Guid Id,
        string Name,
        string Code,
        string Address,
        string City,
        string Type,
        string Status,
        int TotalFloors = 5,
        decimal TotalArea = 1000,
        string ManagerName = "Ahmed Mohamed",
        int EquipmentCount = 10,
        int WorkOrderCount = 5
    );

    //class 
    public class CreateFacilityDto
    {
        [Required]
        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;    
        
        [Required]
        [MaxLength(256)]
        public string City { get; set; } = string.Empty;    
        [Required]
        [MaxLength(256)]
        public string Address { get; set; } = string.Empty;

        [Required]

        public FacilityType Type { get; set; }

        [Required]
        public FacilityStatus Status { get; set; } 
        [Required]
        [Range(0, int.MaxValue)]
        public int TotalFloors { get; set; } = 5;
        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalArea { get; set; } = 1000;
        
    }
}