using System;
using System.Collections.Generic;
using System.Linq;
using Data.BlizzardAPI;
using Data.BlizzardAPI.Endpoints;

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
    }
}
