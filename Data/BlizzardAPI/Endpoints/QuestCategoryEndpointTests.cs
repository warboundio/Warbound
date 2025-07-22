using System.IO;
using System.Text.Json;
using Data.BlizzardAPI.Enums;

namespace Data.BlizzardAPI.Endpoints;

public class QuestCategoryEndpointTests
{
    [Fact]
    public void ItShouldParseQuestCategoryCorrectly()
    {
        string jsonContent = File.ReadAllText("BlizzardAPI/Endpoints/QuestCategory.json");
        JsonElement json = JsonDocument.Parse(jsonContent).RootElement;

        QuestCategoryEndpoint endpoint = new(1);
        QuestCategory result = endpoint.Parse(json);

        Assert.Equal(1, result.Id);
        Assert.Equal("Epic", result.Name);
        Assert.Equal(ETLStateType.COMPLETE, result.Status);
        Assert.True(result.LastUpdatedUtc != default);
    }
}
