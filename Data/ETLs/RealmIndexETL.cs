using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace Data.ETLs;

public class RealmIndexETL : RunnableBlizzardETL
{
    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<RealmIndexETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        RealmIndexEndpoint endpoint = new();
        List<Realm> indexRealms = await endpoint.GetAsync();

        List<int> existingIds = await Context.Realms.Select(r => r.Id).ToListAsync();

        List<Realm> newRealms = [.. indexRealms.Where(r => !existingIds.Contains(r.Id))];
        return [.. newRealms.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        await Task.Run(() =>
        {
            Realm realm = (Realm)item;
            SaveBuffer.Add(realm);
        });
    }
}
