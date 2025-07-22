using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace Data.ETLs;

public class JournalEncounterIndexETL : RunnableBlizzardETL
{
    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<JournalEncounterIndexETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        HashSet<int> existingIds = await Context.JournalEncounters.Select(j => j.Id).ToHashSetAsync();

        JournalEncounterIndexEndpoint endpoint = new();
        List<JournalEncounter> journalEncounterData = await endpoint.GetAsync();

        List<JournalEncounter> newJournalEncounters = [.. journalEncounterData.Where(encounter => !existingIds.Contains(encounter.Id))];
        return [.. newJournalEncounters.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        await Task.Run(() =>
        {
            JournalEncounter journalEncounter = (JournalEncounter)item;
            SaveBuffer.Add(journalEncounter);
        });
    }
}
