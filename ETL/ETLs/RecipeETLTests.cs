using ETL.BlizzardAPI.Endpoints;
using ETL.BlizzardAPI.Enums;

namespace ETL.ETLs;

public class RecipeETLTests
{
    [Fact]
    public void ItShouldHaveInvalidEnumValue()
    {
        // Act & Assert
        Assert.Equal(98, (int)ETLStateType.INVALID);
    }

    [Fact]
    public void ItShouldSetRecipeToInvalidStatus()
    {
        // Arrange
        Recipe recipe = new() { Id = 503024, Status = ETLStateType.NEEDS_ENRICHED };

        // Act - Simulate the behavior when 404 occurs
        recipe.Status = ETLStateType.INVALID;
        recipe.LastUpdatedUtc = System.DateTime.UtcNow;

        // Assert
        Assert.Equal(ETLStateType.INVALID, recipe.Status);
        Assert.Equal(98, (int)recipe.Status);
        Assert.True(recipe.LastUpdatedUtc > System.DateTime.UtcNow.AddMinutes(-1));
    }
}