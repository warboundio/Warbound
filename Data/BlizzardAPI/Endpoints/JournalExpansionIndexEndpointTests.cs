using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Data.BlizzardAPI.Endpoints;

public class JournalExpansionIndexEndpointTests
{
    [Fact]
    public void ItShouldParseJournalExpansionIndexJsonCorrectly()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/JournalExpansionsIndex.json");
        JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

        JournalExpansionIndexEndpoint endpoint = new();
        List<JournalExpansion> result = endpoint.Parse(jsonElement);

        Assert.NotNull(result);
        Assert.True(result.Count > 0);

        JournalExpansion classicExpansion = result.Find(je => je.Id == 68)!;
        Assert.Equal(68, classicExpansion.Id);
        Assert.Equal("Classic", classicExpansion.Name);

        JournalExpansion dragonflightExpansion = result.Find(je => je.Id == 503)!;
        Assert.Equal(503, dragonflightExpansion.Id);
        Assert.Equal("Dragonflight", dragonflightExpansion.Name);
    }
}