using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace Data.ETLs;

public class JournalInstanceMediaETL : RunnableBlizzardETL
{
    private List<int> _instanceIdsToProcess = [];

    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<JournalInstanceMediaETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        List<int> existingJournalInstanceMediaIds = await Context.JournalInstanceMedias.Select(x => x.Id).ToListAsync();
        
        List<JournalExpansion> expansions = await Context.JournalExpansions.ToListAsync();
        
        List<int> allInstanceIds = [];
        
        foreach (JournalExpansion expansion in expansions)
        {
            if (!string.IsNullOrEmpty(expansion.DungeonIds))
            {
                var dungeonIds = expansion.DungeonIds.Split(',')
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => int.Parse(x.Trim()));
                allInstanceIds.AddRange(dungeonIds);
            }
            
            if (!string.IsNullOrEmpty(expansion.RaidIds))
            {
                var raidIds = expansion.RaidIds.Split(',')
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => int.Parse(x.Trim()));
                allInstanceIds.AddRange(raidIds);
            }
        }
        
        _instanceIdsToProcess = allInstanceIds.Where(x => !existingJournalInstanceMediaIds.Contains(x)).ToList();
        return [.. _instanceIdsToProcess.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        int instanceId = (int)item;

        JournalInstanceMediaEndpoint endpoint = new(instanceId);
        JournalInstanceMedia result = await endpoint.GetAsync();

        SaveBuffer.Add(result);
    }
}