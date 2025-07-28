using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Endpoints;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.ETLs;

public class ItemETL : RunnableBlizzardETL
{
    private List<Item> _itemsToProcess = [];

    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<ItemETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        _itemsToProcess = await Context.Items.Where(x => x.Status == ETLStateType.NEEDS_ENRICHED).ToListAsync();
        return [.. _itemsToProcess.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        Item casted = (Item)item;
        ItemEndpoint endpoint = new(casted.Id);
        Item enriched = await endpoint.GetAsync();

        casted.Name = enriched.Name;
        casted.QualityType = enriched.QualityType;
        casted.Level = enriched.Level;
        casted.RequiredLevel = enriched.RequiredLevel;
        casted.ClassType = enriched.ClassType;
        casted.SubclassType = enriched.SubclassType;
        casted.InventoryType = enriched.InventoryType;
        casted.PurchasePrice = enriched.PurchasePrice;
        casted.SellPrice = enriched.SellPrice;
        casted.MaxCount = enriched.MaxCount;
        casted.IsEquippable = enriched.IsEquippable;
        casted.IsStackable = enriched.IsStackable;
        casted.Status = ETLStateType.COMPLETE;
        casted.LastUpdatedUtc = System.DateTime.UtcNow;

        SaveBuffer.Add(casted);
    }
}
