using System;
using System.Collections.Generic;
using System.Text.Json;
using ETL.BlizzardAPI.Enums;
using ETL.BlizzardAPI.General;

namespace ETL.BlizzardAPI.Endpoints;

public class ItemAppearanceEndpoint : BaseBlizzardEndpoint<ItemAppearance>
{
    public int AppearanceId { get; private set; }

    public ItemAppearanceEndpoint(int appearanceId) { AppearanceId = appearanceId; }

    public override string BuildUrl() => $"https://us.api.blizzard.com/data/wow/item-appearance/{AppearanceId}?namespace=static-us&locale=en_US";

    public override ItemAppearance Parse(JsonElement json)
    {
        List<int> itemIds = [];

        try
        {
            foreach (JsonElement item in json.GetProperty("items").EnumerateArray())
            {
                itemIds.Add(item.GetProperty("id").GetInt32());
            }
        }
        catch (Exception)
        {
            ItemAppearance errorObj = new();
            errorObj.Id = AppearanceId;
            errorObj.SlotType = SlotType.UNKNOWN;
            errorObj.ClassType = ClassType.UNKNOWN;
            errorObj.SubclassType = SubclassType.UNKNOWN;
            errorObj.DisplayInfoId = -1;
            errorObj.ItemIds = string.Empty;
            errorObj.Status = ETLStateType.ERROR;
            errorObj.LastUpdatedUtc = DateTime.UtcNow;

            return errorObj;
        }

        string className = json.GetProperty("item_class").GetProperty("name").GetString()!;
        string subclassName = json.GetProperty("item_subclass").GetProperty("name").GetString()!;

        ClassType classType = ClassTypeHelper.FromName(className);
        SubclassType subclassType = SubclassTypeHelper.FromNames(className, subclassName);

        ItemAppearance appearanceObj = new();
        appearanceObj.Id = json.GetProperty("id").GetInt32();
        appearanceObj.SlotType = SlotTypeHelper.FromFieldName(json.GetProperty("slot").GetProperty("name").GetString()!);
        appearanceObj.ClassType = classType;
        appearanceObj.SubclassType = subclassType;
        appearanceObj.DisplayInfoId = json.GetProperty("item_display_info_id").GetInt32();
        appearanceObj.ItemIds = string.Join(";", itemIds);

        return appearanceObj;
    }
}
