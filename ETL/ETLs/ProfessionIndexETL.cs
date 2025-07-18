using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using ETL.BlizzardAPI.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace ETL.ETLs;

public class ProfessionIndexETL : RunnableBlizzardETL
{
    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<ProfessionIndexETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        HashSet<int> existingIds = await Context.Professions.Select(p => p.Id).ToHashSetAsync();

        ProfessionIndexEndpoint endpoint = new();
        List<Profession> professionData = await endpoint.GetAsync();

        List<Profession> newProfessions = [.. professionData.Where(profession => !existingIds.Contains(profession.Id))];
        return [.. newProfessions.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        await Task.Run(() =>
        {
            Profession profession = (Profession)item;
            SaveBuffer.Add(profession);
        });
    }
}