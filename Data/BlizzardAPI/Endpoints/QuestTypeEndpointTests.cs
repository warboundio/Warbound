using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Data.BlizzardAPI.Endpoints;

public class QuestTypeEndpointTests
{
    [Fact]
    public void ItShouldParseQuestIdsFromJson()
    {
        string jsonContent = File.ReadAllText("/home/runner/work/Warbound/Warbound/Data/BlizzardAPI/Endpoints/QuestType.json");
        JsonElement json = JsonSerializer.Deserialize<JsonElement>(jsonContent);

        QuestTypeEndpoint endpoint = new(1);
        List<int> result = endpoint.Parse(json);

        Assert.Equal(3, result.Count);
        Assert.Contains(176, result);
        Assert.Contains(543, result);
        Assert.Contains(75657, result);
    }
}
