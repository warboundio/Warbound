using System;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;

namespace Data.BlizzardAPI.Endpoints;

public class RealmEndpoint : BaseBlizzardEndpoint<Realm>
{
    public string RealmSlug { get; }

    public RealmEndpoint(string realmSlug)
    {
        RealmSlug = realmSlug;
    }

    public override string BuildUrl() =>
        $"https://us.api.blizzard.com/data/wow/realm/{RealmSlug}?namespace=dynamic-us&locale=en_US";

    public override Realm Parse(JsonElement json)
    {
        Realm realmObj = new();
        realmObj.Id = json.GetProperty("id").GetInt32();
        realmObj.Name = json.GetProperty("name").GetString()!;
        realmObj.Slug = json.GetProperty("slug").GetString()!;
        realmObj.Category = json.GetProperty("category").GetString()!;
        realmObj.Locale = json.GetProperty("locale").GetString()!;
        realmObj.Timezone = json.GetProperty("timezone").GetString()!;
        realmObj.Type = json.GetProperty("type").GetProperty("type").GetString()!;
        realmObj.IsTournament = json.GetProperty("is_tournament").GetBoolean();
        realmObj.Region = json.GetProperty("region").GetProperty("name").GetString()!;
        realmObj.Status = ETLStateType.COMPLETE;
        realmObj.LastUpdatedUtc = DateTime.UtcNow;

        return realmObj;
    }
}
