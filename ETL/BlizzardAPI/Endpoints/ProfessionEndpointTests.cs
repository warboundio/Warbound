using System.IO;
using System.Text.Json;
using ETL.BlizzardAPI.Enums;

namespace ETL.BlizzardAPI.Endpoints;

public class ProfessionEndpointTests
{
    public const int VALID_ID = 164;

    [Fact]
    public void ItShouldParseProfessionData()
    {
        string json = File.ReadAllText("BlizzardAPI/Endpoints/Profession.json");
        JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

        ProfessionEndpoint endpoint = new(VALID_ID);
        Profession result = endpoint.Parse(jsonElement);

        Assert.NotNull(result);
        Assert.Equal(VALID_ID, result.Id);
        Assert.Equal("Blacksmithing", result.Name);
        Assert.Equal("PRIMARY", result.Type);
        Assert.Equal("2437;2454;2472;2473;2474;2475;2476;2477;2751;2822;2872", result.SkillTiers);
        Assert.Equal(ETLStateType.COMPLETE, result.Status);
    }
}