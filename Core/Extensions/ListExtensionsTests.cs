namespace Core.Extensions;

public class ListExtensionsTests
{
    [Fact]
    public void ItShouldShuffleListWithDefaultSeed()
    {
        List<int> list = [1, 2, 3, 4, 5];
        List<int> expected = [2, 1, 5, 3, 4];

        list.Shuffle();

        Assert.NotEqual(expected, list);
    }

    [Fact]
    public void ItShouldShuffleListWithCustomSeed()
    {
        List<int> list = [1, 2, 3, 4, 5];
        List<int> expected = [4, 2, 1, 5, 3];

        list.Shuffle(99);

        Assert.NotEqual(expected, list);
    }

    [Fact]
    public void ItShouldNotFailOnEmptyList()
    {
        List<int> list = [];

        list.Shuffle();

        Assert.Empty(list);
    }

    [Fact]
    public void ItShouldNotFailOnSingleElementList()
    {
        List<int> list = [42];

        list.Shuffle();

        Assert.Single(list);
        Assert.Equal(42, list[0]);
    }
}
