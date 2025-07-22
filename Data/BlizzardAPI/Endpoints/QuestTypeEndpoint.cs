using System.Collections.Generic;
using System.Text.Json;
using Data.BlizzardAPI.General;

namespace Data.BlizzardAPI.Endpoints;

public class QuestTypeEndpoint : BaseBlizzardEndpoint<List<int>>
{
    public int QuestTypeId { get; }

    public QuestTypeEndpoint(int questTypeId)
    {
        QuestTypeId = questTypeId;
    }

    public override string BuildUrl() =>
        $"https://us.api.blizzard.com/data/wow/quest/type/{QuestTypeId}?namespace=static-us&locale=en_US";

    public override List<int> Parse(JsonElement json)
    {
        List<int> questIds = [];
        JsonElement questsArray = json.GetProperty("quests");

        foreach (JsonElement questElement in questsArray.EnumerateArray())
        {
            int questId = questElement.GetProperty("id").GetInt32();
            questIds.Add(questId);
        }

        return questIds;
    }
}
