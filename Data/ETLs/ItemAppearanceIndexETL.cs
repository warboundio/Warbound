using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Endpoints;
using Data.BlizzardAPI.Enums;
using Microsoft.EntityFrameworkCore;

namespace Data.ETLs;

public class ItemAppearanceIndexETL : RunnableBlizzardETL
{
    private List<SlotURLTypes> _slotsToProcess = [];

    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<ItemAppearanceIndexETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        _slotsToProcess = [.. Enum.GetValues<SlotURLTypes>()];
        HashSet<int> existingIds = await Context.ItemAppearances.Select(x => x.Id).ToHashSetAsync();
        List<int> newIds = [];

        foreach (SlotURLTypes slot in _slotsToProcess)
        {
            ItemAppearanceSlotEndpoint endpoint = new(slot);
            List<int> ids = await endpoint.GetAsync();
            newIds.AddRange(ids.Where(id => !existingIds.Contains(id)));
        }

        return [.. newIds.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        await Task.Run(() =>
        {
            int id = (int)item;

            SaveBuffer.Add(new ItemAppearance
            {
                Id = id,
                Status = ETLStateType.NEEDS_ENRICHED,
                LastUpdatedUtc = DateTime.UtcNow
            });
        });
    }
}

