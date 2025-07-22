using System.IO;
using System.Text.Json;
using Data.BlizzardAPI.Enums;

namespace Data.BlizzardAPI.Endpoints;

public class JournalEncounterEndpointTests
{
    public const int VALID_ID = 89;

    [Fact]
    public void ItShouldParseJournalEncounterJsonCorrectly()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/JournalEncounter.json");
        JournalEncounterEndpoint endpoint = new(VALID_ID);

        JournalEncounter? result = endpoint.Parse(JsonSerializer.Deserialize<JsonElement>(json));

        Assert.NotNull(result);
        Assert.Equal(VALID_ID, result.Id);
        Assert.Equal("Glubtok", result.Name);
        Assert.Equal(ETLStateType.COMPLETE, result.Status);
    }
}
