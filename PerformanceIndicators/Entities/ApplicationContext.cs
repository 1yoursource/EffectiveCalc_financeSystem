using Microsoft.EntityFrameworkCore;

namespace PerformanceIndicators.Entities
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }

        public DbSet<CashFlow> CashFlows { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
