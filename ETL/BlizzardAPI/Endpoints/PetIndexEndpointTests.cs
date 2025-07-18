using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ETL.BlizzardAPI.Endpoints;

public class PetIndexEndpointTests
{
    [Fact]
    public void ItShouldParsePetIndexJsonCorrectly()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/PetsIndex.json");
        JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

        PetIndexEndpoint endpoint = new();
        List<Pet> result = endpoint.Parse(jsonElement);

        Assert.NotNull(result);
        Assert.True(result.Count > 0);

        Pet firstPet = result.Find(p => p.Id == 283)!;
        Assert.Equal(283, firstPet.Id);
        Assert.Equal("Guild Herald", firstPet.Name);

        Pet anotherPet = result.Find(p => p.Id == 475)!;
        Assert.Equal(475, anotherPet.Id);
        Assert.Equal("Giraffe Calf", anotherPet.Name);
    }
}