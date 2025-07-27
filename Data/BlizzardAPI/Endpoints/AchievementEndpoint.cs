using System;
using System.Collections.Generic;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;
using Data.BlizzardAPI.Models;

namespace Data.BlizzardAPI.Endpoints;

public class AchievementEndpoint : BaseBlizzardEndpoint<Achievement>
{
    public int AchievementId { get; private set; }

    public AchievementEndpoint(int achievementId) { AchievementId = achievementId; }

    public override string BuildUrl() => $"https://us.api.blizzard.com/data/wow/achievement/{AchievementId}?namespace=static-us&locale=en_US";

    public override Achievement Parse(JsonElement json)
    {
        int id = json.GetProperty("id").GetInt32();
        string name = json.GetProperty("name").GetString()!;
        string description = json.GetProperty("description").GetString() ?? string.Empty;
        int points = json.GetProperty("points").GetInt32();

        string categoryName = string.Empty;
        if (json.TryGetProperty("category", out JsonElement categoryElement) && categoryElement.TryGetProperty("name", out JsonElement categoryNameElement))
        {
            categoryName = categoryNameElement.GetString() ?? string.Empty;
        }

        string rewardDescription = string.Empty;
        if (json.TryGetProperty("reward_description", out JsonElement rewardDescElement))
        {
            rewardDescription = rewardDescElement.GetString() ?? string.Empty;
        }

        int? rewardItemId = null;
        string rewardItemName = string.Empty;
        if (json.TryGetProperty("reward_item", out JsonElement rewardItemElement))
        {
            if (rewardItemElement.TryGetProperty("id", out JsonElement rewardItemIdElement))
            {
                rewardItemId = rewardItemIdElement.GetInt32();
            }
            if (rewardItemElement.TryGetProperty("name", out JsonElement rewardItemNameElement))
            {
                rewardItemName = rewardItemNameElement.GetString() ?? string.Empty;
            }
        }

        string icon = string.Empty;
        if (json.TryGetProperty("media", out JsonElement mediaElement) && mediaElement.TryGetProperty("id", out JsonElement mediaIdElement))
        {
            icon = mediaIdElement.GetInt32().ToString();
        }

        List<int> criteriaIds = [];
        List<string> criteriaTypes = [];
        if (json.TryGetProperty("criteria", out JsonElement criteriaElement))
        {
            ExtractCriteriaData(criteriaElement, criteriaIds, criteriaTypes);
        }

        string criteriaIdsString = string.Join(";", criteriaIds);
        string criteriaTypesString = string.Join(";", criteriaTypes);

        Achievement achievement = new()
        {
            Id = id,
            Name = name,
            Description = description,
            CategoryName = categoryName,
            RewardDescription = rewardDescription,
            RewardItemId = rewardItemId,
            RewardItemName = rewardItemName,
            Points = points,
            Icon = icon,
            CriteriaIds = criteriaIdsString,
            CriteriaTypes = criteriaTypesString,
            Status = ETLStateType.COMPLETE,
            LastUpdatedUtc = DateTime.UtcNow
        };

        return achievement;
    }

    private void ExtractCriteriaData(JsonElement criteriaElement, List<int> criteriaIds, List<string> criteriaTypes)
    {
        if (criteriaElement.TryGetProperty("id", out JsonElement idElement))
        {
            criteriaIds.Add(idElement.GetInt32());
        }

        if (criteriaElement.TryGetProperty("operator", out JsonElement operatorElement) && operatorElement.TryGetProperty("type", out JsonElement typeElement))
        {
            criteriaTypes.Add(typeElement.GetString() ?? string.Empty);
        }

        if (criteriaElement.TryGetProperty("child_criteria", out JsonElement childCriteriaElement))
        {
            foreach (JsonElement childElement in childCriteriaElement.EnumerateArray())
            {
                ExtractCriteriaData(childElement, criteriaIds, criteriaTypes);
            }
        }
    }
}
