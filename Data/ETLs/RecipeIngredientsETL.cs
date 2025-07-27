using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.ETLs;

public class RecipeIngredientsETL : RunnableBlizzardETL
{
    private List<int> _newItemIds = [];

    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<RecipeIngredientsETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        HashSet<int> existingItemIds = await Context.Items.Select(i => i.Id).ToHashSetAsync();
        List<Recipe> completeRecipes = await Context.Recipes.Where(r => r.Status == ETLStateType.COMPLETE).ToListAsync();
        HashSet<int> discoveredItemIds = [];

        foreach (Recipe recipe in completeRecipes)
        {
            if (string.IsNullOrEmpty(recipe.Reagents)) continue;

            string[] reagentPairs = recipe.Reagents.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (string reagentPair in reagentPairs)
            {
                string[] parts = reagentPair.Split(':', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 1 && int.TryParse(parts[0], out int itemId))
                {
                    discoveredItemIds.Add(itemId);
                }
            }
        }

        _newItemIds = [.. discoveredItemIds.Where(id => !existingItemIds.Contains(id))];
        return [.. _newItemIds.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        await Task.Run(() =>
        {
            int id = (int)item;

            SaveBuffer.Add(new Item
            {
                Id = id,
                Status = ETLStateType.NEEDS_ENRICHED,
                LastUpdatedUtc = DateTime.UtcNow
            });
        });
    }
}