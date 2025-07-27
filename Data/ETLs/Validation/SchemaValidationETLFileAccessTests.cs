using System.IO;
using System.Text.Json;

namespace Data.ETLs.Validation;

public class SchemaValidationETLFileAccessTests
{
    [Fact]
    public void ItShouldReadItemFixtureFile()
    {
        string fixturePath = Path.Combine("BlizzardAPI", "Endpoints", "Json", "Item.json");

        Assert.True(File.Exists(fixturePath), $"Fixture file should exist at {fixturePath}");

        string content = File.ReadAllText(fixturePath);
        Assert.False(string.IsNullOrEmpty(content), "Fixture file should not be empty");

        JsonElement element = JsonSerializer.Deserialize<JsonElement>(content);
        Assert.True(element.TryGetProperty("id", out JsonElement idElement), "Fixture should have 'id' property");
        Assert.Equal(19019, idElement.GetInt32());
    }

    [Fact]
    public void ItShouldReadAllTargetFixtureFiles()
    {
        string[] fixtures = ["Item.json", "Mount.json", "Pet.json", "Toy.json"];
        int[] expectedIds = [19019, 35, 39, 1131];

        for (int i = 0; i < fixtures.Length; i++)
        {
            string fixturePath = Path.Combine("BlizzardAPI", "Endpoints", "Json", fixtures[i]);
            Assert.True(File.Exists(fixturePath), $"Fixture file should exist: {fixturePath}");

            string content = File.ReadAllText(fixturePath);
            JsonElement element = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.True(element.TryGetProperty("id", out JsonElement idElement), $"Fixture {fixtures[i]} should have 'id' property");
            Assert.Equal(expectedIds[i], idElement.GetInt32());
        }
    }
}
