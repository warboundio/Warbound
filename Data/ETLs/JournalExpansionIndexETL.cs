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

public class JournalExpansionIndexETL : RunnableBlizzardETL
{
    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<JournalExpansionIndexETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        HashSet<int> existingIds = await Context.JournalExpansions.Select(je => je.Id).ToHashSetAsync();

        JournalExpansionIndexEndpoint endpoint = new();
        List<JournalExpansion> journalExpansionsFromApi = await endpoint.GetAsync();
        List<JournalExpansion> newJournalExpansions = [.. journalExpansionsFromApi.Where(journalExpansion => !existingIds.Contains(journalExpansion.Id))];

        return [.. newJournalExpansions.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        await Task.Run(() =>
        {
            JournalExpansion journalExpansion = (JournalExpansion)item;

            SaveBuffer.Add(new JournalExpansion
            {
                Id = journalExpansion.Id,
                Name = journalExpansion.Name,
                Status = ETLStateType.NEEDS_ENRICHED,
                LastUpdatedUtc = DateTime.UtcNow
            });
        });
    }
}
