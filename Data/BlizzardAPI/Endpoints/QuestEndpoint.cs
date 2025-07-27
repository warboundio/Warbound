using System;
using System.Linq;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;
using Data.BlizzardAPI.Models;

namespace Data.BlizzardAPI.Endpoints;

public class QuestEndpoint : BaseBlizzardEndpoint<Quest>
{
    public int QuestId { get; }

    public QuestEndpoint(int questId)
    {
        QuestId = questId;
    }

    public override string BuildUrl() =>
        $"https://us.api.blizzard.com/data/wow/quest/{QuestId}?namespace=static-us&locale=en_US";

    public override Quest Parse(JsonElement json)
    {
        Quest quest = new();
        quest.Id = json.GetProperty("id").GetInt32();
        quest.Name = json.GetProperty("title").GetString()!;

        if (json.TryGetProperty("type", out JsonElement typeElement) && typeElement.TryGetProperty("id", out JsonElement typeIdElement))
        {
            quest.QuestTypeId = typeIdElement.GetInt32();
        }

        if (json.TryGetProperty("area", out JsonElement areaElement) && areaElement.TryGetProperty("id", out JsonElement areaIdElement))
        {
            quest.QuestIdentifier = QuestIdentifier.AREA;
            quest.QuestIdentifierId = areaIdElement.GetInt32();
        }
        else if (json.TryGetProperty("category", out JsonElement categoryElement) && categoryElement.TryGetProperty("id", out JsonElement categoryIdElement))
        {
            quest.QuestIdentifier = QuestIdentifier.CATEGORY;
            quest.QuestIdentifierId = categoryIdElement.GetInt32();
        }

        if (json.TryGetProperty("rewards", out JsonElement rewardsElement) &&
            rewardsElement.TryGetProperty("items", out JsonElement itemsElement) &&
            itemsElement.TryGetProperty("choice_of", out JsonElement choiceOfElement))
        {
            string[] itemIds = choiceOfElement.EnumerateArray()
                .Where(choice => choice.TryGetProperty("item", out JsonElement itemElement) && itemElement.TryGetProperty("id", out _))
                .Select(choice => choice.GetProperty("item").GetProperty("id").GetInt32().ToString())
                .ToArray();

            quest.RewardItems = string.Join(";", itemIds);
        }

        quest.Status = ETLStateType.COMPLETE;
        quest.LastUpdatedUtc = DateTime.UtcNow;

        return quest;
    }
}
