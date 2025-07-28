namespace Data.ETLs;

public class JournalEncounterItemsETLTests
{
    [Fact]
    public void ItShouldParseItemString()
    {
        // Test the parsing logic that will be used in the ETL
        string itemString = "12345;67890;11111;";

        string[] itemIdStrings = itemString.Split(';', System.StringSplitOptions.RemoveEmptyEntries);
        System.Collections.Generic.List<int> itemIds = [];

        foreach (string itemIdString in itemIdStrings)
        {
            if (int.TryParse(itemIdString, out int itemId))
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
    public void ItShouldHandleEmptyItemString()
    {
        string itemString = "";

        string[] itemIdStrings = itemString.Split(';', System.StringSplitOptions.RemoveEmptyEntries);
        System.Collections.Generic.List<int> itemIds = [];

        foreach (string itemIdString in itemIdStrings)
        {
            if (int.TryParse(itemIdString, out int itemId))
            {
                itemIds.Add(itemId);
            }
        }

        Assert.Empty(itemIds);
    }

    [Fact]
    public void ItShouldHandleInvalidItemString()
    {
        string itemString = "invalid;12345;notanumber;";

        string[] itemIdStrings = itemString.Split(';', System.StringSplitOptions.RemoveEmptyEntries);
        System.Collections.Generic.List<int> itemIds = [];

        foreach (string itemIdString in itemIdStrings)
        {
            if (int.TryParse(itemIdString, out int itemId))
            {
                itemIds.Add(itemId);
            }
        }

        Assert.Single(itemIds);
        Assert.Contains(12345, itemIds);
    }
}
