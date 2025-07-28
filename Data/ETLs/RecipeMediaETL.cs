using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Endpoints;
using Data.BlizzardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.ETLs;

public class RecipeMediaETL : RunnableBlizzardETL
{
    private List<Recipe> _recipesToProcess = [];

    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<RecipeMediaETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        List<int> existingRecipeMediaIds = await Context.RecipeMedias.Select(x => x.Id).ToListAsync();
        _recipesToProcess = await Context.Recipes.Where(x => !existingRecipeMediaIds.Contains(x.Id)).ToListAsync();
        return [.. _recipesToProcess.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        Recipe casted = (Recipe)item;

        RecipeMediaEndpoint endpoint = new(casted.Id);
        RecipeMedia result = await endpoint.GetAsync();

        SaveBuffer.Add(result);
    }
}
