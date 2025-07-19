using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ETL.BlizzardAPI.Endpoints;

public class RecipeIndexEndpointTests
{
    [Fact]
    public void ItShouldParseRecipeIndexData()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/ProfessionSkillTier.json");
        JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

        RecipeIndexEndpoint endpoint = new(164, 2477);
        List<Recipe> result = endpoint.Parse(jsonElement);

        Assert.NotNull(result);
        Assert.True(result.Count > 0);

        // Test a few specific recipes from the fixture
        Recipe roughSharpeningStone = result.Find(r => r.Id == 1631)!;
        Assert.Equal(1631, roughSharpeningStone.Id);
        Assert.Equal("Rough Sharpening Stone", roughSharpeningStone.Name);
        Assert.Equal(164, roughSharpeningStone.ProfessionId);
        Assert.Equal(2477, roughSharpeningStone.SkillTierId);

        Recipe copperChainBelt = result.Find(r => r.Id == 1632)!;
        Assert.Equal(1632, copperChainBelt.Id);
        Assert.Equal("Copper Chain Belt", copperChainBelt.Name);
        Assert.Equal(164, copperChainBelt.ProfessionId);
        Assert.Equal(2477, copperChainBelt.SkillTierId);

        // Verify all recipes have profession and skill tier IDs set correctly
        foreach (Recipe recipe in result)
        {
            Assert.Equal(164, recipe.ProfessionId);
            Assert.Equal(2477, recipe.SkillTierId);
            Assert.True(recipe.Id > 0);
            Assert.False(string.IsNullOrEmpty(recipe.Name));
        }
    }
}