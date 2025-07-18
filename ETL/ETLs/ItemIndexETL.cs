using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using ETL.BlizzardAPI.Endpoints;
using ETL.BlizzardAPI.Enums;
using Microsoft.EntityFrameworkCore;

namespace ETL.ETLs;

public class ItemIndexETL : RunnableBlizzardETL
{
    private List<int> _newItemIds = [];

    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<ItemIndexETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        HashSet<int> existingItemIds = await Context.Items.Select(i => i.Id).ToHashSetAsync();
        List<ItemAppearance> appearances = await Context.ItemAppearances.Where(a => a.Status == ETLStateType.COMPLETE).ToListAsync();
        HashSet<int> discoveredItemIds = [];

        foreach (ItemAppearance appearance in appearances)
        {
            foreach (string id in appearance.ItemIds.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                discoveredItemIds.Add(int.Parse(id));
            }
        }

        _newItemIds = [.. discoveredItemIds.Where(id => !existingItemIds.Contains(id))];
        return [.. _newItemIds.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        await Task.Run(() =>
        {
            int id = (int)item;

            SaveBuffer.Add(new Item
            {
                Id = id,
                Status = ETLStateType.NEEDS_ENRICHED,
                LastUpdatedUtc = DateTime.UtcNow
            });
        });
    }
}
