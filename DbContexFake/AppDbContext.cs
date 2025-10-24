using DbContexFake.Entity;
using Microsoft.EntityFrameworkCore;

namespace DbContexFake
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Token).IsRequired().HasMaxLength(200);
            });

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "admin",
                    Token = "admin-token-123",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                },
                new User
                {
                    Id = 2,
                    Name = "developer",
                    Token = "dev-token-2024",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
            new User
            {
                Id = 3,
                Name = "tester",
                Token = "test-token-2024",
                CreatedAt = DateTime.UtcNow,
                IsActive = false
            }

                );


        }
    }
}
