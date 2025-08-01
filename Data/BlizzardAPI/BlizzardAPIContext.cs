using Core.Settings;
using Data.Addon;
using Data.BlizzardAPI.Models;
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
    public DbSet<QuestCategory> QuestCategories => Set<QuestCategory>();
    public DbSet<QuestType> QuestTypes => Set<QuestType>();
    public DbSet<QuestArea> QuestAreas => Set<QuestArea>();
    public DbSet<Quest> Quests => Set<Quest>();
    public DbSet<JournalInstanceMedia> JournalInstanceMedias => Set<JournalInstanceMedia>();
    public DbSet<AuctionRecord> AuctionRecords => Set<AuctionRecord>();
    public DbSet<LootItemSummary> G_LootItemSummaries => Set<LootItemSummary>();
    public DbSet<LootLocationEntry> G_LootLocationEntries => Set<LootLocationEntry>();
    public DbSet<NpcKillCount> G_NpcKillCounts => Set<NpcKillCount>();
    public DbSet<PetBattleLocation> G_PetBattleLocations => Set<PetBattleLocation>();
    public DbSet<Vendor> G_Vendors => Set<Vendor>();
    public DbSet<VendorItem> G_VendorItems => Set<VendorItem>();
    public DbSet<QuestLocation> G_QuestLocations => Set<QuestLocation>();

    public BlizzardAPIContext() : base(CreateOptions()) { }

    private static DbContextOptions<BlizzardAPIContext> CreateOptions() => new DbContextOptionsBuilder<BlizzardAPIContext>().UseNpgsql(ApplicationSettings.Instance.PostgresConnection).Options;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VendorItem>().HasKey(v => new { v.ItemId, v.VendorId, v.FactionId });
        modelBuilder.Entity<LootItemSummary>().HasKey(l => new { l.NpcId, l.ItemId });
        modelBuilder.Entity<LootLocationEntry>().HasKey(l => new { l.NpcId, l.X, l.Y, l.ZoneId });
        modelBuilder.Entity<QuestLocation>().HasKey(q => new { q.QuestId, q.IsStart, q.FactionId });
    }
}
