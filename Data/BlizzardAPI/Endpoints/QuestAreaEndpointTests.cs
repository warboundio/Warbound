using System.IO;
using System.Linq;
using System.Text.Json;

namespace Data.BlizzardAPI.Endpoints;

public class QuestAreaEndpointTests
{
    [Fact]
    public void ItShouldParseQuestAreaJsonCorrectly()
    {
        string jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "BlizzardAPI", "Endpoints", "QuestArea.json");
        string jsonContent = File.ReadAllText(jsonPath);
        JsonDocument document = JsonDocument.Parse(jsonContent);
        JsonElement root = document.RootElement;

        QuestAreaEndpoint endpoint = new(1);
        QuestArea result = endpoint.Parse(root);

        Assert.Equal(1, result.Id);
        Assert.Equal("Dun Morogh", result.Name);
    }

    [Fact]
    public void ItShouldParseQuestAreaJsonWithQuestsCorrectly()
    {
        string jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "BlizzardAPI", "Endpoints", "QuestArea.json");
        string jsonContent = File.ReadAllText(jsonPath);
        JsonDocument document = JsonDocument.Parse(jsonContent);
        JsonElement root = document.RootElement;

        JsonElement questsElement = root.GetProperty("quests");
        JsonElement firstQuest = questsElement.EnumerateArray().First();

        Assert.Equal(313, firstQuest.GetProperty("id").GetInt32());
        Assert.Equal("Forced to Watch from Afar", firstQuest.GetProperty("name").GetString());
    }
}
