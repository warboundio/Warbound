using System.Collections.Generic;
using System.Text.Json;
using Data.BlizzardAPI.General;

namespace Data.BlizzardAPI.Endpoints;

public class CommodityEndpoint : BaseBlizzardEndpoint<List<AuctionsEndpoint.AuctionDatum>>
{
    public override string BuildUrl() =>
        "https://us.api.blizzard.com/data/wow/auctions/commodities?namespace=dynamic-us&locale=en_US";

    public override List<AuctionsEndpoint.AuctionDatum> Parse(JsonElement json)
    {
        List<AuctionsEndpoint.AuctionDatum> commodities = [];
        foreach (JsonElement auctionElement in json.GetProperty("auctions").EnumerateArray())
        {
            int itemId = auctionElement.GetProperty("item").GetProperty("id").GetInt32();
            long unitPrice = auctionElement.GetProperty("unit_price").GetInt64();
            int quantity = auctionElement.GetProperty("quantity").GetInt32();

            commodities.Add(new AuctionsEndpoint.AuctionDatum
            {
                ItemId = itemId,
                Buyout = unitPrice,
                Quantity = quantity
            });
        }
        return commodities;
    }
}
