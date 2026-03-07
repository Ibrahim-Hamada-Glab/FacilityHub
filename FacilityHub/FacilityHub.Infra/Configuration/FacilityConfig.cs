using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacilityHub.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FacilityHub.Infra.Configuration
{
    public class FacilityConfig : IEntityTypeConfiguration<Facility>
    {
        public void Configure(EntityTypeBuilder<Facility> builder)
        {
            builder.ToTable("Facilities");

            builder.HasOne(e => e.Manager)
                .WithMany()
                .HasForeignKey(e => e.ManagerId);

            builder.HasOne(e => e.CreatedBy)
                .WithMany()
                .HasForeignKey(e => e.CreatedById);
        }
        
    }
}