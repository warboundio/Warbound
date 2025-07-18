#pragma warning disable CS8600

using System;
using System.Linq;
using System.Text.Json;
using ETL.BlizzardAPI.Enums;
using ETL.BlizzardAPI.General;

namespace ETL.BlizzardAPI.Endpoints;

public class ItemMediaEndpoint : BaseBlizzardEndpoint<ItemMedia>
{
    public int ItemId { get; }

    public ItemMediaEndpoint(int itemId)
    {
        ItemId = itemId;
    }

    public override string BuildUrl() =>
        $"https://us.api.blizzard.com/data/wow/media/item/{ItemId}?namespace=static-us&locale=en_US";

    public override ItemMedia Parse(JsonElement json)
    {
        int id = json.GetProperty("id").GetInt32();

        JsonElement assets = json.GetProperty("assets");
        JsonElement firstAsset = assets.EnumerateArray().First();
        string url = firstAsset.GetProperty("value").GetString()!;

        ItemMedia mediaObj = new();
        mediaObj.Id = id;
        mediaObj.URL = url;
        mediaObj.Status = ETLStateType.COMPLETE;
        mediaObj.LastUpdatedUtc = DateTime.UtcNow;

        return mediaObj;
    }
}
