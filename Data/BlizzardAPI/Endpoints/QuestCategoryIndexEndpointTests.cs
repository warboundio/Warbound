using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.Models;

namespace Data.BlizzardAPI.Endpoints;

public class QuestCategoryIndexEndpointTests
{
    [Fact]
    public void ItShouldParseQuestCategoryIndexJsonCorrectly()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/Json/QuestCategoriesIndex.json");
        QuestCategoryIndexEndpoint endpoint = new();

        List<QuestCategory> results = endpoint.Parse(JsonSerializer.Deserialize<JsonElement>(json));

        Assert.NotNull(results);
        Assert.True(results.Count > 0);

        QuestCategory firstQuestCategory = results.First();
        Assert.Equal(1, firstQuestCategory.Id);
        Assert.Equal("Epic", firstQuestCategory.Name);
        Assert.Equal(ETLStateType.COMPLETE, firstQuestCategory.Status);

        Assert.True(results.Count > 10);
    }
}
