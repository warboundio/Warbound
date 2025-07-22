using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Data.BlizzardAPI.General;

namespace Data.BlizzardAPI.Endpoints;

public class QuestAreaEndpoint : BaseBlizzardEndpoint<List<int>>
{
    public int AreaId { get; private set; }

    public QuestAreaEndpoint(int areaId) { AreaId = areaId; }

    public override string BuildUrl() => $"https://us.api.blizzard.com/data/wow/quest/area/{AreaId}?namespace=static-us&locale=en_US";

    public override List<int> Parse(JsonElement json)
    {
        List<int> questIds = new();

        if (json.TryGetProperty("quests", out JsonElement questsElement))
        {
            questIds = questsElement.EnumerateArray()
                .Select(questElement => questElement.GetProperty("id").GetInt32())
                .ToList();
        }

        return questIds;
    }
}
