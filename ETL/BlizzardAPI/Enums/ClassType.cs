using System;

namespace ETL.BlizzardAPI.Enums;

public enum ClassType
{
    UNKNOWN = -1,
    BATTLE_PETS = 17,
    CONSUMABLE = 0,
    CONTAINER = 1,
    WEAPON = 2,
    GEM = 3,
    ARMOR = 4,
    REAGENT = 5,
    PROJECTILE = 6,
    TRADESKILL = 7,
    ITEM_ENHANCEMENT = 8,
    RECIPE = 9,
    QUIVER = 11,
    QUEST = 12,
    KEY = 13,
    MISCELLANEOUS = 15,
    GLYPH = 16,
    WOW_TOKEN = 18,
    PROFESSION = 19
}

public static class ClassTypeHelper
{
    public static ClassType FromName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentException("ClassType name must not be null or whitespace.", nameof(name)); }

        string normalized = name
            .Trim()
            .ToUpperInvariant()
            .Replace(" ", "_");

        return Enum.TryParse(normalized, out ClassType result) ? result : throw new ArgumentException($"Unknown ClassType name: '{name}'", nameof(name));
    }
}
