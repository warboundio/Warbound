using System;
using System.Collections.Generic;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;

namespace Data.BlizzardAPI.Endpoints;

public class RecipeIndexEndpoint : BaseBlizzardEndpoint<List<Recipe>>
{
    private readonly int _professionId;
    private readonly int _skillTierId;

    public RecipeIndexEndpoint(int professionId, int skillTierId)
    {
        _professionId = professionId;
        _skillTierId = skillTierId;
    }

    public override string BuildUrl() => $"https://us.api.blizzard.com/data/wow/profession/{_professionId}/skill-tier/{_skillTierId}?namespace=static-us&locale=en_US";

    public override List<Recipe> Parse(JsonElement json)
    {
        List<Recipe> recipes = [];

        if (json.TryGetProperty("categories", out JsonElement categories))
        {
            foreach (JsonElement category in categories.EnumerateArray())
            {
                if (category.TryGetProperty("recipes", out JsonElement recipesArray))
                {
                    foreach (JsonElement recipeElement in recipesArray.EnumerateArray())
                    {
                        Recipe recipe = new();
                        recipe.Id = recipeElement.GetProperty("id").GetInt32();
                        recipe.Name = recipeElement.GetProperty("name").GetString()!;
                        recipe.ProfessionId = _professionId;
                        recipe.SkillTierId = _skillTierId;
                        recipe.Status = ETLStateType.NEEDS_ENRICHED;
                        recipe.LastUpdatedUtc = DateTime.UtcNow;

                        recipes.Add(recipe);
                    }
                }
            }
        }

        return recipes;
    }
}
