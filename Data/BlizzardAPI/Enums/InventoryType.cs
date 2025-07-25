using System;
using System.Collections.Generic;

namespace Data.BlizzardAPI.Enums;

public enum InventoryType
{
    UNKNOWN = -1,
    NON_EQUIP = 0,
    HEAD = 1,
    NECK = 2,
    SHOULDER = 3,
    BODY = 4,
    CHEST = 5,
    WAIST = 6,
    LEGS = 7,
    FEET = 8,
    WRIST = 9,
    HANDS = 10,
    FINGER = 11,
    TRINKET = 12,
    ONE_HAND = 13,
    OFF_HAND = 14,
    RANGED = 15,
    BACK = 16,
    TWO_HAND = 17,
    BAG = 18,
    TABARD = 19,
    ROBE = 20,
    MAIN_HAND = 21,
    HELD_IN_OFFHAND = 23,
    AMMO = 24,
    THROWN = 25,
    RANGED_RIGHT = 26,
    QUIVER = 27,
    RELIC = 28,
    PROFESSION_TOOL = 29,
    PROFESSION_EQUIPMENT = 30,
    EQUIPABLE_SPELL_OFFENSIVE = 31,
    EQUIPABLE_SPELL_UTILITY = 32,
    EQUIPABLE_SPELL_DEFENSIVE = 33,
    EQUIPABLE_SPELL_WEAPON = 34,
    UNKNOWN_ENCODING = 80
}

public static class InventoryTypeHelper
{
    private static readonly Dictionary<string, InventoryType> _map = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Head"] = InventoryType.HEAD,
        ["Neck"] = InventoryType.NECK,
        ["Shoulder"] = InventoryType.SHOULDER,
        ["Shirt"] = InventoryType.BODY,
        ["Chest"] = InventoryType.CHEST,
        ["Waist"] = InventoryType.WAIST,
        ["Legs"] = InventoryType.LEGS,
        ["Feet"] = InventoryType.FEET,
        ["Wrist"] = InventoryType.WRIST,
        ["Hands"] = InventoryType.HANDS,
        ["Finger"] = InventoryType.FINGER,
        ["Trinket"] = InventoryType.TRINKET,
        ["One-Hand"] = InventoryType.ONE_HAND,
        ["Off Hand"] = InventoryType.OFF_HAND,
        ["Ranged"] = InventoryType.RANGED,
        ["Back"] = InventoryType.BACK,
        ["Two-Hand"] = InventoryType.TWO_HAND,
        ["Bag"] = InventoryType.BAG,
        ["Tabard"] = InventoryType.TABARD,
        ["Robe"] = InventoryType.ROBE,
        ["Main Hand"] = InventoryType.MAIN_HAND,
        ["Held In Off-hand"] = InventoryType.HELD_IN_OFFHAND,
        ["Ammo"] = InventoryType.AMMO,
        ["Thrown"] = InventoryType.THROWN,
        ["Ranged Right"] = InventoryType.RANGED_RIGHT,
        ["Quiver"] = InventoryType.QUIVER,
        ["Relic"] = InventoryType.RELIC,
        ["Profession Tool"] = InventoryType.PROFESSION_TOOL,
        ["Profession Equipment"] = InventoryType.PROFESSION_EQUIPMENT,
        ["Equipable Spell - Offensive"] = InventoryType.EQUIPABLE_SPELL_OFFENSIVE,
        ["Equipable Spell - Utility"] = InventoryType.EQUIPABLE_SPELL_UTILITY,
        ["Equipable Spell - Defensive"] = InventoryType.EQUIPABLE_SPELL_DEFENSIVE,
        ["Equipable Spell - Weapon"] = InventoryType.EQUIPABLE_SPELL_WEAPON
    };

    public static InventoryType FromFieldName(string field)
    {
        return string.IsNullOrWhiteSpace(field)
            ? throw new ArgumentException("Field name must not be null or empty.", nameof(field))
            : _map.TryGetValue(field.Trim(), out InventoryType result)
                ? result
                : throw new ArgumentException($"Unknown inventory field: '{field}'.", nameof(field));
    }
}
