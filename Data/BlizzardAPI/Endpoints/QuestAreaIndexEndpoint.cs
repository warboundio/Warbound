using System;
using System.Collections.Generic;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;

namespace Data.BlizzardAPI.Endpoints;

public class QuestAreaIndexEndpoint : BaseBlizzardEndpoint<List<QuestArea>>
{
    public override string BuildUrl() =>
        "https://us.api.blizzard.com/data/wow/quest/area/index?namespace=static-us&locale=en_US";

    public override List<QuestArea> Parse(JsonElement json)
    {
        List<QuestArea> questAreas = [];
        JsonElement areasArray = json.GetProperty("areas");

        foreach (JsonElement areaElement in areasArray.EnumerateArray())
        {
            QuestArea questAreaObj = new();
            questAreaObj.Id = areaElement.GetProperty("id").GetInt32();
            questAreaObj.Name = areaElement.GetProperty("name").GetString()!;
            questAreaObj.Status = ETLStateType.COMPLETE;
            questAreaObj.LastUpdatedUtc = DateTime.UtcNow;

            questAreas.Add(questAreaObj);
        }

        return questAreas;
    }
}
