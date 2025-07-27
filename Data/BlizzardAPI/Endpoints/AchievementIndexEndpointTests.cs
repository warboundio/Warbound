using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.Models;

namespace Data.BlizzardAPI.Endpoints;

public class AchievementIndexEndpointTests
{
    [Fact]
    public void ItShouldParseAchievementIndexJsonCorrectly()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/AchievementIndex.json");
        AchievementIndexEndpoint endpoint = new();

        List<Achievement>? result = endpoint.Parse(JsonSerializer.Deserialize<JsonElement>(json));

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Achievement firstAchievement = result[0];
        Assert.Equal(463, firstAchievement.Id);
        Assert.Equal("Realm First! Level 80 Warlock (Legacy)", firstAchievement.Name);
        Assert.Equal(ETLStateType.NEEDS_ENRICHED, firstAchievement.Status);
    }
}
