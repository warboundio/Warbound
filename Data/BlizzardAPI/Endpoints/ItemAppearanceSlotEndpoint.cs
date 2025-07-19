using System.Collections.Generic;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;

namespace Data.BlizzardAPI.Endpoints;
public class ItemAppearanceSlotEndpoint : BaseBlizzardEndpoint<List<int>>
{
    public SlotURLTypes Slot { get; }

    public ItemAppearanceSlotEndpoint(SlotURLTypes slot)
    {
        Slot = slot;
    }

    public override string BuildUrl() => $"https://us.api.blizzard.com/data/wow/item-appearance/slot/{Slot}?namespace=static-us&locale=en_US";

    public override List<int> Parse(JsonElement json)
    {
        List<int> ids = [];
        foreach (JsonElement appearance in json.GetProperty("appearances").EnumerateArray()) { ids.Add(appearance.GetProperty("id").GetInt32()); }
        return ids;
    }
}
