using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Data.BlizzardAPI.Enums;

namespace Data.BlizzardAPI.Endpoints;

public class QuestTypeIndexEndpointTests
{
    [Fact]
    public void ItShouldParseQuestTypeIndexJsonCorrectly()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/QuestTypeIndex.json");
        QuestTypeIndexEndpoint endpoint = new();

        List<QuestType> results = endpoint.Parse(JsonSerializer.Deserialize<JsonElement>(json));

        Assert.NotNull(results);
        Assert.True(results.Count > 0);

        // Test first quest type
        QuestType firstQuestType = results.First();
        Assert.Equal(1, firstQuestType.Id);
        Assert.Equal("Group", firstQuestType.Name);
        Assert.Equal(ETLStateType.COMPLETE, firstQuestType.Status);

        // Verify multiple quest types are parsed
        Assert.True(results.Count > 5);

        // Check for specific known quest types from the JSON
        Assert.Contains(results, qt => qt.Id == 41 && qt.Name == "PvP");
        Assert.Contains(results, qt => qt.Id == 62 && qt.Name == "Raid");
        Assert.Contains(results, qt => qt.Id == 81 && qt.Name == "Dungeon");
    }
}