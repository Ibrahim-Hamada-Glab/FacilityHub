using FacilityHub.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FacilityHub.Infra.Configuration;

public class AppUserConfig : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasMany(e => e.LoginActivities)
            .WithOne()
            .HasForeignKey(e => e.UserId);
        
    }
}