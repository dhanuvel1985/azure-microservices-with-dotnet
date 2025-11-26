using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using Pet_Management_Api.DataAccess.Entities;
using System.Security.Cryptography.Xml;

namespace Pet_Management_Api.DataAccess
{
    public class ManagementDbContext(DbContextOptions<ManagementDbContext> options) : DbContext(options)
    {
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Breed> Breeds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Breed>().HasData(
                [
                    new Breed(1,"Beagle"),
                    new Breed(2, "Staffordshire Terrier")
                ]
            );

            modelBuilder.Entity<Pet>().HasData(
                [
                    new Pet() { Id = 1, Name = "Gianni", Age = 20, BreedId = 1 },
                    new Pet() { Id = 2, Name = "Nina", Age = 3, BreedId = 2 },
                    new Pet() { Id = 3, Name = "Cati", Age = 17, BreedId = 1 },
                ]
            );
        }
    }

    public static class ManagementDbContextExtensions
    {
        public static void EnsureDbIsCreated(this IApplicationBuilder app) 
        { 
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ManagementDbContext>();
            context!.Database.EnsureCreated();
        }
    }
}
