using System.IO;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.Models;

namespace Data.BlizzardAPI.Endpoints;

public class PetEndpointTests
{
    public const int VALID_ID = 39;

    [Fact]
    public void ItShouldParsePetData()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/Json/Pet.json");
        JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

        PetEndpoint endpoint = new(VALID_ID);
        Pet result = endpoint.Parse(jsonElement);

        Assert.NotNull(result);
        Assert.Equal(VALID_ID, result.Id);
        Assert.Equal("Mechanical Squirrel", result.Name);
        Assert.Equal("MECHANICAL", result.BattlePetType);
        Assert.False(result.IsCapturable);
        Assert.True(result.IsTradable);
        Assert.True(result.IsBattlePet);
        Assert.False(result.IsAllianceOnly);
        Assert.False(result.IsHordeOnly);
        Assert.Equal("PROFESSION", result.SourceType);
        Assert.Equal("https://render.worldofwarcraft.com/us/icons/56/inv_pet_mechanicalsquirrel.jpg", result.Icon);
        Assert.Equal(ETLStateType.COMPLETE, result.Status);
    }
}
