using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Endpoints;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.ETLs;

public class MountIndexETL : RunnableBlizzardETL
{
    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<MountIndexETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        HashSet<int> existingIds = await Context.Mounts.Select(m => m.Id).ToHashSetAsync();

        MountIndexEndpoint endpoint = new();
        List<Mount> mountsFromApi = await endpoint.GetAsync();
        List<Mount> newMounts = [.. mountsFromApi.Where(mount => !existingIds.Contains(mount.Id))];

        return [.. newMounts.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        await Task.Run(() =>
        {
            Mount mount = (Mount)item;

            SaveBuffer.Add(new Mount
            {
                Id = mount.Id,
                Name = mount.Name,
                Status = ETLStateType.NEEDS_ENRICHED,
                LastUpdatedUtc = DateTime.UtcNow
            });
        });
    }
}
