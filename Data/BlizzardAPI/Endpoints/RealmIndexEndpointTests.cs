using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.Models;

namespace Data.BlizzardAPI.Endpoints;

public class RealmIndexEndpointTests
{
    [Fact]
    public void ItShouldParseRealmIndexJsonCorrectly()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/RealmIndex.json");
        RealmIndexEndpoint endpoint = new();

        List<Realm> results = endpoint.Parse(JsonSerializer.Deserialize<JsonElement>(json));

        Assert.NotNull(results);
        Assert.True(results.Count > 0);

        // Test first realm
        Realm firstRealm = results.First();
        Assert.Equal(129, firstRealm.Id);
        Assert.Equal("Gurubashi", firstRealm.Name);
        Assert.Equal("gurubashi", firstRealm.Slug);
        Assert.Equal(string.Empty, firstRealm.Category);
        Assert.Equal(string.Empty, firstRealm.Locale);
        Assert.Equal(string.Empty, firstRealm.Timezone);
        Assert.Equal(string.Empty, firstRealm.Type);
        Assert.False(firstRealm.IsTournament);
        Assert.Equal(string.Empty, firstRealm.Region);
        Assert.Equal(ETLStateType.NEEDS_ENRICHED, firstRealm.Status);

        // Verify multiple realms are parsed
        Assert.True(results.Count > 10);
    }
}
