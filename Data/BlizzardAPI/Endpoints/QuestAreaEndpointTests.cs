using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Data.BlizzardAPI.Endpoints;

public class QuestAreaEndpointTests
{
    [Fact]
    public void ItShouldParseQuestAreaJsonAndReturnQuestIds()
    {
        string jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "BlizzardAPI", "Endpoints", "QuestArea.json");
        string jsonContent = File.ReadAllText(jsonPath);
        JsonDocument document = JsonDocument.Parse(jsonContent);
        JsonElement root = document.RootElement;

        QuestAreaEndpoint endpoint = new(1);
        List<int> result = endpoint.Parse(root);

        Assert.NotEmpty(result);
        Assert.Contains(313, result);
        Assert.Contains(314, result);
        Assert.Contains(315, result);
    }

    [Fact]
    public void ItShouldReturnCorrectNumberOfQuests()
    {
        string jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "BlizzardAPI", "Endpoints", "QuestArea.json");
        string jsonContent = File.ReadAllText(jsonPath);
        JsonDocument document = JsonDocument.Parse(jsonContent);
        JsonElement root = document.RootElement;

        QuestAreaEndpoint endpoint = new(1);
        List<int> result = endpoint.Parse(root);

        JsonElement questsElement = root.GetProperty("quests");
        int expectedCount = questsElement.EnumerateArray().Count();

        Assert.Equal(expectedCount, result.Count);
    }

    [Fact]
    public void ItShouldReturnEmptyListWhenNoQuestsProperty()
    {
        string jsonWithoutQuests = /*lang=json,strict*/ "{ \"id\": 1, \"area\": \"Test Area\" }";
        JsonDocument document = JsonDocument.Parse(jsonWithoutQuests);
        JsonElement root = document.RootElement;

        QuestAreaEndpoint endpoint = new(1);
        List<int> result = endpoint.Parse(root);

        Assert.Empty(result);
    }
}
