using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using ETL.BlizzardAPI.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace ETL.ETLs;

public class RecipeIndexETL : RunnableBlizzardETL
{
    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<RecipeIndexETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        // Get existing recipe IDs to filter out duplicates
        HashSet<int> existingRecipeIds = await Context.Recipes.Select(r => r.Id).ToHashSetAsync();

        // Get all professions that have skill tiers defined
        List<Profession> professionsWithSkillTiers = await Context.Professions
            .Where(p => !string.IsNullOrEmpty(p.SkillTiers))
            .ToListAsync();

        List<Recipe> newRecipes = [];

        foreach (Profession profession in professionsWithSkillTiers)
        {
            // Parse semicolon-delimited SkillTiers field
            string[] skillTierIds = profession.SkillTiers.Split(';', StringSplitOptions.RemoveEmptyEntries);

            foreach (string skillTierIdStr in skillTierIds)
            {
                if (int.TryParse(skillTierIdStr, out int skillTierId))
                {
                    // Fetch all recipes for this profession/skill tier combination
                    RecipeIndexEndpoint endpoint = new(profession.Id, skillTierId);
                    List<Recipe> recipesFromApi = await endpoint.GetAsync();

                    // Filter to only new recipes that don't already exist
                    List<Recipe> newRecipesForTier = [.. recipesFromApi.Where(recipe => !existingRecipeIds.Contains(recipe.Id))];
                    newRecipes.AddRange(newRecipesForTier);
                }
            }
        }

        return [.. newRecipes.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        await Task.Run(() =>
        {
            Recipe recipe = (Recipe)item;
            SaveBuffer.Add(recipe);
        });
    }
}
