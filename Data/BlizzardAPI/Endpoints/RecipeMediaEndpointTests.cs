using System.IO;
using System.Text.Json;
using Data.BlizzardAPI.Enums;

namespace Data.BlizzardAPI.Endpoints;

public class RecipeMediaEndpointTests
{
    public const int VALID_ID = 1631;

    [Fact]
    public void ItShouldParseRecipeMediaData()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/RecipeMedia.json");
        RecipeMediaEndpoint endpoint = new(VALID_ID);

        RecipeMedia? result = endpoint.Parse(JsonSerializer.Deserialize<JsonElement>(json));

        Assert.NotNull(result);
        Assert.Equal(VALID_ID, result.Id);
        Assert.Equal("https://render.worldofwarcraft.com/us/icons/56/inv_stone_sharpeningstone_01.jpg", result.URL);
        Assert.Equal(ETLStateType.COMPLETE, result.Status);
    }
}
