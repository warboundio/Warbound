using System;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;

namespace Data.BlizzardAPI.Endpoints;

public class MountEndpoint : BaseBlizzardEndpoint<Mount>
{
    public int MountId { get; private set; }

    public MountEndpoint(int mountId) { MountId = mountId; }

    public override string BuildUrl() => $"https://us.api.blizzard.com/data/wow/mount/{MountId}?namespace=static-us&locale=en_US";

    public override Mount Parse(JsonElement json)
    {
        int id = json.GetProperty("id").GetInt32();
        string name = json.GetProperty("name").GetString()!;

        string sourceType = string.Empty;
        if (json.TryGetProperty("source", out JsonElement sourceElement) && sourceElement.TryGetProperty("type", out JsonElement typeElement))
        {
            sourceType = typeElement.GetString() ?? string.Empty;
        }

        int creatureDisplayId = json.GetProperty("creature_displays")[0].GetProperty("id").GetInt32();

        Mount mountObj = new();
        mountObj.Id = id;
        mountObj.Name = name;
        mountObj.SourceType = sourceType;
        mountObj.CreatureDisplayId = creatureDisplayId;
        mountObj.Status = ETLStateType.COMPLETE;
        mountObj.LastUpdatedUtc = DateTime.UtcNow;

        return mountObj;
    }
}
