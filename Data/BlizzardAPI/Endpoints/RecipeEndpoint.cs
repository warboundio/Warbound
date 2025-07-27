using System;
using System.Text;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;
using Data.BlizzardAPI.Models;

namespace Data.BlizzardAPI.Endpoints;

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

        if (json.TryGetProperty("crafted_item", out JsonElement craftedItemElement) && craftedItemElement.TryGetProperty("id", out JsonElement craftedItemIdElement))
        {
            recipe.CraftedItemId = craftedItemIdElement.GetInt32();
        }

        if (json.TryGetProperty("crafted_quantity", out JsonElement craftedQuantityElement) && craftedQuantityElement.TryGetProperty("value", out JsonElement craftedQuantityValueElement))
        {
            string craftedQuantityString = craftedQuantityValueElement.ToString();
            recipe.CraftedQuantity = (int)double.Parse(craftedQuantityString);
        }

        StringBuilder reagentsBuilder = new();
        if (json.TryGetProperty("reagents", out JsonElement reagentsArray) && reagentsArray.ValueKind == JsonValueKind.Array && reagentsArray.GetArrayLength() > 0)
        {
            foreach (JsonElement reagentElement in reagentsArray.EnumerateArray())
            {
                int reagentId = reagentElement.GetProperty("reagent").GetProperty("id").GetInt32();
                int quantity = reagentElement.GetProperty("quantity").GetInt32();
                reagentsBuilder.Append($"{reagentId}:{quantity};");
            }
            recipe.Reagents = reagentsBuilder.ToString();
        }

        recipe.Status = ETLStateType.COMPLETE;
        recipe.LastUpdatedUtc = DateTime.UtcNow;

        return recipe;
    }
}
