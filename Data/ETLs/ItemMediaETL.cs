using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Endpoints;
using Data.BlizzardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.ETLs;

public class ItemMediaETL : RunnableBlizzardETL
{
    private List<Item> _itemsToProcess = [];

    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<ItemMediaETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        List<int> existingItemMediaIds = await Context.ItemMedias.Select(x => x.Id).ToListAsync();
        _itemsToProcess = await Context.Items.Where(x => !existingItemMediaIds.Contains(x.Id)).ToListAsync();
        return [.. _itemsToProcess.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        Item casted = (Item)item;

        ItemMediaEndpoint endpoint = new(casted.Id);
        ItemMedia result = await endpoint.GetAsync();

        SaveBuffer.Add(result);
    }
}
