using Clinic_Management_Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Management_Api.DataAccess
{
    public class ClinicDbContext(DbContextOptions<ClinicDbContext> options) : DbContext(options)
    {
        public DbSet<Consultation> Consultations { get; set; }
    }

    public static class ClinicDbContextExtensions
    {
        public static void EnsureDbIsCreated(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ClinicDbContext>();
            context!.Database.EnsureCreated();
        }
    }
}
