using System.IO;
using System.Text.Json;
using Data.BlizzardAPI.Enums;

namespace Data.BlizzardAPI.Endpoints;

public class JournalInstanceMediaEndpointTests
{
    public const int VALID_ID = 63;

    [Fact]
    public void ItShouldParseJournalInstanceMediaJsonCorrectly()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/JournalInstanceMedia.json");
        JournalInstanceMediaEndpoint endpoint = new(VALID_ID);

        JournalInstanceMedia? result = endpoint.Parse(JsonSerializer.Deserialize<JsonElement>(json));

        Assert.NotNull(result);
        Assert.Equal(VALID_ID, result.Id);
        Assert.Equal("https://render.worldofwarcraft.com/us/zones/deadmines-small.jpg", result.URL);
        Assert.Equal(ETLStateType.COMPLETE, result.Status);
    }
}