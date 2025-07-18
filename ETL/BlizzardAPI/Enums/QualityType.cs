using System;
using System.Collections.Generic;

namespace ETL.BlizzardAPI.Enums;

public enum QualityType
{
    POOR = 0,
    COMMON = 1,
    UNCOMMON = 2,
    RARE = 3,
    EPIC = 4,
    LEGENDARY = 5,
    ARTIFACT = 6,
    HEIRLOOM = 7,
    WOW_TOKEN = 8,
    UNKNOWN = -1
}

public static class QualityTypeHelper
{
    private static readonly Dictionary<string, QualityType> _map = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Poor"] = QualityType.POOR,
        ["Common"] = QualityType.COMMON,
        ["Uncommon"] = QualityType.UNCOMMON,
        ["Rare"] = QualityType.RARE,
        ["Epic"] = QualityType.EPIC,
        ["Legendary"] = QualityType.LEGENDARY,
        ["Artifact"] = QualityType.ARTIFACT,
        ["Heirloom"] = QualityType.HEIRLOOM,
        ["WoW Token"] = QualityType.WOW_TOKEN
    };

    public static QualityType FromFieldName(string field)
    {
        return string.IsNullOrWhiteSpace(field)
            ? throw new ArgumentException("Field name must not be null or whitespace.", nameof(field))
            : _map.TryGetValue(field.Trim(), out QualityType result)
            ? result
            : throw new ArgumentException($"Unknown item quality type: '{field}'.", nameof(field));
    }
}
