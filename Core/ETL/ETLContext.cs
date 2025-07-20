using Core.Settings;
using Microsoft.EntityFrameworkCore;

namespace Core.ETL;

public class CoreContext : DbContext
{
    public DbSet<ETLJob> Jobs => Set<ETLJob>();
    public DbSet<GitHubIssue> GitHubIssues => Set<GitHubIssue>();

    public CoreContext() : base(CreateOptions()) { }

    private static DbContextOptions<CoreContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<CoreContext>()
            .UseNpgsql(ApplicationSettings.Instance.PostgresConnection)
            .Options;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ETLJob>().HasIndex(x => x.Name).IsUnique();
    }
}

// Keep ETLContext as an alias for backward compatibility
public class ETLContext : CoreContext
{
}
