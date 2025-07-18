using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using ETL.BlizzardAPI.Endpoints;
using ETL.BlizzardAPI.Enums;
using Microsoft.EntityFrameworkCore;

namespace ETL.ETLs;

public class RealmETL : RunnableBlizzardETL
{
    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<RealmETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        List<Realm> realmsToProcess = await Context.Realms.Where(r => r.Status == ETLStateType.NEEDS_ENRICHED).ToListAsync();
        return [.. realmsToProcess.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        Realm realm = (Realm)item;

        RealmEndpoint endpoint = new(realm.Slug);
        Realm enriched = await endpoint.GetAsync();

        realm.Category = enriched.Category;
        realm.Locale = enriched.Locale;
        realm.Timezone = enriched.Timezone;
        realm.Type = enriched.Type;
        realm.IsTournament = enriched.IsTournament;
        realm.Region = enriched.Region;
        realm.Status = ETLStateType.COMPLETE;
        realm.LastUpdatedUtc = DateTime.UtcNow;

        SaveBuffer.Add(realm);
    }
}
