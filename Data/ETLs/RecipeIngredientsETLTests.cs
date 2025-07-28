namespace Data.ETLs;

public class RecipeIngredientsETLTests
{
    [Fact]
    public void ItShouldParseReagentString()
    {
        // Test the parsing logic that will be used in the ETL
        string reagentString = "12345:2;67890:5;11111:1;";

        string[] reagentPairs = reagentString.Split(';', System.StringSplitOptions.RemoveEmptyEntries);
        System.Collections.Generic.List<int> itemIds = [];

        foreach (string reagentPair in reagentPairs)
        {
            string[] parts = reagentPair.Split(':', System.StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 1 && int.TryParse(parts[0], out int itemId))
            {
                itemIds.Add(itemId);
            }
        }

        Assert.Equal(3, itemIds.Count);
        Assert.Contains(12345, itemIds);
        Assert.Contains(67890, itemIds);
        Assert.Contains(11111, itemIds);
    }

    [Fact]
    public void ItShouldHandleEmptyReagentString()
    {
        string reagentString = "";

        string[] reagentPairs = reagentString.Split(';', System.StringSplitOptions.RemoveEmptyEntries);
        System.Collections.Generic.List<int> itemIds = [];

        foreach (string reagentPair in reagentPairs)
        {
            string[] parts = reagentPair.Split(':', System.StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 1 && int.TryParse(parts[0], out int itemId))
            {
                itemIds.Add(itemId);
            }
        }

        Assert.Empty(itemIds);
    }

    [Fact]
    public void ItShouldHandleInvalidReagentString()
    {
        string reagentString = "invalid:text;12345:2;notanumber:1;";

        string[] reagentPairs = reagentString.Split(';', System.StringSplitOptions.RemoveEmptyEntries);
        System.Collections.Generic.List<int> itemIds = [];

        foreach (string reagentPair in reagentPairs)
        {
            string[] parts = reagentPair.Split(':', System.StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 1 && int.TryParse(parts[0], out int itemId))
            {
                itemIds.Add(itemId);
            }
        }

        Assert.Single(itemIds);
        Assert.Contains(12345, itemIds);
    }
}
