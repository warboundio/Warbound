using System;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;

namespace Data.BlizzardAPI.Endpoints;

public class PetEndpoint : BaseBlizzardEndpoint<Pet>
{
    public int PetId { get; private set; }

    public PetEndpoint(int petId) { PetId = petId; }

    public override string BuildUrl() => $"https://us.api.blizzard.com/data/wow/pet/{PetId}?namespace=static-us&locale=en_US";

    public override Pet Parse(JsonElement json)
    {
        Pet petObj = new();
        petObj.Id = json.GetProperty("id").GetInt32();
        petObj.Name = json.GetProperty("name").GetString()!;
        petObj.BattlePetType = json.GetProperty("battle_pet_type").GetProperty("type").GetString()!;
        petObj.IsCapturable = json.GetProperty("is_capturable").GetBoolean();
        petObj.IsTradable = json.GetProperty("is_tradable").GetBoolean();
        petObj.IsBattlePet = json.GetProperty("is_battlepet").GetBoolean();
        petObj.IsAllianceOnly = json.GetProperty("is_alliance_only").GetBoolean();
        petObj.IsHordeOnly = json.GetProperty("is_horde_only").GetBoolean();

        string sourceType = string.Empty;
        if (json.TryGetProperty("source", out JsonElement sourceElement) && sourceElement.TryGetProperty("type", out JsonElement typeElement))
        {
            sourceType = typeElement.GetString() ?? string.Empty;
        }
        petObj.SourceType = sourceType;

        petObj.Icon = json.GetProperty("icon").GetString()!;
        petObj.Status = ETLStateType.COMPLETE;
        petObj.LastUpdatedUtc = DateTime.UtcNow;

        return petObj;
    }
}
