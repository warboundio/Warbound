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

public class ProfessionETL : RunnableBlizzardETL
{

    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<ProfessionETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        List<Profession> professionsToProcess = await Context.Professions.Where(x => x.Status == ETLStateType.NEEDS_ENRICHED).ToListAsync();
        return [.. professionsToProcess.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        Profession profession = (Profession)item;

        ProfessionEndpoint endpoint = new(profession.Id);
        Profession enriched = await endpoint.GetAsync();

        profession.Type = enriched.Type;
        profession.SkillTiers = enriched.SkillTiers;
        profession.Status = ETLStateType.COMPLETE;
        profession.LastUpdatedUtc = DateTime.UtcNow;

        SaveBuffer.Add(profession);
    }
}
