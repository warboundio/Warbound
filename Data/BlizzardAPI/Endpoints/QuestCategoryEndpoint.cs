using System.Collections.Generic;
using System.Text.Json;
using Data.BlizzardAPI.General;

namespace Data.BlizzardAPI.Endpoints;

public class QuestCategoryEndpoint : BaseBlizzardEndpoint<List<int>>
{
    public int CategoryId { get; }

    public QuestCategoryEndpoint(int categoryId)
    {
        CategoryId = categoryId;
    }

    public override string BuildUrl() =>
        $"https://us.api.blizzard.com/data/wow/quest/category/{CategoryId}?namespace=static-us&locale=en_US";

    public override List<int> Parse(JsonElement json)
    {
        List<int> questIds = [];

        if (json.TryGetProperty("quests", out JsonElement questsArray))
        {
            foreach (JsonElement questElement in questsArray.EnumerateArray())
            {
                if (questElement.TryGetProperty("id", out JsonElement idElement))
                {
                    questIds.Add(idElement.GetInt32());
                }
            }
        }

        return questIds;
    }
}
