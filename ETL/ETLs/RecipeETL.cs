using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using ETL.BlizzardAPI.Endpoints;
using ETL.BlizzardAPI.Enums;
using Microsoft.EntityFrameworkCore;

namespace ETL.ETLs;

public class RecipeETL : RunnableBlizzardETL
{
    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<RecipeETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        List<Recipe> recipesToProcess = await Context.Recipes.Where(x => x.Status == ETLStateType.NEEDS_ENRICHED).ToListAsync();
        return [.. recipesToProcess.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        Recipe recipe = (Recipe)item;
        RecipeEndpoint endpoint = new(recipe.Id);
        Recipe enriched = await endpoint.GetAsync();

        recipe.Name = enriched.Name;
        recipe.CraftedItemId = enriched.CraftedItemId;
        recipe.CraftedQuantity = enriched.CraftedQuantity;
        recipe.Reagents = enriched.Reagents;
        recipe.Status = ETLStateType.COMPLETE;
        recipe.LastUpdatedUtc = System.DateTime.UtcNow;

        SaveBuffer.Add(recipe);
    }
}