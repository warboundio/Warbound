using System.IO;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.Models;

namespace Data.BlizzardAPI.Endpoints;

public class ItemEndpointTests
{
    public const int VALID_ID = 19019;

    [Fact]
    public void ItShouldParseItemJsonCorrectly()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/Json/Item.json");
        ItemEndpoint endpoint = new(VALID_ID);

        Item? result = endpoint.Parse(JsonSerializer.Deserialize<JsonElement>(json));

        Assert.NotNull(result);
        Assert.Equal(VALID_ID, result.Id);
        Assert.Equal("Thunderfury, Blessed Blade of the Windseeker", result.Name);
        Assert.Equal(QualityType.LEGENDARY, result.QualityType);
        Assert.Equal(29, result.Level);
        Assert.Equal(25, result.RequiredLevel);
        Assert.Equal(ClassType.WEAPON, result.ClassType);
        Assert.Equal(SubclassType.WEAPON_SWORD, result.SubclassType);
        Assert.Equal("One-Hand", result.InventoryType);
        Assert.Equal(162276, result.PurchasePrice);
        Assert.Equal(32455, result.SellPrice);
        Assert.Equal(1, result.MaxCount);
        Assert.True(result.IsEquippable);
        Assert.False(result.IsStackable);
    }
}
