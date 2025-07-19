using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace Data.ETLs;

public class ProfessionMediaETL : RunnableBlizzardETL
{
    private List<Profession> _professionsToProcess = [];

    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<ProfessionMediaETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        List<int> existingProfessionMediaIds = await Context.ProfessionMedias.Select(x => x.Id).ToListAsync();
        _professionsToProcess = await Context.Professions.Where(x => !existingProfessionMediaIds.Contains(x.Id)).ToListAsync();
        return [.. _professionsToProcess.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        Profession casted = (Profession)item;

        ProfessionMediaEndpoint endpoint = new(casted.Id);
        ProfessionMedia result = await endpoint.GetAsync();

        SaveBuffer.Add(result);
    }
}