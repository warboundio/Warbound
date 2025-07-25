namespace Data.Support;

public class Base90Tests
{
    [Fact]
    public void ItShouldConvert()
    {
        string encoding = Base90.Encode(35, 3);
        Assert.Equal("  ~", encoding);

        ulong value = Base90.Decode("  ~");
        Assert.Equal((ulong)35, value);
    }

    [Fact]
    public void ItShouldMakeShorter()
    {
        string encoding = Base90.Encode(204593, 3);
        Assert.Equal("oTT", encoding);
    }

    [Fact]
    public void ItShouldForceLength()
    {
        string encoding = Base90.Encode(10, 1);
        Assert.Equal(".", encoding);
    }
}
