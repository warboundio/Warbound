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

        List<(string FixtureFile, object? Parameter, string EndpointType)> targets = [
            ("Achievement.json", 9713, "Achievement"),
            ("AchievementIndex.json", null, "AchievementIndex"),
            ("AchievementMedia.json", 6, "AchievementMedia"),
            ("Item.json", 19019, "Item"),
            ("ItemAppearance.json", 321, "ItemAppearance"),
            ("ItemMedia.json", 19019, "ItemMedia"),
            ("JournalEncounter.json", 89, "JournalEncounter"),
            ("JournalExpansion.json", 68, "JournalExpansion"),
            ("JournalInstanceMedia.json", 68, "JournalInstanceMedia"),
            ("Mount.json", 35, "Mount"),
            ("Pet.json", 39, "Pet"),
            ("Profession.json", 164, "Profession"),
            ("ProfessionIndex.json", null, "ProfessionIndex"),
            ("ProfessionMedia.json", 164, "ProfessionMedia"),
            ("QuestAreaIndex.json", null, "QuestAreaIndex"),
            ("QuestTypeIndex.json", null, "QuestTypeIndex"),
            ("Realm.json", "tichondrius", "Realm"),
            ("RealmIndex.json", null, "RealmIndex"),
            ("Recipe.json", 1631, "Recipe"),
            ("RecipeMedia.json", 1631, "RecipeMedia"),
            ("Toy.json", 1131, "Toy"),
            ("ToyIndex.json", null, "ToyIndex")
        ];

        foreach ((string fixtureFile, object? parameter, string endpointType) in targets)
        {
            try
            {
                await ValidateEndpointSchemaAsync(fixtureFile, parameter, endpointType);
            }
            catch (Exception ex)
            {
                Logging.Error(GetType().Name, $"Failed to validate schema for {fixtureFile}", ex);
            }
        }

        Logging.Info(GetType().Name, "Schema validation completed.");
    }

    private async Task ValidateEndpointSchemaAsync(string fixtureFile, object? parameter, string endpointType)
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string fixturePath = Path.Combine(currentDirectory, "bin", "debug", "net8.0", "BlizzardAPI", "Endpoints", fixtureFile);
        if (!File.Exists(fixturePath))
        {
            Logging.Warn(GetType().Name, $"Fixture file not found: {fixturePath}");
            return;
        }

        string fixtureJson = await File.ReadAllTextAsync(fixturePath);
        JsonElement fixtureElement = JsonSerializer.Deserialize<JsonElement>(fixtureJson);
        string liveUrl = GetEndpointUrl(endpointType, parameter);
        Logging.Info(GetType().Name, $"Validating schema for {fixtureFile} against {liveUrl}");

        string liveJson = await BlizzardAPIRouter.GetJsonRawAsync(liveUrl, forceLiveCall: true);
        JsonElement liveElement = JsonSerializer.Deserialize<JsonElement>(liveJson);
        List<string> differences = CompareSchemas(fixtureElement, liveElement, "");

        if (differences.Count == 0) { Logging.Info(GetType().Name, $"Schema validation PASSED for {fixtureFile} - schemas match"); }
        else { LogSchemaDifferences(fixtureFile, liveUrl, differences); }
    }

    public string GetEndpointUrl(string endpointType, object? parameter)
    {
        return endpointType switch
        {
            "Achievement" => new AchievementEndpoint((int)parameter!).BuildUrl(),
            "AchievementIndex" => new AchievementIndexEndpoint().BuildUrl(),
            "AchievementMedia" => new AchievementMediaEndpoint((int)parameter!).BuildUrl(),
            "Item" => new ItemEndpoint((int)parameter!).BuildUrl(),
            "ItemAppearance" => new ItemAppearanceEndpoint((int)parameter!).BuildUrl(),
            "ItemMedia" => new ItemMediaEndpoint((int)parameter!).BuildUrl(),
            "JournalEncounter" => new JournalEncounterEndpoint((int)parameter!).BuildUrl(),
            "JournalExpansion" => new JournalExpansionEndpoint((int)parameter!).BuildUrl(),
            "JournalInstanceMedia" => new JournalInstanceMediaEndpoint((int)parameter!).BuildUrl(),
            "Mount" => new MountEndpoint((int)parameter!).BuildUrl(),
            "Pet" => new PetEndpoint((int)parameter!).BuildUrl(),
            "Profession" => new ProfessionEndpoint((int)parameter!).BuildUrl(),
            "ProfessionIndex" => new ProfessionIndexEndpoint().BuildUrl(),
            "ProfessionMedia" => new ProfessionMediaEndpoint((int)parameter!).BuildUrl(),
            "QuestAreaIndex" => new QuestAreaIndexEndpoint().BuildUrl(),
            "QuestTypeIndex" => new QuestTypeIndexEndpoint().BuildUrl(),
            "Realm" => new RealmEndpoint((string)parameter!).BuildUrl(),
            "RealmIndex" => new RealmIndexEndpoint().BuildUrl(),
            "Recipe" => new RecipeEndpoint((int)parameter!).BuildUrl(),
            "RecipeMedia" => new RecipeMediaEndpoint((int)parameter!).BuildUrl(),
            "Toy" => new ToyEndpoint((int)parameter!).BuildUrl(),
            "ToyIndex" => new ToyIndexEndpoint().BuildUrl(),
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
        HashSet<string> fixtureProps = [];
        HashSet<string> liveProps = [];

        foreach (JsonProperty prop in fixture.EnumerateObject()) { fixtureProps.Add(prop.Name); }
        foreach (JsonProperty prop in live.EnumerateObject()) { liveProps.Add(prop.Name); }

        foreach (string prop in fixtureProps)
        {
            if (!liveProps.Contains(prop))
            {
                differences.Add($"Property missing in live API at {(string.IsNullOrEmpty(path) ? prop : $"{path}.{prop}")}");
            }
        }

        foreach (string prop in liveProps)
        {
            if (!fixtureProps.Contains(prop))
            {
                differences.Add($"New property in live API at {(string.IsNullOrEmpty(path) ? prop : $"{path}.{prop}")}");
            }
        }

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
