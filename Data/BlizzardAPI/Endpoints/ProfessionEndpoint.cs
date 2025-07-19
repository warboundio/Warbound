using System;
using System.Linq;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.General;

namespace Data.BlizzardAPI.Endpoints;

public class ProfessionEndpoint : BaseBlizzardEndpoint<Profession>
{
    public int ProfessionId { get; private set; }

    public ProfessionEndpoint(int professionId) { ProfessionId = professionId; }

    public override string BuildUrl() => $"https://us.api.blizzard.com/data/wow/profession/{ProfessionId}?namespace=static-us&locale=en_US";

    public override Profession Parse(JsonElement json)
    {
        int id = json.GetProperty("id").GetInt32();
        string name = json.GetProperty("name").GetString()!;
        string type = json.GetProperty("type").GetProperty("type").GetString()!;

        string skillTiers = string.Empty;
        if (json.TryGetProperty("skill_tiers", out JsonElement skillTiersElement))
        {
            int[] skillTierIds = skillTiersElement.EnumerateArray()
                .Select(tier => tier.GetProperty("id").GetInt32())
                .ToArray();
            skillTiers = string.Join(";", skillTierIds);
        }

        Profession professionObj = new();
        professionObj.Id = id;
        professionObj.Name = name;
        professionObj.Type = type;
        professionObj.SkillTiers = skillTiers;
        professionObj.Status = ETLStateType.COMPLETE;
        professionObj.LastUpdatedUtc = DateTime.UtcNow;

        return professionObj;
    }
}