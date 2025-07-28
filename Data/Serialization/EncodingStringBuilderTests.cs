using Data.BlizzardAPI.Enums;
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
            SlotType = SlotType.HEAD,
            ClassType = ClassType.UNKNOWN,
            SubclassType = "Quest",
        });

        Assert.Equal("A|Quest|yABhB", encodedValue);
    }

    [Fact]
    public void ItShouldGetEncodingTranslation()
    {
        EncodingStringBuilder esb = new('A', typeof(ItemAppearance));
        string translation = esb.GetEncodingTranslation();
        Assert.Equal("A|SubclassType_(-1)|ClassType(1)|Id(3)|SlotType(1)", translation);
    }
}
