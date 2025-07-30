namespace Data.ETLs;

public class QuestRewardItemsETLTests
{
    [Fact]
    public void ItShouldParseRewardItemsString()
    {
        // Test the parsing logic that will be used in the ETL
        string rewardItemsString = "12345;67890;54321;";

        string[] itemIds = rewardItemsString.Split(';', System.StringSplitOptions.RemoveEmptyEntries);
        System.Collections.Generic.List<int> parsedItemIds = [];

        foreach (string itemIdString in itemIds)
        {
            if (int.TryParse(itemIdString, out int itemId))
            {
                parsedItemIds.Add(itemId);
            }
        }

        Assert.Equal(3, parsedItemIds.Count);
        Assert.Contains(12345, parsedItemIds);
        Assert.Contains(67890, parsedItemIds);
        Assert.Contains(54321, parsedItemIds);
    }

    [Fact]
    public void ItShouldHandleEmptyRewardItemsString()
    {
        string rewardItemsString = "";

        string[] itemIds = rewardItemsString.Split(';', System.StringSplitOptions.RemoveEmptyEntries);
        System.Collections.Generic.List<int> parsedItemIds = [];

        foreach (string itemIdString in itemIds)
        {
            if (int.TryParse(itemIdString, out int itemId))
            {
                parsedItemIds.Add(itemId);
            }
        }

        Assert.Empty(parsedItemIds);
    }

    [Fact]
    public void ItShouldHandleInvalidRewardItemsString()
    {
        string rewardItemsString = "invalid;12345;notanumber;67890;";

        string[] itemIds = rewardItemsString.Split(';', System.StringSplitOptions.RemoveEmptyEntries);
        System.Collections.Generic.List<int> parsedItemIds = [];

        foreach (string itemIdString in itemIds)
        {
            if (int.TryParse(itemIdString, out int itemId))
            {
                parsedItemIds.Add(itemId);
            }
        }

        Assert.Equal(2, parsedItemIds.Count);
        Assert.Contains(12345, parsedItemIds);
        Assert.Contains(67890, parsedItemIds);
    }
}
