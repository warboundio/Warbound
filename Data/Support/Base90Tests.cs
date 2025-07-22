namespace Data.Support;

public class Base90Tests
{
    [Fact]
    public void ItShouldConvert()
    {
        string encoding = Base90.Encode(35);
        Assert.Equal("  ~", encoding);

        ulong value = Base90.Decode("  ~");
        Assert.Equal((ulong)35, value);
    }

    [Fact]
    public void ItShouldMakeShorter()
    {
        string encoding = Base90.Encode(204593);
        Assert.Equal("oTT", encoding);
    }
}
