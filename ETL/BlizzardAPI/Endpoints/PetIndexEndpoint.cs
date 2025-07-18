using System;
using System.Collections.Generic;
using System.Text.Json;
using ETL.BlizzardAPI.Enums;
using ETL.BlizzardAPI.General;

namespace ETL.BlizzardAPI.Endpoints;

public class PetIndexEndpoint : BaseBlizzardEndpoint<List<Pet>>
{
    public override string BuildUrl() => "https://us.api.blizzard.com/data/wow/pet/index?namespace=static-us&locale=en_US";

    public override List<Pet> Parse(JsonElement json)
    {
        List<Pet> pets = [];
        foreach (JsonElement pet in json.GetProperty("pets").EnumerateArray())
        {
            Pet petObj = new();
            petObj.Id = pet.GetProperty("id").GetInt32();
            petObj.Name = pet.GetProperty("name").GetString()!;
            petObj.Status = ETLStateType.NEEDS_ENRICHED;
            petObj.LastUpdatedUtc = DateTime.UtcNow;

            pets.Add(petObj);
        }

        return pets;
    }
}