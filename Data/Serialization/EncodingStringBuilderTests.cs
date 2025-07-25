using Data.BlizzardAPI.Endpoints;
using Data.BlizzardAPI.Enums;

namespace Data.Serialization;
public class EncodingStringBuilderTests
{
    [Fact]
    public void ItShould()
    {
        EncodingStringBuilder esb = new('A', new ItemAppearance());
        string encodedValue = esb.GetEncodedString(new ItemAppearance
        {
            Id = 123,
            SlotType = SlotType.HEAD,
            ClassType = ClassType.UNKNOWN,
            SubclassType = SubclassType.QUEST_QUEST,
        });

        Assert.Equal("A|h OsOX6", encodedValue);
    }

    [Fact]
    public void ItShouldGetEncodingTranslation()
    {
        EncodingStringBuilder esb = new('A', new ItemAppearance());
        string translation = esb.GetEncodingTranslation();
        Assert.Equal("A|ClassType(1)|Id(3)|SlotType(1)|SubclassType(2)", translation);
    }
}
