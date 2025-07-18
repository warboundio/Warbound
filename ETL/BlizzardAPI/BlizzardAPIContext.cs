using Core.Settings;
using ETL.BlizzardAPI.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace ETL.BlizzardAPI;

public class BlizzardAPIContext : DbContext
{
    public DbSet<ItemAppearance> ItemAppearances => Set<ItemAppearance>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Toy> Toys => Set<Toy>();
    public DbSet<Mount> Mounts => Set<Mount>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<Profession> Professions => Set<Profession>();
    public DbSet<ProfessionMedia> ProfessionMedias => Set<ProfessionMedia>();
    public DbSet<Realm> Realms => Set<Realm>();
    public DbSet<ItemMedia> ItemMedias => Set<ItemMedia>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeMedia> RecipeMedias => Set<RecipeMedia>();

    public BlizzardAPIContext() : base(CreateOptions()) { }

    private static DbContextOptions<BlizzardAPIContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<BlizzardAPIContext>()
            .UseNpgsql(ApplicationSettings.Instance.PostgresConnection)
            .Options;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}
