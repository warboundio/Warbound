#pragma warning disable CS8600

using System;
using System.Linq;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;
using Data.BlizzardAPI.Models;

namespace Data.BlizzardAPI.Endpoints;

public class JournalInstanceMediaEndpoint : BaseBlizzardEndpoint<JournalInstanceMedia>
{
    public int InstanceId { get; }

    public JournalInstanceMediaEndpoint(int instanceId)
    {
        InstanceId = instanceId;
    }

    public override string BuildUrl() =>
        $"https://us.api.blizzard.com/data/wow/media/journal-instance/{InstanceId}?namespace=static-us&locale=en_US";

    public override JournalInstanceMedia Parse(JsonElement json)
    {
        JsonElement assets = json.GetProperty("assets");
        JsonElement firstAsset = assets.EnumerateArray().First();
        string url = firstAsset.GetProperty("value").GetString()!;

        JournalInstanceMedia mediaObj = new();
        mediaObj.Id = InstanceId;
        mediaObj.URL = url;
        mediaObj.Status = ETLStateType.COMPLETE;
        mediaObj.LastUpdatedUtc = DateTime.UtcNow;

        return mediaObj;
    }
}
