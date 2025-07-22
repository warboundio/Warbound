using System.IO;
using System.Text.Json;
using Data.BlizzardAPI.Enums;

namespace Data.BlizzardAPI.Endpoints;

public class MountEndpointTests
{
    public const int VALID_ID = 35;

    [Fact]
    public void ItShouldParseMountJsonCorrectly()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/Mount.json");
        MountEndpoint endpoint = new(VALID_ID);

        Mount? result = endpoint.Parse(JsonSerializer.Deserialize<JsonElement>(json));

        Assert.NotNull(result);
        Assert.Equal(VALID_ID, result.Id);
        Assert.Equal("Ivory Raptor", result.Name);
        Assert.Equal("VENDOR", result.SourceType);
        Assert.Equal(6471, result.CreatureDisplayId);
        Assert.Equal(ETLStateType.COMPLETE, result.Status);
    }
}
