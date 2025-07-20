using System;
using System.Collections.Generic;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;

namespace Data.BlizzardAPI.Endpoints;

public class ProfessionIndexEndpoint : BaseBlizzardEndpoint<List<Profession>>
{
    public override string BuildUrl() => "https://us.api.blizzard.com/data/wow/profession/index?namespace=static-us&locale=en_US";

    public override List<Profession> Parse(JsonElement json)
    {
        List<Profession> professions = [];
        foreach (JsonElement profession in json.GetProperty("professions").EnumerateArray())
        {
            Profession professionObj = new();
            professionObj.Id = profession.GetProperty("id").GetInt32();
            professionObj.Name = profession.GetProperty("name").GetString()!;
            professionObj.Status = ETLStateType.NEEDS_ENRICHED;
            professionObj.LastUpdatedUtc = DateTime.UtcNow;

            professions.Add(professionObj);
        }

        return professions;
    }
}
