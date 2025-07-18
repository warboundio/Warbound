namespace Core.Extensions;

public class StringExtensionsTests
{
    [Fact]
    public void ItShouldReturnCorrectHashForKnownInput()
    {
        string input = "hello";
        string expectedHash = "2cf24dba5fb0a30e26e83b2ac5b9e29e1b161e5c1fa7425e73043362938b9824";
        string result = input.Hash();
        Assert.Equal(expectedHash, result);
    }

    [Fact]
    public void ItShouldReturnDifferentHashesForDifferentInputs()
    {
        string input1 = "test1";
        string input2 = "test2";
        string hash1 = input1.Hash();
        string hash2 = input2.Hash();
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void ItShouldReturnSameHashForSameInput()
    {
        string input = "repeat";
        string hash1 = input.Hash();
        string hash2 = input.Hash();
        Assert.Equal(hash1, hash2);
    }
}
