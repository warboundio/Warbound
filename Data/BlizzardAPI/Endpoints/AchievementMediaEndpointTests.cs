using System.IO;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.Models;

namespace Data.BlizzardAPI.Endpoints;

public class AchievementMediaEndpointTests
{
    public const int VALID_ID = 6;

    [Fact]
    public void ItShouldParseAchievementMediaJsonCorrectly()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/AchievementMedia.json");
        AchievementMediaEndpoint endpoint = new(VALID_ID);

        AchievementMedia? result = endpoint.Parse(JsonSerializer.Deserialize<JsonElement>(json));

        Assert.NotNull(result);
        Assert.Equal(VALID_ID, result.Id);
        Assert.Equal("https://render.worldofwarcraft.com/us/icons/56/achievement_level_10.jpg", result.URL);
        Assert.Equal(ETLStateType.COMPLETE, result.Status);
    }
}
