using System.IO;
using System.Text.Json;
using Data.BlizzardAPI.Models;

namespace Data.BlizzardAPI.Endpoints;

public class ItemAppearanceEndpointTests
{
    public const int VALID_ID = 321;

    [Fact]
    public void ItShouldParseItemAppearanceJsonCorrectly()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/Json/ItemAppearance.json");
        JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

        ItemAppearanceEndpoint endpoint = new(VALID_ID);
        ItemAppearance result = endpoint.Parse(jsonElement);

        Assert.NotNull(result);
        Assert.Equal(VALID_ID, result.Id);
        Assert.Equal("Mail", result.SubclassType);
        Assert.Equal("Head", result.SlotType);
        Assert.Equal("Armor", result.ClassType);

        Assert.Equal(1166, result.DisplayInfoId);
        Assert.Equal("11735;19945;29979;153104;187914", result.ItemIds);
    }
}
