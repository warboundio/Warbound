using System.IO;
using System.Text.Json;
using Data.BlizzardAPI.Enums;

namespace Data.BlizzardAPI.Endpoints;

public class RecipeEndpointTests
{
    public const int VALID_ID = 1631;

    [Fact]
    public void ItShouldParseRecipeData()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/Recipe.json");
        RecipeEndpoint endpoint = new(VALID_ID);

        Recipe? result = endpoint.Parse(JsonSerializer.Deserialize<JsonElement>(json));

        Assert.NotNull(result);
        Assert.Equal(VALID_ID, result.Id);
        Assert.Equal("Rough Sharpening Stone", result.Name);
        Assert.Equal(2862, result.CraftedItemId);
        Assert.Equal(1, result.CraftedQuantity);
        Assert.Equal("2835:1;", result.Reagents);
        Assert.Equal(ETLStateType.COMPLETE, result.Status);
    }
}