using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FacilityHub.Core.Entities
{
    public class RefreshToken:BaseEntity
    {
        [Required]
        [MaxLength(256)]
        public string Token { get; set; } = string.Empty;


        [Required]
        [MaxLength(256)]
        public string UserAgent { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string IpAddress { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string FamilyId { get; set; } = string.Empty;

      

        [Required]
        public DateTime ExpiresAt { get; set; }


        public bool IsRevoked { get; set; }
     
        public bool IsUsed { get; set; }


        public bool IsActive => !IsRevoked && !IsUsed && DateTime.UtcNow < ExpiresAt;


        
        
    }
}