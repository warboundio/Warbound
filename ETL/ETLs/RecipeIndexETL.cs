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
        // Get all professions that have skill tiers defined
        List<Profession> professionsWithSkillTiers = await Context.Professions
            .Where(p => !string.IsNullOrEmpty(p.SkillTiers))
            .ToListAsync();

        List<(int professionId, int skillTierId)> professionSkillTierPairs = [];

        foreach (Profession profession in professionsWithSkillTiers)
        {
            // Parse semicolon-delimited SkillTiers field
            string[] skillTierIds = profession.SkillTiers.Split(';', StringSplitOptions.RemoveEmptyEntries);
            
            foreach (string skillTierIdStr in skillTierIds)
            {
                if (int.TryParse(skillTierIdStr, out int skillTierId))
                {
                    // Check if recipes already exist for this profession/skill tier combination
                    bool recipesExist = await Context.Recipes
                        .AnyAsync(r => r.ProfessionId == profession.Id && r.SkillTierId == skillTierId);
                    
                    if (!recipesExist)
                    {
                        professionSkillTierPairs.Add((profession.Id, skillTierId));
                    }
                }
            }
        }

        return [.. professionSkillTierPairs.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        await Task.Run(async () =>
        {
            (int professionId, int skillTierId) = ((int, int))item;
            
            RecipeIndexEndpoint endpoint = new(professionId, skillTierId);
            List<Recipe> recipes = await endpoint.GetAsync();
            
            foreach (Recipe recipe in recipes)
            {
                SaveBuffer.Add(recipe);
            }
        });
    }
}