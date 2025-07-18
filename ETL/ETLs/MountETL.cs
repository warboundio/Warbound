using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using ETL.BlizzardAPI.Endpoints;
using ETL.BlizzardAPI.Enums;
using Microsoft.EntityFrameworkCore;

namespace ETL.ETLs;

public class MountETL : RunnableBlizzardETL
{

    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<MountETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        List<Mount> mountsToProcess = await Context.Mounts.Where(x => x.Status == ETLStateType.NEEDS_ENRICHED).ToListAsync();
        return [.. mountsToProcess.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        Mount mount = (Mount)item;

        MountEndpoint endpoint = new(mount.Id);
        Mount enriched = await endpoint.GetAsync();

        mount.SourceType = enriched.SourceType;
        mount.CreatureDisplayId = enriched.CreatureDisplayId;
        mount.Status = ETLStateType.COMPLETE;
        mount.LastUpdatedUtc = DateTime.UtcNow;

        SaveBuffer.Add(mount);
    }
}
