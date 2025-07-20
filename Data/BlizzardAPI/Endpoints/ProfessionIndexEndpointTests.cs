using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Data.BlizzardAPI.Endpoints;

public class ProfessionIndexEndpointTests
{
    [Fact]
    public void ItShouldParseProfessionIndexJsonCorrectly()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/ProfessionIndex.json");
        JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

        ProfessionIndexEndpoint endpoint = new();
        List<Profession> result = endpoint.Parse(jsonElement);

        Assert.NotNull(result);
        Assert.True(result.Count > 0);

        Profession blacksmithing = result.Find(p => p.Id == 164)!;
        Assert.Equal(164, blacksmithing.Id);
        Assert.Equal("Blacksmithing", blacksmithing.Name);

        Profession alchemy = result.Find(p => p.Id == 171)!;
        Assert.Equal(171, alchemy.Id);
        Assert.Equal("Alchemy", alchemy.Name);
    }
}
