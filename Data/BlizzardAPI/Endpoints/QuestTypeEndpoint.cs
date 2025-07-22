using System;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;

namespace Data.BlizzardAPI.Endpoints;

public class QuestTypeEndpoint : BaseBlizzardEndpoint<QuestType>
{
    public int QuestTypeId { get; }

    public QuestTypeEndpoint(int questTypeId)
    {
        QuestTypeId = questTypeId;
    }

    public override string BuildUrl() =>
        $"https://us.api.blizzard.com/data/wow/quest/type/{QuestTypeId}?namespace=static-us&locale=en_US";

    public override QuestType Parse(JsonElement json)
    {
        QuestType questTypeObj = new();
        questTypeObj.Id = json.GetProperty("id").GetInt32();
        questTypeObj.Name = json.GetProperty("type").GetString()!;
        questTypeObj.Status = ETLStateType.COMPLETE;
        questTypeObj.LastUpdatedUtc = DateTime.UtcNow;

        return questTypeObj;
    }
}
