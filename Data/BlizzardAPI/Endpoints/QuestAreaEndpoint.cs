using System;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;

namespace Data.BlizzardAPI.Endpoints;

public class QuestAreaEndpoint : BaseBlizzardEndpoint<QuestArea>
{
    public int AreaId { get; private set; }

    public QuestAreaEndpoint(int areaId) { AreaId = areaId; }

    public override string BuildUrl() => $"https://us.api.blizzard.com/data/wow/quest/area/{AreaId}?namespace=static-us&locale=en_US";

    public override QuestArea Parse(JsonElement json)
    {
        int id = json.GetProperty("id").GetInt32();
        string area = json.GetProperty("area").GetString()!;

        QuestArea questAreaObj = new();
        questAreaObj.Id = id;
        questAreaObj.Name = area;
        questAreaObj.Status = ETLStateType.COMPLETE;
        questAreaObj.LastUpdatedUtc = DateTime.UtcNow;

        return questAreaObj;
    }
}
