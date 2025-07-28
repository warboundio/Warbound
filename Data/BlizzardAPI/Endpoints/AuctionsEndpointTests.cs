using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Data.BlizzardAPI.Endpoints;

public class AuctionsEndpointTests
{
    [Fact]
    public void Parse_AuctionsJsonFile_ReturnsExpectedAuctionDatumList()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/Json/Auctions.json");
        AuctionsEndpoint endpoint = new(1185);
        JsonDocument doc = JsonDocument.Parse(json);
        List<AuctionsEndpoint.AuctionDatum> result = endpoint.Parse(doc.RootElement);

        // There are 20 auctions in the file
        Assert.Equal(20, result.Count);

        // Check first auction
        Assert.Equal(223487, result[0].ItemId);
        Assert.Equal(4850000, result[0].Buyout);
        Assert.Equal(1, result[0].Quantity);

        // Check last auction
        Assert.Equal(14131, result[19].ItemId);
        Assert.Equal(18883000, result[19].Buyout);
        Assert.Equal(1, result[19].Quantity);
    }
}
