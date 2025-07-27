using System;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;
using Data.BlizzardAPI.Models;

namespace Data.BlizzardAPI.Endpoints;

public class ToyEndpoint : BaseBlizzardEndpoint<Toy>
{
    public int ToyId { get; private set; }

    public ToyEndpoint(int toyId) { ToyId = toyId; }

    public override string BuildUrl() => $"https://us.api.blizzard.com/data/wow/toy/{ToyId}?namespace=static-us&locale=en_US";

    public override Toy Parse(JsonElement json)
    {
        Toy toyObj = new();
        toyObj.Id = json.GetProperty("id").GetInt32();
        toyObj.Name = json.GetProperty("item").GetProperty("name").GetString()!;
        toyObj.SourceType = json.GetProperty("source").GetProperty("type").GetString()!;
        toyObj.MediaId = json.GetProperty("media").GetProperty("id").GetInt32();
        toyObj.Status = ETLStateType.COMPLETE;
        toyObj.LastUpdatedUtc = DateTime.UtcNow;

        return toyObj;
    }
}
