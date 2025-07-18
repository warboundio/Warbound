using System.IO;
using System.Text.Json;
using Xunit;

namespace ETL.ETLs.Validation;

public class SchemaValidationETLEndToEndTests
{
    [Fact]
    public void ItShouldCompareItemFixtureWithItself()
    {
        // Arrange
        string fixturePath = Path.Combine("BlizzardAPI", "Endpoints", "Item.json");
        string fixtureJson = File.ReadAllText(fixturePath);
        JsonElement fixtureElement = JsonSerializer.Deserialize<JsonElement>(fixtureJson);
        
        SchemaValidationETL etl = new();
        
        // Act - Compare fixture with itself (should be identical)
        var differences = etl.CompareSchemas(fixtureElement, fixtureElement, "");
        
        // Assert
        Assert.Empty(differences);
    }

    [Fact]
    public void ItShouldDetectDifferencesInModifiedItemFixture()
    {
        // Arrange
        string fixturePath = Path.Combine("BlizzardAPI", "Endpoints", "Item.json");
        string fixtureJson = File.ReadAllText(fixturePath);
        JsonElement fixtureElement = JsonSerializer.Deserialize<JsonElement>(fixtureJson);
        
        // Create a modified version missing the "level" property
        string modifiedJson = """
        {
          "id": 19019,
          "name": "Thunderfury, Blessed Blade of the Windseeker",
          "quality": {
            "type": "LEGENDARY",
            "name": "Legendary"
          }
        }
        """;
        JsonElement modifiedElement = JsonSerializer.Deserialize<JsonElement>(modifiedJson);
        
        SchemaValidationETL etl = new();
        
        // Act
        var differences = etl.CompareSchemas(fixtureElement, modifiedElement, "");
        
        // Assert
        Assert.NotEmpty(differences);
        Assert.Contains(differences, d => d.Contains("Property missing in live API"));
    }

    [Fact]
    public void ItShouldValidateAllTargetEndpointUrls()
    {
        // Arrange
        SchemaValidationETL etl = new();
        
        // Act & Assert - Verify all target endpoints can generate URLs
        string itemUrl = etl.GetEndpointUrl("Item", 19019);
        string mountUrl = etl.GetEndpointUrl("Mount", 35);
        string petUrl = etl.GetEndpointUrl("Pet", 39);
        string toyUrl = etl.GetEndpointUrl("Toy", 1131);
        
        Assert.Contains("item/19019", itemUrl);
        Assert.Contains("mount/35", mountUrl);
        Assert.Contains("pet/39", petUrl);
        Assert.Contains("toy/1131", toyUrl);
        
        // All URLs should be HTTPS and use the same base pattern
        Assert.All(new[] { itemUrl, mountUrl, petUrl, toyUrl }, url =>
        {
            Assert.StartsWith("https://us.api.blizzard.com/data/wow/", url);
            Assert.Contains("namespace=static-us", url);
            Assert.Contains("locale=en_US", url);
        });
    }
}