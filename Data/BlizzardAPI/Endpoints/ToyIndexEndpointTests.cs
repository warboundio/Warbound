using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Data.BlizzardAPI.Models;

namespace Data.BlizzardAPI.Endpoints;

public class ToyIndexEndpointTests
{
    [Fact]
    public void ItShouldParseToyIndexJsonCorrectly()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/ToyIndex.json");
        JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

        ToyIndexEndpoint endpoint = new();
        List<Toy> result = endpoint.Parse(jsonElement);

        Assert.NotNull(result);
        Assert.True(result.Count > 0);

        Toy firstToy = result.Find(t => t.Id == 30)!;
        Assert.Equal(30, firstToy.Id);
        Assert.Equal("Murloc Costume", firstToy.Name);

        Toy anotherToy = result.Find(t => t.Id == 366)!;
        Assert.Equal(366, anotherToy.Id);
        Assert.Equal("Rukhmar's Sacred Memory", anotherToy.Name);
    }
}
