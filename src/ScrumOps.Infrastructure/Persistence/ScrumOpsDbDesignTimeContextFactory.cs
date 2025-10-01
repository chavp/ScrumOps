using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ScrumOps.Infrastructure.Persistence;

namespace ScrumOps.Infrastructure.Persistence
{
    public class ScrumOpsDbDesignTimeContextFactory : IDesignTimeDbContextFactory<ScrumOpsDbContext>
    {
        public ScrumOpsDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ScrumOpsDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=scrumops;Username=scrumops;Password=scrumops123");

            // Enable sensitive data logging in development
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();

            return new ScrumOpsDbContext(optionsBuilder.Options);
        }
    }
}
