using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Data.ETLs.Validation;

public class SchemaValidationETLEndToEndTests
{
    [Fact]
    public void ItShouldCompareItemFixtureWithItself()
    {
        string fixturePath = Path.Combine("BlizzardAPI", "Endpoints", "Item.json");
        string fixtureJson = File.ReadAllText(fixturePath);
        JsonElement fixtureElement = JsonSerializer.Deserialize<JsonElement>(fixtureJson);

        SchemaValidationETL etl = new();

        // Act - Compare fixture with itself (should be identical)
        List<string> differences = etl.CompareSchemas(fixtureElement, fixtureElement, "");
        Assert.Empty(differences);
    }

    [Fact]
    public void ItShouldDetectDifferencesInModifiedItemFixture()
    {
        string fixturePath = Path.Combine("BlizzardAPI", "Endpoints", "Item.json");
        string fixtureJson = File.ReadAllText(fixturePath);
        JsonElement fixtureElement = JsonSerializer.Deserialize<JsonElement>(fixtureJson);

        // Create a modified version missing the "level" property
        string modifiedJson = /*lang=json,strict*/ """
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
        List<string> differences = etl.CompareSchemas(fixtureElement, modifiedElement, "");

        Assert.NotEmpty(differences);
        Assert.Contains(differences, d => d.Contains("Property missing in live API"));
    }

    [Fact]
    public void ItShouldValidateAllTargetEndpointUrls()
    {
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
    }
}
