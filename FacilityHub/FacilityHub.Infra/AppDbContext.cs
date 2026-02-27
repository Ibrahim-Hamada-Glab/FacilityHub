using FacilityHub.Core.Entities;
using FacilityHub.Infra.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FacilityHub.Infra;

public class AppDbContext(DbContextOptions options) : IdentityDbContext<AppUser>(options)
{
    
    //Sets
    public DbSet<LoginActivity> LoginActivities { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        

        base.OnModelCreating(builder);
              
        builder.Entity<AppUser>().ToTable("Users");

        builder.ApplyConfiguration(new AppUserConfig());
    }

    public override int SaveChanges()
    {
         var entries =  ChangeTracker.Entries<BaseEntity>().Where(e=>e.State is EntityState.Added or EntityState.Modified);

        foreach (var entry in entries)
        {
            var entity = entry.Entity;

            switch (entry.State)
            {
                case EntityState.Added:
                    entity.CreateAt =  DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entity.UpdateAt =  DateTime.UtcNow;
                    break;
                
            }
        }
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var entries =  ChangeTracker.Entries<BaseEntity>().Where(e=>e.State is EntityState.Added or EntityState.Modified);

        foreach (var entry in entries)
        {
            var entity = entry.Entity;

            switch (entry.State)
            {
                case EntityState.Added:
                    entity.CreateAt =  DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entity.UpdateAt =  DateTime.UtcNow;
                    break;
                
            }
        }
        return  await base.SaveChangesAsync(cancellationToken);
    }

    private void OnSavingChanges(object sender, EventArgs e)
    {
      
        
    }
}