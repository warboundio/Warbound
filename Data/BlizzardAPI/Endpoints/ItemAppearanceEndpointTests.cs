using System.IO;
using System.Text.Json;
using Data.BlizzardAPI.Enums;

namespace Data.BlizzardAPI.Endpoints;

public class ItemAppearanceEndpointTests
{
    public const int VALID_ID = 321;

    [Fact]
    public void ItShouldParseItemAppearanceJsonCorrectly()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/ItemAppearance.json");
        JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

        ItemAppearanceEndpoint endpoint = new(VALID_ID);
        ItemAppearance result = endpoint.Parse(jsonElement);

        Assert.NotNull(result);
        Assert.Equal(VALID_ID, result.Id);
        Assert.Equal(SlotType.HEAD, result.SlotType);
        Assert.Equal(ClassType.ARMOR, result.ClassType);
        Assert.Equal(SubclassType.ARMOR_MAIL, result.SubclassType);
        Assert.Equal(1166, result.DisplayInfoId);
        Assert.Equal("11735;19945;29979;153104;187914", result.ItemIds);
    }
}
