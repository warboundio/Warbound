using System;
using System.Collections.Generic;
using System.Linq;
using Data.BlizzardAPI;
using Data.BlizzardAPI.Models;

namespace Data.Mappings;

public class RecipeExpansionMappings
{
    public void MapAndPersist()
    {
        using BlizzardAPIContext context = new();

        Dictionary<int, ObjectExpansionMapping> itemObjectExpansionMappings = GetObjectExpansionMappings(context, 'I');
        Dictionary<int, ObjectExpansionMapping> recipeObjectExpansionMappings = GetObjectExpansionMappings(context, 'R');
        List<Recipe> recipes = GetAllRecipes(context);

        int countAdded = 0;
        foreach (Recipe recipe in recipes)
        {
            if (recipe.CraftedItemId <= 0) { continue; }

            if (itemObjectExpansionMappings.TryGetValue(recipe.CraftedItemId, out ObjectExpansionMapping? craftedItemMapping) &&
                craftedItemMapping.ExpansionId != -1)
            {
                // Only add if not already present
                if (!recipeObjectExpansionMappings.ContainsKey(recipe.Id))
                {
                    ObjectExpansionMapping mapping = new()
                    {
                        Id = recipe.Id,
                        CollectionType = 'R',
                        ExpansionId = craftedItemMapping.ExpansionId
                    };
                    context.ObjectExpansionMappings.Add(mapping);
                    recipeObjectExpansionMappings[recipe.Id] = mapping;
                    countAdded++;
                }
            }
        }

        context.SaveChanges();
    }

    private Dictionary<int, ObjectExpansionMapping> GetObjectExpansionMappings(BlizzardAPIContext context, char collectionType)
    {
        return context.ObjectExpansionMappings
            .Where(x => x.CollectionType == collectionType)
            .ToDictionary(x => x.Id);
    }

    private List<Recipe> GetAllRecipes(BlizzardAPIContext context) => [.. context.Recipes];
}
