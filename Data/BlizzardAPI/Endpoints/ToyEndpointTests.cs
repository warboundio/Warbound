using System.IO;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.Models;

namespace Data.BlizzardAPI.Endpoints;

public class ToyEndpointTests
{
    public const int VALID_ID = 1131;

    [Fact]
    public void ItShouldParseToyJsonCorrectly()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/Toy.json");
        JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

        ToyEndpoint endpoint = new(VALID_ID);
        Toy result = endpoint.Parse(jsonElement);

        Assert.NotNull(result);
        Assert.Equal(VALID_ID, result.Id);
        Assert.Equal("Offering Kit Maker", result.Name);
        Assert.Equal("DROP", result.SourceType);
        Assert.Equal(187344, result.MediaId);
        Assert.Equal(ETLStateType.COMPLETE, result.Status);
    }
}
