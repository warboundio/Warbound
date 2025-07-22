using System;
using System.Linq;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;

namespace Data.BlizzardAPI.Endpoints;

public class JournalEncounterEndpoint : BaseBlizzardEndpoint<JournalEncounter>
{
    public int EncounterId { get; private set; }

    public JournalEncounterEndpoint(int encounterId) { EncounterId = encounterId; }

    public override string BuildUrl() => $"https://us.api.blizzard.com/data/wow/journal-encounter/{EncounterId}?namespace=static-us&locale=en_US";

    public override JournalEncounter Parse(JsonElement json)
    {
        int id = json.GetProperty("id").GetInt32();
        string name = json.GetProperty("name").GetString()!;

        string items = string.Empty;
        if (json.TryGetProperty("items", out JsonElement itemsElement))
        {
            string[] itemIds = itemsElement.EnumerateArray()
                .Where(item => item.TryGetProperty("item", out JsonElement itemElement) && itemElement.TryGetProperty("id", out _))
                .Select(item => item.GetProperty("item").GetProperty("id").GetInt32().ToString())
                .ToArray();
            items = string.Join(";", itemIds);
        }

        string instanceName = string.Empty;
        int instanceId = -1;
        if (json.TryGetProperty("instance", out JsonElement instanceElement))
        {
            instanceName = instanceElement.TryGetProperty("name", out JsonElement nameElement) ? nameElement.GetString() ?? string.Empty : string.Empty;
            instanceId = instanceElement.TryGetProperty("id", out JsonElement idElement) ? idElement.GetInt32() : -1;
        }

        string categoryType = string.Empty;
        if (json.TryGetProperty("category", out JsonElement categoryElement) && categoryElement.TryGetProperty("type", out JsonElement typeElement))
        {
            categoryType = typeElement.GetString() ?? string.Empty;
        }

        string modesTypes = string.Empty;
        if (json.TryGetProperty("modes", out JsonElement modesElement))
        {
            string[] modeTypes = modesElement.EnumerateArray()
                .Where(mode => mode.TryGetProperty("type", out _))
                .Select(mode => mode.GetProperty("type").GetString() ?? string.Empty)
                .Where(type => !string.IsNullOrEmpty(type))
                .ToArray();
            modesTypes = string.Join(";", modeTypes);
        }

        JournalEncounter encounter = new();
        encounter.Id = id;
        encounter.Name = name;
        encounter.Items = items;
        encounter.InstanceName = instanceName;
        encounter.InstanceId = instanceId;
        encounter.CategoryType = categoryType;
        encounter.ModesTypes = modesTypes;
        encounter.Status = ETLStateType.COMPLETE;
        encounter.LastUpdatedUtc = DateTime.UtcNow;

        return encounter;
    }
}
