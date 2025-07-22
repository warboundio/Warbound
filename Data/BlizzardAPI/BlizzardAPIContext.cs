using Core.Settings;
using Data.BlizzardAPI.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace Data.BlizzardAPI;

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
    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<AchievementMedia> AchievementMedias => Set<AchievementMedia>();
    public DbSet<JournalExpansion> JournalExpansions => Set<JournalExpansion>();
    public DbSet<JournalEncounter> JournalEncounters => Set<JournalEncounter>();

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
