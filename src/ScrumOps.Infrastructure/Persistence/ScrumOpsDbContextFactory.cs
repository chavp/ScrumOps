using Microsoft.EntityFrameworkCore;
using ScrumOps.Infrastructure.Persistence;

namespace Mti.ProductManagement.Persistance
{
    public class ScrumOpsDbContextFactory : IDbContextFactory<ScrumOpsDbContext>
    {
        private DbContextOptions<ScrumOpsDbContext> _options;

        public ScrumOpsDbContextFactory(string connectionString)
        {
            _options = new DbContextOptionsBuilder<ScrumOpsDbContext>()
                .UseNpgsql(connectionString)
                .Options;
        }

        public ScrumOpsDbContext CreateDbContext()
        {
            return new ScrumOpsDbContext(_options);
        }
    }
}
