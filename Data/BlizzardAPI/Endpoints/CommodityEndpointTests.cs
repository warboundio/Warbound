using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Data.BlizzardAPI.Endpoints;

public class CommodityEndpointTests
{
    [Fact]
    public void Parse_CommoditiesJsonFile_ReturnsExpectedAuctionDatumList()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/Json/Commodities.json");
        CommodityEndpoint endpoint = new();
        JsonDocument doc = JsonDocument.Parse(json);
        List<AuctionsEndpoint.AuctionDatum> result = endpoint.Parse(doc.RootElement);

        Assert.Equal(10, result.Count);

        // Check first auction
        Assert.Equal(192833, result[0].ItemId);
        Assert.Equal(80000, result[0].Buyout);
        Assert.Equal(20, result[0].Quantity);

        // Check last auction
        Assert.Equal(220145, result[9].ItemId);
        Assert.Equal(181000, result[9].Buyout);
        Assert.Equal(4, result[9].Quantity);
    }
}
