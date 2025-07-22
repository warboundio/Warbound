using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Endpoints;
using Data.BlizzardAPI.Enums;
using Microsoft.EntityFrameworkCore;

namespace Data.ETLs;

public class JournalExpansionETL : RunnableBlizzardETL
{
    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<JournalExpansionETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        List<JournalExpansion> expansionsToProcess = await Context.JournalExpansions.Where(x => x.Status == ETLStateType.NEEDS_ENRICHED).ToListAsync();
        return [.. expansionsToProcess.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        JournalExpansion expansion = (JournalExpansion)item;

        JournalExpansionEndpoint endpoint = new(expansion.Id);
        JournalExpansion enriched = await endpoint.GetAsync();

        expansion.DungeonIds = enriched.DungeonIds;
        expansion.RaidIds = enriched.RaidIds;
        expansion.Status = ETLStateType.COMPLETE;
        expansion.LastUpdatedUtc = DateTime.UtcNow;

        SaveBuffer.Add(expansion);
    }
}
