using System;
using System.Collections.Generic;
using System.Text.Json;
using ETL.BlizzardAPI.Enums;
using ETL.BlizzardAPI.General;

namespace ETL.BlizzardAPI.Endpoints;

public class ToyIndexEndpoint : BaseBlizzardEndpoint<List<Toy>>
{
    public override string BuildUrl() => "https://us.api.blizzard.com/data/wow/toy/?namespace=static-us&locale=en_US";

    public override List<Toy> Parse(JsonElement json)
    {
        List<Toy> toys = [];
        foreach (JsonElement toy in json.GetProperty("toys").EnumerateArray())
        {
            Toy toyObj = new();
            toyObj.Id = toy.GetProperty("id").GetInt32();
            toyObj.Name = toy.GetProperty("name").GetString()!;
            toyObj.Status = ETLStateType.NEEDS_ENRICHED;
            toyObj.LastUpdatedUtc = DateTime.UtcNow;

            toys.Add(toyObj);
        }

        return toys;
    }
}
