using System;
using System.Collections.Generic;
using System.Linq;
using Data.Addon;
using Data.BlizzardAPI;
using Data.BlizzardAPI.Models;

namespace Data;

public sealed class WarcraftData
{
    private static readonly Lazy<WarcraftData> _instance = new(() => new WarcraftData());
    public static WarcraftData Instance => _instance.Value;

    public Dictionary<int, ItemAppearance> ItemAppearances { get; private set; } = [];
    public Dictionary<int, Item> Items { get; private set; } = [];
    public Dictionary<int, Toy> Toys { get; private set; } = [];
    public Dictionary<int, Mount> Mounts { get; private set; } = [];
    public Dictionary<int, Pet> Pets { get; private set; } = [];
    public Dictionary<int, Profession> Professions { get; private set; } = [];
    public Dictionary<int, ProfessionMedia> ProfessionMedias { get; private set; } = [];
    public Dictionary<int, Realm> Realms { get; private set; } = [];
    public Dictionary<int, ItemMedia> ItemMedias { get; private set; } = [];
    public Dictionary<int, Recipe> Recipes { get; private set; } = [];
    public Dictionary<int, RecipeMedia> RecipeMedias { get; private set; } = [];
    public Dictionary<int, Achievement> Achievements { get; private set; } = [];
    public Dictionary<int, AchievementMedia> AchievementMedias { get; private set; } = [];
    public Dictionary<int, JournalExpansion> JournalExpansions { get; private set; } = [];
    public Dictionary<int, JournalEncounter> JournalEncounters { get; private set; } = [];
    public Dictionary<int, QuestCategory> QuestCategories { get; private set; } = [];
    public Dictionary<int, QuestType> QuestTypes { get; private set; } = [];
    public Dictionary<int, QuestArea> QuestAreas { get; private set; } = [];
    public Dictionary<int, Quest> Quests { get; private set; } = [];
    public Dictionary<int, JournalInstanceMedia> JournalInstanceMedias { get; private set; } = [];
    public Dictionary<(int NpcId, int ItemId), LootItemSummary> G_LootItemSummaries { get; private set; } = [];
    public Dictionary<(int NpcId, int X, int Y, int ZoneId), LootLocationEntry> G_LootLocationEntries { get; private set; } = [];
    public Dictionary<int, NpcKillCount> G_NpcKillCounts { get; private set; } = [];
    public Dictionary<int, PetBattleLocation> G_PetBattleLocations { get; private set; } = [];
    public Dictionary<int, Vendor> G_Vendors { get; private set; } = [];
    public Dictionary<int, VendorItem> G_VendorItems { get; private set; } = [];
    public Dictionary<int, List<AuctionRecord>> G_Auctions { get; private set; } = [];

    private bool _isLoaded;

    private WarcraftData() { }

    public void Load()
    {
        if (_isLoaded) { return; }
        _isLoaded = true;

        using BlizzardAPIContext context = new();

        ItemAppearances = context.ItemAppearances.ToDictionary(x => x.Id);
        Items = context.Items.ToDictionary(x => x.Id);
        Toys = context.Toys.ToDictionary(x => x.Id);
        Mounts = context.Mounts.ToDictionary(x => x.Id);
        Pets = context.Pets.ToDictionary(x => x.Id);
        Professions = context.Professions.ToDictionary(x => x.Id);
        ProfessionMedias = context.ProfessionMedias.ToDictionary(x => x.Id);
        Realms = context.Realms.ToDictionary(x => x.Id);
        ItemMedias = context.ItemMedias.ToDictionary(x => x.Id);
        Recipes = context.Recipes.ToDictionary(x => x.Id);
        RecipeMedias = context.RecipeMedias.ToDictionary(x => x.Id);
        Achievements = context.Achievements.ToDictionary(x => x.Id);
        AchievementMedias = context.AchievementMedias.ToDictionary(x => x.Id);
        JournalExpansions = context.JournalExpansions.ToDictionary(x => x.Id);
        JournalEncounters = context.JournalEncounters.ToDictionary(x => x.Id);
        QuestCategories = context.QuestCategories.ToDictionary(x => x.Id);
        QuestTypes = context.QuestTypes.ToDictionary(x => x.Id);
        QuestAreas = context.QuestAreas.ToDictionary(x => x.Id);
        Quests = context.Quests.ToDictionary(x => x.Id);
        JournalInstanceMedias = context.JournalInstanceMedias.ToDictionary(x => x.Id);

        G_Auctions = context.AuctionRecords.GroupBy(x => x.ItemId).ToDictionary(g => g.Key, g => g.ToList());
        G_LootItemSummaries = context.G_LootItemSummaries.ToDictionary(x => (x.NpcId, x.ItemId));
        G_LootLocationEntries = context.G_LootLocationEntries.ToDictionary(x => (x.NpcId, x.X, x.Y, x.ZoneId));
        G_NpcKillCounts = context.G_NpcKillCounts.ToDictionary(x => x.NpcId);
        G_PetBattleLocations = context.G_PetBattleLocations.ToDictionary(x => x.PetSpeciesId);
        G_Vendors = context.G_Vendors.ToDictionary(x => x.NpcId);
        G_VendorItems = context.G_VendorItems.ToDictionary(x => x.VendorId);
    }
}
