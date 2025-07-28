#pragma warning disable CS8600

using System;
using System.Linq;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;
using Data.BlizzardAPI.Models;

namespace Data.BlizzardAPI.Endpoints;

public class ProfessionMediaEndpoint : BaseBlizzardEndpoint<ProfessionMedia>
{
    public int ProfessionId { get; }

    public ProfessionMediaEndpoint(int professionId)
    {
        ProfessionId = professionId;
    }

    public override string BuildUrl() =>
        $"https://us.api.blizzard.com/data/wow/media/profession/{ProfessionId}?namespace=static-us&locale=en_US";

    public override ProfessionMedia Parse(JsonElement json)
    {
        int id = json.GetProperty("id").GetInt32();

        JsonElement assets = json.GetProperty("assets");
        JsonElement firstAsset = assets.EnumerateArray().First();
        string url = firstAsset.GetProperty("value").GetString()!;

        ProfessionMedia mediaObj = new();
        mediaObj.Id = id;
        mediaObj.URL = url;
        mediaObj.Status = ETLStateType.COMPLETE;
        mediaObj.LastUpdatedUtc = DateTime.UtcNow;

        return mediaObj;
    }
}
