using System.IO;
using System.Text.Json;

namespace Data.BlizzardAPI.Endpoints;

public class QuestTypeEndpointTests
{
    [Fact]
    public void ItShouldParseQuestTypeFromJson()
    {
        string jsonContent = File.ReadAllText("/home/runner/work/Warbound/Warbound/Data/BlizzardAPI/Endpoints/QuestType.json");
        JsonElement json = JsonSerializer.Deserialize<JsonElement>(jsonContent);

        QuestTypeEndpoint endpoint = new(1);
        QuestType result = endpoint.Parse(json);

        Assert.Equal(1, result.Id);
        Assert.Equal("Group", result.Name);
    }
}
