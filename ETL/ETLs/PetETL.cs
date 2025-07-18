using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using ETL.BlizzardAPI.Endpoints;
using ETL.BlizzardAPI.Enums;
using Microsoft.EntityFrameworkCore;

namespace ETL.ETLs;

public class PetETL : RunnableBlizzardETL
{

    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<PetETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        List<Pet> petsToProcess = await Context.Pets.Where(x => x.Status == ETLStateType.NEEDS_ENRICHED).ToListAsync();
        return [.. petsToProcess.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        Pet pet = (Pet)item;

        PetEndpoint endpoint = new(pet.Id);
        Pet enriched = await endpoint.GetAsync();

        pet.BattlePetType = enriched.BattlePetType;
        pet.IsCapturable = enriched.IsCapturable;
        pet.IsTradable = enriched.IsTradable;
        pet.IsBattlePet = enriched.IsBattlePet;
        pet.IsAllianceOnly = enriched.IsAllianceOnly;
        pet.IsHordeOnly = enriched.IsHordeOnly;
        pet.SourceType = enriched.SourceType;
        pet.Icon = enriched.Icon;
        pet.Status = ETLStateType.COMPLETE;
        pet.LastUpdatedUtc = DateTime.UtcNow;

        SaveBuffer.Add(pet);
    }
}
