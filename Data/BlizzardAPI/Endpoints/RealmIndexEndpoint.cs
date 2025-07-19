using System;
using System.Collections.Generic;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;

namespace Data.BlizzardAPI.Endpoints;

public class RealmIndexEndpoint : BaseBlizzardEndpoint<List<Realm>>
{
    public override string BuildUrl() =>
        "https://us.api.blizzard.com/data/wow/realm/?namespace=dynamic-us&locale=en_US";

    public override List<Realm> Parse(JsonElement json)
    {
        List<Realm> realms = [];
        JsonElement realmsArray = json.GetProperty("realms");

        foreach (JsonElement realmElement in realmsArray.EnumerateArray())
        {
            Realm realmObj = new();
            realmObj.Id = realmElement.GetProperty("id").GetInt32();
            realmObj.Name = realmElement.GetProperty("name").GetString()!;
            realmObj.Slug = realmElement.GetProperty("slug").GetString()!;
            realmObj.Status = ETLStateType.NEEDS_ENRICHED;
            realmObj.LastUpdatedUtc = DateTime.UtcNow;

            realms.Add(realmObj);
        }

        return realms;
    }
}
