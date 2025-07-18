using System.IO;
using System.Text.Json;
using ETL.BlizzardAPI.Enums;

namespace ETL.BlizzardAPI.Endpoints;

public class ItemMediaEndpointTests
{
    public const int VALID_ID = 19019;

    [Fact]
    public void ItShouldParseItemMediaJsonCorrectly()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/ItemMedia.json");
        ItemMediaEndpoint endpoint = new(VALID_ID);

        ItemMedia? result = endpoint.Parse(JsonSerializer.Deserialize<JsonElement>(json));

        Assert.NotNull(result);
        Assert.Equal(VALID_ID, result.Id);
        Assert.Equal("https://render.worldofwarcraft.com/us/icons/56/inv_sword_39.jpg", result.URL);
        Assert.Equal(ETLStateType.COMPLETE, result.Status);
    }
}
