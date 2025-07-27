using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Endpoints;
using Data.BlizzardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.ETLs;

public class PetIndexETL : RunnableBlizzardETL
{
    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<PetIndexETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        HashSet<int> existingIds = await Context.Pets.Select(p => p.Id).ToHashSetAsync();

        PetIndexEndpoint endpoint = new();
        List<Pet> petData = await endpoint.GetAsync();

        List<Pet> newPets = [.. petData.Where(pet => !existingIds.Contains(pet.Id))];
        return [.. newPets.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        await Task.Run(() =>
        {
            Pet pet = (Pet)item;
            SaveBuffer.Add(pet);
        });
    }
}
