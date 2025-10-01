using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ScrumOps.Infrastructure.Persistence;

namespace Mti.ProductManagement.Persistance
{
    public class ScrumOpsDbDesignTimeContextFactory : IDesignTimeDbContextFactory<ScrumOpsDbContext>
    {
        public ScrumOpsDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ScrumOpsDbContext>();
            optionsBuilder.UseNpgsql("Server=localhost;TrustServerCertificate=True;User Id=scrumops;Password=scrumops123;Database=scrumopsdb;");

            // Enable sensitive data logging in development
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();

            return new ScrumOpsDbContext(optionsBuilder.Options);
        }
    }
}
