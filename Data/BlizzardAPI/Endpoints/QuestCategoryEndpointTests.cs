using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Data.BlizzardAPI.Endpoints;

public class QuestCategoryEndpointTests
{
    [Fact]
    public void ItShouldParseQuestIdsCorrectly()
    {
        string jsonContent = File.ReadAllText("BlizzardAPI/Endpoints/Json/QuestCategory.json");
        JsonElement json = JsonDocument.Parse(jsonContent).RootElement;

        QuestCategoryEndpoint endpoint = new(1);
        List<int> result = endpoint.Parse(json);

        Assert.Equal(2, result.Count);
        Assert.Contains(8446, result);
        Assert.Contains(8447, result);
    }
}
