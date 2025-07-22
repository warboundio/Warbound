using System;
using System.Collections.Generic;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;

namespace Data.BlizzardAPI.Endpoints;

public class QuestCategoryIndexEndpoint : BaseBlizzardEndpoint<List<QuestCategory>>
{
    public override string BuildUrl() =>
        "https://us.api.blizzard.com/data/wow/quest/category/index?namespace=static-us&locale=en_US";

    public override List<QuestCategory> Parse(JsonElement json)
    {
        List<QuestCategory> questCategories = [];
        JsonElement categoriesArray = json.GetProperty("categories");

        foreach (JsonElement categoryElement in categoriesArray.EnumerateArray())
        {
            QuestCategory questCategoryObj = new();
            questCategoryObj.Id = categoryElement.GetProperty("id").GetInt32();
            questCategoryObj.Name = categoryElement.GetProperty("name").GetString()!;
            questCategoryObj.Status = ETLStateType.COMPLETE;
            questCategoryObj.LastUpdatedUtc = DateTime.UtcNow;

            questCategories.Add(questCategoryObj);
        }

        return questCategories;
    }
}
