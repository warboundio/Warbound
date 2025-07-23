using System.Collections.Generic;
using System.Text.Json;
using Data.BlizzardAPI.General;
using static Data.BlizzardAPI.Endpoints.AuctionsEndpoint;

namespace Data.BlizzardAPI.Endpoints;

public class AuctionsEndpoint : BaseBlizzardEndpoint<List<AuctionDatum>>
{
    private readonly int _connectedRealmId;

    public AuctionsEndpoint(int connectedRealmId)
    {
        _connectedRealmId = connectedRealmId;
    }

    public override string BuildUrl() =>
        $"https://us.api.blizzard.com/data/wow/connected-realm/{_connectedRealmId}/auctions?namespace=dynamic-us&locale=en_US";

    public override List<AuctionDatum> Parse(JsonElement json)
    {
        List<AuctionDatum> auctions = [];
        foreach (JsonElement auctionElement in json.GetProperty("auctions").EnumerateArray())
        {
            int itemId = auctionElement.GetProperty("item").GetProperty("id").GetInt32();
            long buyout = auctionElement.GetProperty("buyout").GetInt64();
            int quantity = auctionElement.GetProperty("quantity").GetInt32();

            auctions.Add(new AuctionDatum
            {
                ItemId = itemId,
                Buyout = buyout,
                Quantity = quantity
            });
        }
        return auctions;
    }

    public class AuctionDatum
    {
        public int ItemId { get; set; }
        public long Buyout { get; set; }
        public int Quantity { get; set; }
    }
}
