using System;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;
using Data.BlizzardAPI.Models;

namespace Data.BlizzardAPI.Endpoints;

public class ItemEndpoint : BaseBlizzardEndpoint<Item>
{
    public int ItemId { get; }

    public ItemEndpoint(int itemId)
    {
        ItemId = itemId;
    }

    public override string BuildUrl() =>
        $"https://us.api.blizzard.com/data/wow/item/{ItemId}?namespace=static-us&locale=en_US";

    public override Item Parse(JsonElement json)
    {
        Item itemObj = new();
        itemObj.Id = json.GetProperty("id").GetInt32();
        itemObj.Name = json.GetProperty("name").GetString()!;
        itemObj.QualityType = QualityTypeHelper.FromFieldName(json.GetProperty("quality").GetProperty("name").GetString()!);
        itemObj.Level = json.GetProperty("level").GetInt32();
        itemObj.RequiredLevel = json.GetProperty("required_level").GetInt32();
        itemObj.ClassType = json.GetProperty("item_class").GetProperty("name").GetString()!;

        itemObj.SubclassType = json.GetProperty("item_subclass").GetProperty("name").GetString()!;
        itemObj.InventoryType = json.GetProperty("inventory_type").GetProperty("name").GetString()!;
        itemObj.PurchasePrice = json.GetProperty("purchase_price").GetInt32();
        itemObj.SellPrice = json.GetProperty("sell_price").GetInt32();
        itemObj.MaxCount = json.GetProperty("max_count").GetInt32();
        itemObj.IsEquippable = json.GetProperty("is_equippable").GetBoolean();
        itemObj.IsStackable = json.GetProperty("is_stackable").GetBoolean();
        itemObj.LastUpdatedUtc = DateTime.UtcNow;
        itemObj.Status = ETLStateType.COMPLETE;

        return itemObj;
    }
}
