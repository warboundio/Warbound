#pragma warning disable CS8600

using System;
using System.Linq;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;
using Data.BlizzardAPI.Models;

namespace Data.BlizzardAPI.Endpoints;

public class RecipeMediaEndpoint : BaseBlizzardEndpoint<RecipeMedia>
{
    public int RecipeId { get; }

    public RecipeMediaEndpoint(int recipeId)
    {
        RecipeId = recipeId;
    }

    public override string BuildUrl() =>
        $"https://us.api.blizzard.com/data/wow/media/recipe/{RecipeId}?namespace=static-us&locale=en_US";

    public override RecipeMedia Parse(JsonElement json)
    {
        int id = json.GetProperty("id").GetInt32();

        JsonElement assets = json.GetProperty("assets");
        JsonElement firstAsset = assets.EnumerateArray().First();
        string url = firstAsset.GetProperty("value").GetString()!;

        RecipeMedia mediaObj = new();
        mediaObj.Id = id;
        mediaObj.URL = url;
        mediaObj.Status = ETLStateType.COMPLETE;
        mediaObj.LastUpdatedUtc = DateTime.UtcNow;

        return mediaObj;
    }
}
