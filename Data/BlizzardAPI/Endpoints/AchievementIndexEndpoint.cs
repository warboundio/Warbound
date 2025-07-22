using System;
using System.Collections.Generic;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;

namespace Data.BlizzardAPI.Endpoints;

public class AchievementIndexEndpoint : BaseBlizzardEndpoint<List<Achievement>>
{
    public override string BuildUrl() => "https://us.api.blizzard.com/data/wow/achievement/?namespace=static-us&locale=en_US";

    public override List<Achievement> Parse(JsonElement json)
    {
        List<Achievement> achievements = [];
        foreach (JsonElement achievementElement in json.GetProperty("achievements").EnumerateArray())
        {
            Achievement achievementObj = new();
            achievementObj.Id = achievementElement.GetProperty("id").GetInt32();
            achievementObj.Name = achievementElement.GetProperty("name").GetString()!;
            achievementObj.Status = ETLStateType.NEEDS_ENRICHED;
            achievementObj.LastUpdatedUtc = DateTime.UtcNow;

            achievements.Add(achievementObj);
        }
        return achievements;
    }
}