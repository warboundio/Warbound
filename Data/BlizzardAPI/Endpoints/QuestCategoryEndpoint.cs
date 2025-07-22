using System;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;

namespace Data.BlizzardAPI.Endpoints;

public class QuestCategoryEndpoint : BaseBlizzardEndpoint<QuestCategory>
{
    public int CategoryId { get; }

    public QuestCategoryEndpoint(int categoryId)
    {
        CategoryId = categoryId;
    }

    public override string BuildUrl() =>
        $"https://us.api.blizzard.com/data/wow/quest/category/{CategoryId}?namespace=static-us&locale=en_US";

    public override QuestCategory Parse(JsonElement json)
    {
        QuestCategory questCategory = new();
        questCategory.Id = json.GetProperty("id").GetInt32();
        questCategory.Name = json.GetProperty("category").GetString()!;
        questCategory.Status = ETLStateType.COMPLETE;
        questCategory.LastUpdatedUtc = DateTime.UtcNow;

        return questCategory;
    }
}
