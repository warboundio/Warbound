using System.IO;
using System.Text.Json;
using Data.BlizzardAPI.Enums;

namespace Data.BlizzardAPI.Endpoints;

public class AchievementEndpointTests
{
    public const int VALID_ID = 9713;

    [Fact]
    public void ItShouldParseAchievementJsonCorrectly()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/Achievement.json");
        AchievementEndpoint endpoint = new(VALID_ID);

        Achievement? result = endpoint.Parse(JsonSerializer.Deserialize<JsonElement>(json));

        Assert.NotNull(result);
        Assert.Equal(VALID_ID, result.Id);
        Assert.Equal("Awake the Drakes", result.Name);
        Assert.Equal("Collect the following drake mounts.", result.Description);
        Assert.Equal("Mounts", result.CategoryName);
        Assert.Equal(5, result.Points);
        Assert.Equal(ETLStateType.COMPLETE, result.Status);
    }
}
