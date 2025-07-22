using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Endpoints;
using Data.BlizzardAPI.Enums;
using Microsoft.EntityFrameworkCore;

namespace Data.ETLs;

public class QuestTypeETL : RunnableBlizzardETL
{
    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<QuestTypeETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        List<QuestType> questTypesToProcess = await Context.QuestTypes.Where(x => x.Status == ETLStateType.NEEDS_ENRICHED).ToListAsync();
        return [.. questTypesToProcess.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        QuestType casted = (QuestType)item;
        QuestTypeEndpoint endpoint = new(casted.Id);
        QuestType enriched = await endpoint.GetAsync();

        casted.Name = enriched.Name;
        casted.Status = ETLStateType.COMPLETE;
        casted.LastUpdatedUtc = System.DateTime.UtcNow;

        SaveBuffer.Add(casted);
    }
}
