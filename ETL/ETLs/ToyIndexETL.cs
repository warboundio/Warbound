using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using ETL.BlizzardAPI.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace ETL.ETLs;

public class ToyIndexETL : RunnableBlizzardETL
{
    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<ToyIndexETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        HashSet<int> existingIds = await Context.Toys.Select(t => t.Id).ToHashSetAsync();

        ToyIndexEndpoint endpoint = new();
        List<Toy> toyData = await endpoint.GetAsync();

        List<Toy> newToys = [.. toyData.Where(toy => !existingIds.Contains(toy.Id))];
        return [.. newToys.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        await Task.Run(() =>
        {
            Toy toy = (Toy)item;
            SaveBuffer.Add(toy);
        });
    }
}
