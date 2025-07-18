using Core.Settings;
using Microsoft.EntityFrameworkCore;

namespace Core.ETL;

public class ETLContext : DbContext
{
    public DbSet<ETLJob> Jobs => Set<ETLJob>();

    public ETLContext() : base(CreateOptions()) { }

    private static DbContextOptions<ETLContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<ETLContext>()
            .UseNpgsql(ApplicationSettings.Instance.PostgresConnection)
            .Options;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<ETLJob>().HasIndex(x => x.Name).IsUnique();
}
