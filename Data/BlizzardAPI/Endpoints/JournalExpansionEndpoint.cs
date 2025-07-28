using System;
using System.Linq;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;

namespace Data.BlizzardAPI.Endpoints;

public class JournalExpansionEndpoint : BaseBlizzardEndpoint<JournalExpansion>
{
    public int ExpansionId { get; private set; }

    public JournalExpansionEndpoint(int expansionId) { ExpansionId = expansionId; }

    public override string BuildUrl() => $"https://us.api.blizzard.com/data/wow/journal-expansion/{ExpansionId}?namespace=static-us&locale=en_US";

    public override JournalExpansion Parse(JsonElement json)
    {
        int id = json.GetProperty("id").GetInt32();
        string name = json.GetProperty("name").GetString()!;

        string dungeonIds = string.Empty;
        if (json.TryGetProperty("dungeons", out JsonElement dungeonsElement))
        {
            int[] dungeonIdArray = [.. dungeonsElement.EnumerateArray().Select(d => d.GetProperty("id").GetInt32())];
            dungeonIds = string.Join(";", dungeonIdArray);
        }

        string raidIds = string.Empty;
        if (json.TryGetProperty("raids", out JsonElement raidsElement))
        {
            int[] raidIdArray = [.. raidsElement.EnumerateArray().Select(r => r.GetProperty("id").GetInt32())];
            raidIds = string.Join(";", raidIdArray);
        }

        JournalExpansion expansion = new();
        expansion.Id = id;
        expansion.Name = name;
        expansion.DungeonIds = dungeonIds;
        expansion.RaidIds = raidIds;
        expansion.Status = ETLStateType.COMPLETE;
        expansion.LastUpdatedUtc = DateTime.UtcNow;

        return expansion;
    }
}
