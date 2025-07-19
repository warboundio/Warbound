using System;

namespace Data.BlizzardAPI.Enums;

public enum SlotType
{
    UNKNOWN = -1,
    HEAD = 1,
    LEGS = 2,
    BACK = 3,
    FEET = 4,
    SHOULDER = 5,
    CHEST = 6,
    WRIST = 7,
    ONE_HAND = 8,
    HANDS = 9,
    WAIST = 10,
    RANGED = 11,
    TWO_HAND = 12,
    SHIRT = 13,
    MAIN_HAND = 14,
    TABARD = 15,
    OFF_HAND = 16,
    SPELL_WEAPON = 17,
    PROFESSION = 18,
    AMMO = 19
}

public sealed class SlotTypeHelper
{
    public static SlotType FromFieldName(string field)
    {
        return string.IsNullOrWhiteSpace(field)
            ? throw new ArgumentException("Field name must not be null or whitespace.", nameof(field))
            : field switch
            {
                "Head" => SlotType.HEAD,
                "Legs" => SlotType.LEGS,
                "Back" => SlotType.BACK,
                "Feet" => SlotType.FEET,
                "Shoulder" => SlotType.SHOULDER,
                "Chest" => SlotType.CHEST,
                "Wrist" => SlotType.WRIST,
                "One-Hand" => SlotType.ONE_HAND,
                "Hands" => SlotType.HANDS,
                "Waist" => SlotType.WAIST,
                "Ranged" => SlotType.RANGED,
                "Two-Hand" => SlotType.TWO_HAND,
                "Shirt" => SlotType.SHIRT,
                "Main Hand" => SlotType.MAIN_HAND,
                "Tabard" => SlotType.TABARD,
                "Held in Off-hand" => SlotType.OFF_HAND,
                "Held In Off-hand" => SlotType.OFF_HAND,
                "Off Hand" => SlotType.OFF_HAND,
                "Equipable Spell - Weapon" => SlotType.SPELL_WEAPON,
                "Profession Tool" => SlotType.PROFESSION,
                "Ammo" => SlotType.AMMO,
                _ => throw new ArgumentException($"Unknown slot type: '{field}'.", nameof(field))
            };
    }
}
