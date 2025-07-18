using System;
using System.Collections.Generic;
using System.Text.Json;
using ETL.BlizzardAPI.Enums;
using ETL.BlizzardAPI.General;

namespace ETL.BlizzardAPI.Endpoints;

public class MountIndexEndpoint : BaseBlizzardEndpoint<List<Mount>>
{
    public override string BuildUrl() => "https://us.api.blizzard.com/data/wow/mount/?namespace=static-us&locale=en_US";

    public override List<Mount> Parse(JsonElement json)
    {
        List<Mount> mounts = [];
        foreach (JsonElement mountElement in json.GetProperty("mounts").EnumerateArray())
        {
            Mount mountObj = new();
            mountObj.Id = mountElement.GetProperty("id").GetInt32();
            mountObj.Name = mountElement.GetProperty("name").GetString()!;
            mountObj.Status = ETLStateType.NEEDS_ENRICHED;
            mountObj.LastUpdatedUtc = DateTime.UtcNow;

            mounts.Add(mountObj);
        }
        return mounts;
    }
}
