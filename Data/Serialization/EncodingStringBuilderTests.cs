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
            ClassType = ClassType.UNKNOWN,
            SubclassType = SubclassType.QUEST_QUEST,
        });

        Assert.Equal("A|Head|yABhNe", encodedValue);
    }

    [Fact]
    public void ItShouldGetEncodingTranslation()
    {
        EncodingStringBuilder esb = new('A', typeof(ItemAppearance));
        string translation = esb.GetEncodingTranslation();
        Assert.Equal("A|SlotType_(-1)|ClassType(1)|Id(3)|SubclassType(2)", translation);
    }
}
