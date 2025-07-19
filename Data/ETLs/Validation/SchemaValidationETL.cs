using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Logs;
using Data.BlizzardAPI.Endpoints;
using Data.BlizzardAPI.General;

namespace Data.ETLs.Validation;

public class SchemaValidationETL
{
    public static async Task RunAsync()
    {
        SchemaValidationETL etl = new();
        await etl.ProcessAsync();
    }

    private async Task ProcessAsync()
    {
        Logging.Info(GetType().Name, "Starting schema validation for Blizzard API endpoints.");

        List<(string FixtureFile, int Id, string EndpointType)> targets = [
            ("Item.json", 19019, "Item"),
            ("Mount.json", 35, "Mount"),
            ("Pet.json", 39, "Pet"),
            ("Toy.json", 1131, "Toy")
        ];

        foreach ((string fixtureFile, int id, string endpointType) in targets)
        {
            try
            {
                await ValidateEndpointSchemaAsync(fixtureFile, id, endpointType);
            }
            catch (Exception ex)
            {
                Logging.Error(GetType().Name, $"Failed to validate schema for {fixtureFile}", ex);
            }
        }

        Logging.Info(GetType().Name, "Schema validation completed.");
    }

    private async Task ValidateEndpointSchemaAsync(string fixtureFile, int id, string endpointType)
    {
        string fixturePath = Path.Combine("BlizzardAPI", "Endpoints", fixtureFile);
        if (!File.Exists(fixturePath))
        {
            Logging.Warn(GetType().Name, $"Fixture file not found: {fixturePath}");
            return;
        }

        string fixtureJson = await File.ReadAllTextAsync(fixturePath);
        JsonElement fixtureElement = JsonSerializer.Deserialize<JsonElement>(fixtureJson);
        string liveUrl = GetEndpointUrl(endpointType, id);
        Logging.Info(GetType().Name, $"Validating schema for {fixtureFile} against {liveUrl}");

        string liveJson = await BlizzardAPIRouter.GetJsonRawAsync(liveUrl, forceLiveCall: true);
        JsonElement liveElement = JsonSerializer.Deserialize<JsonElement>(liveJson);
        List<string> differences = CompareSchemas(fixtureElement, liveElement, "");

        if (differences.Count == 0) { Logging.Info(GetType().Name, $"Schema validation PASSED for {fixtureFile} - schemas match"); }
        else { LogSchemaDifferences(fixtureFile, liveUrl, differences); }
    }

    public string GetEndpointUrl(string endpointType, int id)
    {
        return endpointType switch
        {
            "Item" => new ItemEndpoint(id).BuildUrl(),
            "Mount" => new MountEndpoint(id).BuildUrl(),
            "Pet" => new PetEndpoint(id).BuildUrl(),
            "Toy" => new ToyEndpoint(id).BuildUrl(),
            _ => throw new ArgumentException($"Unknown endpoint type: {endpointType}")
        };
    }

    public List<string> CompareSchemas(JsonElement fixture, JsonElement live, string path)
    {
        List<string> differences = [];

        if (fixture.ValueKind != live.ValueKind)
        {
            differences.Add($"Type mismatch at {path}: fixture={fixture.ValueKind}, live={live.ValueKind}");
            return differences;
        }

        if (fixture.ValueKind == JsonValueKind.Object) { CompareObjectSchemas(fixture, live, path, differences); }
        else if (fixture.ValueKind == JsonValueKind.Array) { CompareArraySchemas(fixture, live, path, differences); }

        return differences;
    }

    private void CompareObjectSchemas(JsonElement fixture, JsonElement live, string path, List<string> differences)
    {
        // Get all property names from both objects
        HashSet<string> fixtureProps = [];
        HashSet<string> liveProps = [];

        foreach (JsonProperty prop in fixture.EnumerateObject()) { fixtureProps.Add(prop.Name); }
        foreach (JsonProperty prop in live.EnumerateObject()) { liveProps.Add(prop.Name); }

        // Check for properties missing in live API
        foreach (string prop in fixtureProps)
        {
            if (!liveProps.Contains(prop))
            {
                differences.Add($"Property missing in live API at {(string.IsNullOrEmpty(path) ? prop : $"{path}.{prop}")}");
            }
        }

        // Check for new properties in live API
        foreach (string prop in liveProps)
        {
            if (!fixtureProps.Contains(prop))
            {
                differences.Add($"New property in live API at {(string.IsNullOrEmpty(path) ? prop : $"{path}.{prop}")}");
            }
        }

        // Recursively compare common properties
        foreach (string prop in fixtureProps)
        {
            if (liveProps.Contains(prop))
            {
                JsonElement fixtureValue = fixture.GetProperty(prop);
                JsonElement liveValue = live.GetProperty(prop);
                string childPath = string.IsNullOrEmpty(path) ? prop : $"{path}.{prop}";
                differences.AddRange(CompareSchemas(fixtureValue, liveValue, childPath));
            }
        }
    }

    private void CompareArraySchemas(JsonElement fixture, JsonElement live, string path, List<string> differences)
    {
        // For arrays, we compare the schema of the first element if both arrays have elements
        if (fixture.GetArrayLength() > 0 && live.GetArrayLength() > 0)
        {
            JsonElement fixtureFirst = fixture[0];
            JsonElement liveFirst = live[0];
            differences.AddRange(CompareSchemas(fixtureFirst, liveFirst, $"{path}[0]"));
        }
        else if (fixture.GetArrayLength() == 0 && live.GetArrayLength() > 0)
        {
            differences.Add($"Array at {path} was empty in fixture but has elements in live API");
        }
        else if (fixture.GetArrayLength() > 0 && live.GetArrayLength() == 0)
        {
            differences.Add($"Array at {path} had elements in fixture but is empty in live API");
        }
    }

    private void LogSchemaDifferences(string fixtureFile, string url, List<string> differences)
    {
        // Categorize differences by severity
        List<string> errors = [];
        List<string> warnings = [];

        foreach (string diff in differences)
        {
            if (diff.Contains("missing in live API") || diff.Contains("Type mismatch"))
            {
                errors.Add(diff);
            }
            else
            {
                warnings.Add(diff);
            }
        }

        if (errors.Count > 0)
        {
            Logging.Warn(GetType().Name, $"Schema validation FAILED for {fixtureFile} ({url}) - {errors.Count} critical differences: {string.Join("; ", errors)}");
        }

        if (warnings.Count > 0)
        {
            Logging.Warn(GetType().Name, $"Schema validation WARNING for {fixtureFile} ({url}) - {warnings.Count} minor differences: {string.Join("; ", warnings)}");
        }
    }
}
