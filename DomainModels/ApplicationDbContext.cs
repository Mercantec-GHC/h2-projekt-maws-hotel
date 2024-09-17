using Microsoft.EntityFrameworkCore;

namespace DomainModels
{
    public class ApplicationDbContext : DbContext  
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Profile> Profiles { get; set; }

        public DbSet<Room> Rooms { get; set; }

        // ... other DbSet properties ...

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Profile>()
                .ToTable("profile")  
                .HasIndex(p => p.Email)
                .IsUnique();
        }
    }
}
