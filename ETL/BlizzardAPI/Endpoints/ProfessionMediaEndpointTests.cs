using System.IO;
using System.Text.Json;
using ETL.BlizzardAPI.Enums;

namespace ETL.BlizzardAPI.Endpoints;

public class ProfessionMediaEndpointTests
{
    public const int VALID_ID = 164;

    [Fact]
    public void ItShouldParseProfessionMediaData()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/ProfessionMedia.json");
        ProfessionMediaEndpoint endpoint = new(VALID_ID);

        ProfessionMedia? result = endpoint.Parse(JsonSerializer.Deserialize<JsonElement>(json));

        Assert.NotNull(result);
        Assert.Equal(VALID_ID, result.Id);
        Assert.Equal("https://render.worldofwarcraft.com/us/icons/56/ui_profession_blacksmithing.jpg", result.URL);
        Assert.Equal(ETLStateType.COMPLETE, result.Status);
    }
}