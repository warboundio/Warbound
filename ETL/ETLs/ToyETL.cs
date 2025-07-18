using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using ETL.BlizzardAPI.Endpoints;
using ETL.BlizzardAPI.Enums;
using Microsoft.EntityFrameworkCore;

namespace ETL.ETLs;

public class ToyETL : RunnableBlizzardETL
{
    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<ToyETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        List<Toy> toysToProcess = await Context.Toys.Where(x => x.Status == ETLStateType.NEEDS_ENRICHED).ToListAsync();
        return [.. toysToProcess.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        Toy toy = (Toy)item;

        ToyEndpoint endpoint = new(toy.Id);
        Toy enriched = await endpoint.GetAsync();

        toy.SourceType = enriched.SourceType;
        toy.MediaId = enriched.MediaId;
        toy.Status = ETLStateType.COMPLETE;
        toy.LastUpdatedUtc = DateTime.UtcNow;

        SaveBuffer.Add(toy);
    }
}
