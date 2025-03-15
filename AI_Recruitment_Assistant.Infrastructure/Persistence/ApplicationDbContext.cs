
using AI_Recruitment_Assistant.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AI_Recruitment_Assistant.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User, IdentityRole<int>, int>(options)
{
    public DbSet<JobPosting> JobPostings { get; set; }
    public DbSet<JobApplication> JobApplications { get; set; }
    public DbSet<Interview> Interviews { get; set; }
    public DbSet<SystemConstant> SystemConstants { get; set; }
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .ToList();

        foreach (var entityEntry in entries)
        {
            var createdAtProperty = entityEntry.Entity.GetType().GetProperty("CreatedAt");
            var updatedAtProperty = entityEntry.Entity.GetType().GetProperty("UpdatedAt");

            var now = DateTime.UtcNow;

            if (entityEntry.State == EntityState.Modified && updatedAtProperty != null)
            {
                updatedAtProperty.SetValue(entityEntry.Entity, now);
            }
            else if (entityEntry.State == EntityState.Added)
            {
                createdAtProperty?.SetValue(entityEntry.Entity, now);
                updatedAtProperty?.SetValue(entityEntry.Entity, now);
            }
        }

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
        modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
        modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
        modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
        modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
        modelBuilder.Entity<User>().ToTable("Users");


        modelBuilder.Entity<User>()
            .HasOne(u => u.UserType)
            .WithMany(c => c.UserTypesNavigation)
            .HasForeignKey(c => c.UserTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobPosting>()
            .HasOne(jp => jp.CreatedByUser)
            .WithMany(u => u.CreatedJobPostings)
            .HasForeignKey(jp => jp.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

       
        modelBuilder.Entity<JobPosting>()
            .HasMany(jp => jp.Applications)
            .WithOne(ja => ja.JobPosting)
            .HasForeignKey(ja => ja.JobPostingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobApplication>()
            .HasMany(ja => ja.Interviews)
            .WithOne(i => i.JobApplication)
            .HasForeignKey(i => i.JobApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobApplication>()
           .HasOne(ja => ja.Status)
           .WithMany(i => i.StatusesNavigation)
           .HasForeignKey(i => i.StatusId)
           .OnDelete(DeleteBehavior.Cascade);

    }
}
