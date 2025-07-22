#pragma warning disable CS8600

using System;
using System.Linq;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;

namespace Data.BlizzardAPI.Endpoints;

public class AchievementMediaEndpoint : BaseBlizzardEndpoint<AchievementMedia>
{
    public int AchievementId { get; }

    public AchievementMediaEndpoint(int achievementId)
    {
        AchievementId = achievementId;
    }

    public override string BuildUrl() =>
        $"https://us.api.blizzard.com/data/wow/media/achievement/{AchievementId}?namespace=static-us&locale=en_US";

    public override AchievementMedia Parse(JsonElement json)
    {
        int id = json.GetProperty("id").GetInt32();

        JsonElement assets = json.GetProperty("assets");
        JsonElement firstAsset = assets.EnumerateArray().First();
        string url = firstAsset.GetProperty("value").GetString()!;

        AchievementMedia mediaObj = new();
        mediaObj.Id = id;
        mediaObj.URL = url;
        mediaObj.Status = ETLStateType.COMPLETE;
        mediaObj.LastUpdatedUtc = DateTime.UtcNow;

        return mediaObj;
    }
}
