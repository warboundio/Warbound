using System;
using System.Collections.Generic;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;

namespace Data.BlizzardAPI.Endpoints;

public class QuestTypeIndexEndpoint : BaseBlizzardEndpoint<List<QuestType>>
{
    public override string BuildUrl() =>
        "https://us.api.blizzard.com/data/wow/quest/type/index?namespace=static-us&locale=en_US";

    public override List<QuestType> Parse(JsonElement json)
    {
        List<QuestType> questTypes = [];
        JsonElement typesArray = json.GetProperty("types");

        foreach (JsonElement typeElement in typesArray.EnumerateArray())
        {
            QuestType questTypeObj = new();
            questTypeObj.Id = typeElement.GetProperty("id").GetInt32();
            questTypeObj.Name = typeElement.GetProperty("name").GetString()!;
            questTypeObj.Status = ETLStateType.COMPLETE;
            questTypeObj.LastUpdatedUtc = DateTime.UtcNow;

            questTypes.Add(questTypeObj);
        }

        return questTypes;
    }
}
