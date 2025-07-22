using System;
using System.Collections.Generic;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;

namespace Data.BlizzardAPI.Endpoints;

public class JournalExpansionIndexEndpoint : BaseBlizzardEndpoint<List<JournalExpansion>>
{
    public override string BuildUrl() => "https://us.api.blizzard.com/data/wow/journal-expansion/index?namespace=static-us&locale=en_US";

    public override List<JournalExpansion> Parse(JsonElement json)
    {
        List<JournalExpansion> journalExpansions = [];
        foreach (JsonElement tierElement in json.GetProperty("tiers").EnumerateArray())
        {
            JournalExpansion journalExpansionObj = new();
            journalExpansionObj.Id = tierElement.GetProperty("id").GetInt32();
            journalExpansionObj.Name = tierElement.GetProperty("name").GetString()!;
            journalExpansionObj.Status = ETLStateType.NEEDS_ENRICHED;
            journalExpansionObj.LastUpdatedUtc = DateTime.UtcNow;

            journalExpansions.Add(journalExpansionObj);
        }
        return journalExpansions;
    }
}
