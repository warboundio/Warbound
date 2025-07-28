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
            SlotType = "Head",
            ClassType = "UNKNOWN",
            SubclassType = SubclassType.QUEST_QUEST,
        });

        Assert.Equal("A|Head|yABhNe", encodedValue);
        Assert.Equal("A|UNKNOWN|ABhBNe", encodedValue);
    }

    [Fact]
    public void ItShouldGetEncodingTranslation()
    {
        EncodingStringBuilder esb = new('A', typeof(ItemAppearance));
        string translation = esb.GetEncodingTranslation();
        Assert.Equal("A|SlotType_(-1)|ClassType(1)|Id(3)|SubclassType(2)", translation);
        Assert.Equal("A|ClassType_(-1)|Id(3)|SlotType(1)|SubclassType(2)", translation);
    }
}
