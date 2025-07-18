using System.IO;
using System.Text.Json;
using ETL.BlizzardAPI.Enums;

namespace ETL.BlizzardAPI.Endpoints;

public class RealmEndpointTests
{
    [Fact]
    public void ItShouldParseRealmJsonCorrectly()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/Realm.json");
        RealmEndpoint endpoint = new("tichondrius");

        Realm? result = endpoint.Parse(JsonSerializer.Deserialize<JsonElement>(json));

        Assert.NotNull(result);
        Assert.Equal(11, result.Id);
        Assert.Equal("Tichondrius", result.Name);
        Assert.Equal("tichondrius", result.Slug);
        Assert.Equal("United States", result.Category);
        Assert.Equal("enUS", result.Locale);
        Assert.Equal("America/Los_Angeles", result.Timezone);
        Assert.Equal("NORMAL", result.Type);
        Assert.False(result.IsTournament);
        Assert.Equal("North America", result.Region);
        Assert.Equal(ETLStateType.COMPLETE, result.Status);
    }
}
