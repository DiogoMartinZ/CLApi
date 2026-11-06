using CLApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CLApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Id)
                      .ValueGeneratedNever();

                entity.HasIndex(p => p.Id)
                      .IsUnique();

                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Stock).IsRequired();
                entity.Property(e => e.DateCreated).IsRequired();
                entity.Property(e => e.IsDeleted).IsRequired();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Id)
                      .ValueGeneratedNever();

                entity.HasIndex(p => p.Id)
                      .IsUnique();

                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(256);
                
            });

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 100001, 
                    Name = "Optics Cleaning Kit", 
                    Description = "Professional lens cleaning solution", 
                    Price = 29.99, 
                    Stock = 150,
                    DateCreated = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false },

                new Product { Id = 100002, 
                    Name = "Precision Screwdriver Set", 
                    Description = "For delicate mechanical repairs", 
                    Price = 45.00, 
                    Stock = 80,
                    DateCreated = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false
                }
            );

            modelBuilder.Entity<User>().HasData(
        

               new User
               {
                   Id = 1,
                   Username = "admin",
                   PasswordHash = "admin123" // In a real application
               }
           );
        }
    }
}