using System.IO;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.Models;

namespace Data.BlizzardAPI.Endpoints;

public class QuestEndpointTests
{
    public const int VALID_AREA_QUEST_ID = 27868;
    public const int VALID_CATEGORY_QUEST_ID = 8446;

    [Fact]
    public void ItShouldParseAreaQuestJsonCorrectly()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/Quest.json");
        QuestEndpoint endpoint = new(VALID_AREA_QUEST_ID);

        Quest? result = endpoint.Parse(JsonSerializer.Deserialize<JsonElement>(json));

        Assert.NotNull(result);
        Assert.Equal(VALID_AREA_QUEST_ID, result.Id);
        Assert.Equal("The Crucible of Carnage: The Twilight Terror!", result.Name);
        Assert.Equal(1, result.QuestTypeId);
        Assert.Equal(QuestIdentifier.AREA, result.QuestIdentifier);
        Assert.Equal(4922, result.QuestIdentifierId);
        Assert.Equal("63789;63787;63788;63792;63790;63791", result.RewardItems);
        Assert.Equal(ETLStateType.COMPLETE, result.Status);
    }

    [Fact]
    public void ItShouldParseCategoryQuestJsonCorrectly()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/Quest2.json");
        QuestEndpoint endpoint = new(VALID_CATEGORY_QUEST_ID);

        Quest? result = endpoint.Parse(JsonSerializer.Deserialize<JsonElement>(json));

        Assert.NotNull(result);
        Assert.Equal(VALID_CATEGORY_QUEST_ID, result.Id);
        Assert.Equal("Shrouded in Nightmare", result.Name);
        Assert.Equal(QuestIdentifier.CATEGORY, result.QuestIdentifier);
        Assert.Equal(1, result.QuestIdentifierId);
        Assert.Equal(string.Empty, result.RewardItems);
        Assert.Equal(ETLStateType.COMPLETE, result.Status);
    }

    [Fact]
    public void ItShouldHandleMissingTypeId()
    {
        string json = /*lang=json,strict*/ @"{
            ""id"": 12345,
            ""title"": ""Test Quest"",
            ""area"": {
                ""id"": 100
            }
        }";
        QuestEndpoint endpoint = new(12345);

        Quest? result = endpoint.Parse(JsonSerializer.Deserialize<JsonElement>(json));

        Assert.NotNull(result);
        Assert.Equal(0, result.QuestTypeId);
        Assert.Equal(QuestIdentifier.AREA, result.QuestIdentifier);
        Assert.Equal(100, result.QuestIdentifierId);
    }

    [Fact]
    public void ItShouldHandleMissingAreaAndCategory()
    {
        string json = /*lang=json,strict*/ @"{
            ""id"": 12345,
            ""title"": ""Test Quest"",
            ""type"": {
                ""id"": 2
            }
        }";
        QuestEndpoint endpoint = new(12345);

        Quest? result = endpoint.Parse(JsonSerializer.Deserialize<JsonElement>(json));

        Assert.NotNull(result);
        Assert.Equal(2, result.QuestTypeId);
        Assert.Equal(QuestIdentifier.UNKNOWN, result.QuestIdentifier);
        Assert.Equal(0, result.QuestIdentifierId);
    }

    [Fact]
    public void ItShouldHandleMissingRewardItems()
    {
        string json = /*lang=json,strict*/ @"{
            ""id"": 12345,
            ""title"": ""Test Quest"",
            ""area"": {
                ""id"": 100
            },
            ""rewards"": {
                ""experience"": 1000
            }
        }";
        QuestEndpoint endpoint = new(12345);

        Quest? result = endpoint.Parse(JsonSerializer.Deserialize<JsonElement>(json));

        Assert.NotNull(result);
        Assert.Equal(string.Empty, result.RewardItems);
    }
}
