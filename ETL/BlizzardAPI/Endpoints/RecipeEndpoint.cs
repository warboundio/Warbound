using System;
using System.Text;
using System.Text.Json;
using ETL.BlizzardAPI.Enums;
using ETL.BlizzardAPI.General;

namespace ETL.BlizzardAPI.Endpoints;

public class RecipeEndpoint : BaseBlizzardEndpoint<Recipe>
{
    public int RecipeId { get; }

    public RecipeEndpoint(int recipeId)
    {
        RecipeId = recipeId;
    }

    public override string BuildUrl() =>
        $"https://us.api.blizzard.com/data/wow/recipe/{RecipeId}?namespace=static-us&locale=en_US";

    public override Recipe Parse(JsonElement json)
    {
        Recipe recipe = new();
        recipe.Id = json.GetProperty("id").GetInt32();
        recipe.Name = json.GetProperty("name").GetString()!;
        recipe.CraftedItemId = json.GetProperty("crafted_item").GetProperty("id").GetInt32();
        recipe.CraftedQuantity = json.GetProperty("crafted_quantity").GetProperty("value").GetInt32();
        
        // Parse reagents array into semicolon-delimited string
        StringBuilder reagentsBuilder = new();
        JsonElement reagentsArray = json.GetProperty("reagents");
        foreach (JsonElement reagentElement in reagentsArray.EnumerateArray())
        {
            int reagentId = reagentElement.GetProperty("reagent").GetProperty("id").GetInt32();
            int quantity = reagentElement.GetProperty("quantity").GetInt32();
            reagentsBuilder.Append($"{reagentId}:{quantity};");
        }
        recipe.Reagents = reagentsBuilder.ToString();
        
        recipe.Status = ETLStateType.COMPLETE;
        recipe.LastUpdatedUtc = DateTime.UtcNow;

        return recipe;
    }
}