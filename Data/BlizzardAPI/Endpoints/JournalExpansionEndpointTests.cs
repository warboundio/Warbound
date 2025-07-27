using System.IO;
using System.Text.Json;
using Data.BlizzardAPI.Models;

namespace Data.BlizzardAPI.Endpoints;

public class JournalExpansionEndpointTests
{
    [Fact]
    public void ItShouldParseJournalExpansionJsonCorrectly()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/JournalExpansion.json");
        JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

        JournalExpansionEndpoint endpoint = new(68);
        JournalExpansion result = endpoint.Parse(jsonElement);

        Assert.NotNull(result);
        Assert.Equal(68, result.Id);
        Assert.Equal("Classic", result.Name);

        Assert.Equal("227;228;63;230;1277;1276;231;229;232;226;233;234;311;316;246;64;236;1292;238;237;239;240;241", result.DungeonIds);
        Assert.Equal("741;742;743;744;1301", result.RaidIds);
    }
}
