using System;
using System.Collections.Generic;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;

namespace Data.BlizzardAPI.Endpoints;

public class JournalEncounterIndexEndpoint : BaseBlizzardEndpoint<List<JournalEncounter>>
{
    public override string BuildUrl() => "https://us.api.blizzard.com/data/wow/journal-encounter/index?namespace=static-us&locale=en_US";

    public override List<JournalEncounter> Parse(JsonElement json)
    {
        List<JournalEncounter> journalEncounters = [];
        foreach (JsonElement encounter in json.GetProperty("encounters").EnumerateArray())
        {
            JournalEncounter journalEncounter = new();
            journalEncounter.Id = encounter.GetProperty("id").GetInt32();
            journalEncounter.Name = encounter.GetProperty("name").GetString()!;
            journalEncounter.Status = ETLStateType.NEEDS_ENRICHED;
            journalEncounter.LastUpdatedUtc = DateTime.UtcNow;

            journalEncounters.Add(journalEncounter);
        }

        return journalEncounters;
    }
}