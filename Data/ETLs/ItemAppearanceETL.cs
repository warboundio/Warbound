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

public class ItemAppearanceETL : RunnableBlizzardETL
{
    private List<ItemAppearance> _itemsToProcess = [];

    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<ItemAppearanceETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        _itemsToProcess = await Context.ItemAppearances.Where(x => x.Status == ETLStateType.NEEDS_ENRICHED).ToListAsync();
        return [.. _itemsToProcess.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        ItemAppearance casted = (ItemAppearance)item;

        ItemAppearanceEndpoint endpoint = new(casted.Id);
        ItemAppearance enriched = await endpoint.GetAsync();

        casted.SlotType = enriched.SlotType;
        casted.ClassType = enriched.ClassType;
        casted.SubclassType = enriched.SubclassType;
        casted.DisplayInfoId = enriched.DisplayInfoId;
        casted.ItemIds = enriched.ItemIds;
        casted.Status = ETLStateType.COMPLETE;
        casted.LastUpdatedUtc = DateTime.UtcNow;

        SaveBuffer.Add(casted);
    }
}
