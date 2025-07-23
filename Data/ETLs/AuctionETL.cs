using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Endpoints;
using static Data.BlizzardAPI.Endpoints.AuctionsEndpoint;

namespace Data.ETLs;

public class AuctionETL : RunnableBlizzardETL
{
    private List<int> _allItemIds = [];

    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<AuctionETL>(job);

    private readonly List<AuctionDatum> _commodities = [];
    private readonly List<AuctionDatum> _auctions = [];

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        int[] connectedRealms = [60, 58, 5]; // stormrage, stormreaver, proudmoore
        foreach(int realm in connectedRealms)
        {
            AuctionsEndpoint auctionEndpoint = new(realm);
            _auctions.AddRange(await auctionEndpoint.GetAsync(false));
        }

        CommodityEndpoint commodityEndpoint = new();
        _commodities.AddRange(await commodityEndpoint.GetAsync(false));

        _allItemIds = [.. _auctions.Select(a => a.ItemId).Union(_commodities.Select(c => c.ItemId)).Distinct()];
        return [.. _allItemIds.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        await Task.Run(() =>
        {
            int itemId = (int)item;
            SaveAuctionToDatabase(itemId);
            SaveCommodityToDatabase(itemId);
        });
    }

    private void SaveCommodityToDatabase(int itemId)
    {
        if (!_commodities.Any(c => c.ItemId == itemId)) { return; }

        long lowestPrice = GetCommodityPriceAtPercentile(itemId, 0.02);
        int quantity = _commodities.Where(c => c.ItemId == itemId).Sum(c => c.Quantity) / 3;

        AuctionRecord auctionRecord = new()
        {
            Id = Guid.NewGuid(),
            ItemId = itemId,
            CreatedOn = DateTime.UtcNow,
            IsCommodity = true,
            Price = lowestPrice,
            Quantity = quantity
        };

        if (auctionRecord.Quantity >= 50 && auctionRecord.Price != -1) { SaveBuffer.Add(auctionRecord); }
    }

    private void SaveAuctionToDatabase(int itemId)
    {
        if (!_auctions.Any(o => o.ItemId == itemId)) { return; }

        long lowestPrice = _auctions.Where(a => a.ItemId == itemId).Min(a => a.Buyout);
        int quantity = _auctions.Where(a => a.ItemId == itemId).Sum(a => a.Quantity) / 3;

        AuctionRecord auctionRecord = new()
        {
            Id = Guid.NewGuid(),
            ItemId = itemId,
            CreatedOn = DateTime.UtcNow,
            IsCommodity = false,
            Price = lowestPrice,
            Quantity = quantity
        };

        if (auctionRecord.Quantity >= 1) { SaveBuffer.Add(auctionRecord); }
    }

    public long GetCommodityPriceAtPercentile(int itemId, double percentile = 0.02)
    {
        List<AuctionDatum> listings = [.. _commodities.Where(c => c.ItemId == itemId).OrderBy(c => c.Buyout)];

        int totalQuantity = listings.Sum(c => c.Quantity);
        int cutoffQuantity = (int)Math.Ceiling(totalQuantity * percentile);

        int accumulated = 0;
        foreach (AuctionDatum listing in listings)
        {
            accumulated += listing.Quantity;
            if (accumulated >= cutoffQuantity)
            {
                return listing.Buyout;
            }
        }

        return -1;
    }
}
