using Data.BlizzardAPI.Models;

namespace Data.Serialization;
public class EncodingStringBuilderTests
{
    [Fact]
    public void ItShould()
    {
        EncodingStringBuilder esb = new('A', typeof(ItemAppearance));
        string encodedValue = esb.GetEncodedString(new ItemAppearance
        {
            Id = 123,

            SubclassType = "Quest",
            SlotType = "Head",
            ClassType = "CTTest",
        });

        Assert.Equal("A|CTTest|Head|Quest|ABh", encodedValue);
    }

    [Fact]
    public void ItShouldGetEncodingTranslation()
    {
        EncodingStringBuilder esb = new('A', typeof(ItemAppearance));
        string translation = esb.GetEncodingTranslation();
        Assert.Equal("A|ClassType_(-1)|SlotType_(-1)|SubclassType_(-1)|Id(3)", translation);
    }
}
