using System.Collections.Generic;
using System.Text.Json;

namespace ETL.ETLs.Validation;

public class SchemaValidationETLTests
{
    [Fact]
    public void ItShouldDetectIdenticalSchemas()
    {
        // Arrange
        string json1 = """{"id": 123, "name": "test", "active": true}""";
        string json2 = """{"id": 456, "name": "other", "active": true}"""; // Same types, different values

        JsonElement element1 = JsonSerializer.Deserialize<JsonElement>(json1);
        JsonElement element2 = JsonSerializer.Deserialize<JsonElement>(json2);

        SchemaValidationETL etl = new();

        // Act
        List<string> differences = etl.CompareSchemas(element1, element2, "");

        // Assert
        Assert.Empty(differences);
    }

    [Fact]
    public void ItShouldDetectMissingProperties()
    {
        // Arrange
        string json1 = """{"id": 123, "name": "test", "active": true}""";
        string json2 = """{"id": 456, "name": "other"}""";

        JsonElement element1 = JsonSerializer.Deserialize<JsonElement>(json1);
        JsonElement element2 = JsonSerializer.Deserialize<JsonElement>(json2);

        SchemaValidationETL etl = new();

        // Act
        List<string> differences = etl.CompareSchemas(element1, element2, "");

        // Assert
        Assert.Single(differences);
        Assert.Contains("Property missing in live API at active", differences[0]);
    }

    [Fact]
    public void ItShouldDetectNewProperties()
    {
        // Arrange
        string json1 = """{"id": 123, "name": "test"}""";
        string json2 = """{"id": 456, "name": "other", "active": true}""";

        JsonElement element1 = JsonSerializer.Deserialize<JsonElement>(json1);
        JsonElement element2 = JsonSerializer.Deserialize<JsonElement>(json2);

        SchemaValidationETL etl = new();

        // Act
        List<string> differences = etl.CompareSchemas(element1, element2, "");

        // Assert
        Assert.Single(differences);
        Assert.Contains("New property in live API at active", differences[0]);
    }

    [Fact]
    public void ItShouldDetectTypeMismatches()
    {
        // Arrange
        string json1 = """{"id": 123, "active": true}""";
        string json2 = """{"id": "456", "active": true}""";

        JsonElement element1 = JsonSerializer.Deserialize<JsonElement>(json1);
        JsonElement element2 = JsonSerializer.Deserialize<JsonElement>(json2);

        SchemaValidationETL etl = new();

        // Act
        List<string> differences = etl.CompareSchemas(element1, element2, "");

        // Assert
        Assert.Single(differences);
        Assert.Contains("Type mismatch at id", differences[0]);
    }

    [Fact]
    public void ItShouldHandleNestedObjects()
    {
        // Arrange
        string json1 = """{"id": 123, "meta": {"version": 1, "author": "test"}}""";
        string json2 = """{"id": 456, "meta": {"version": 2}}""";

        JsonElement element1 = JsonSerializer.Deserialize<JsonElement>(json1);
        JsonElement element2 = JsonSerializer.Deserialize<JsonElement>(json2);

        SchemaValidationETL etl = new();

        // Act
        List<string> differences = etl.CompareSchemas(element1, element2, "");

        // Assert
        Assert.Single(differences);
        Assert.Contains("Property missing in live API at meta.author", differences[0]);
    }

    [Fact]
    public void ItShouldHandleArrays()
    {
        // Arrange
        string json1 = """{"items": [{"id": 1, "name": "test"}]}""";
        string json2 = """{"items": [{"id": 2, "name": "other", "active": true}]}""";

        JsonElement element1 = JsonSerializer.Deserialize<JsonElement>(json1);
        JsonElement element2 = JsonSerializer.Deserialize<JsonElement>(json2);

        SchemaValidationETL etl = new();

        // Act
        List<string> differences = etl.CompareSchemas(element1, element2, "");

        // Assert
        Assert.Single(differences);
        Assert.Contains("New property in live API at items[0].active", differences[0]);
    }
}
