using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Endpoints;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.ETLs;

public class JournalEncounterETL : RunnableBlizzardETL
{

    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<JournalEncounterETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        List<JournalEncounter> encountersToProcess = await Context.JournalEncounters.Where(x => x.Status == ETLStateType.NEEDS_ENRICHED).ToListAsync();
        return [.. encountersToProcess.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        JournalEncounter encounter = (JournalEncounter)item;

        JournalEncounterEndpoint endpoint = new(encounter.Id);
        JournalEncounter enriched = await endpoint.GetAsync();

        encounter.Items = enriched.Items;
        encounter.InstanceName = enriched.InstanceName;
        encounter.InstanceId = enriched.InstanceId;
        encounter.CategoryType = enriched.CategoryType;
        encounter.ModesTypes = enriched.ModesTypes;
        encounter.Status = ETLStateType.COMPLETE;
        encounter.LastUpdatedUtc = DateTime.UtcNow;

        SaveBuffer.Add(encounter);
    }
}
