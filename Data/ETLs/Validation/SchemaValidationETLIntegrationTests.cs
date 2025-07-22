using System;

namespace Data.ETLs.Validation;

public class SchemaValidationETLIntegrationTests
{
    [Fact]
    public void ItShouldGenerateCorrectItemEndpointUrl()
    {
        SchemaValidationETL etl = new();
        string url = etl.GetEndpointUrl("Item", 19019);
        Assert.Equal("https://us.api.blizzard.com/data/wow/item/19019?namespace=static-us&locale=en_US", url);
    }

    [Fact]
    public void ItShouldGenerateCorrectMountEndpointUrl()
    {
        SchemaValidationETL etl = new();
        string url = etl.GetEndpointUrl("Mount", 35);
        Assert.Equal("https://us.api.blizzard.com/data/wow/mount/35?namespace=static-us&locale=en_US", url);
    }

    [Fact]
    public void ItShouldGenerateCorrectPetEndpointUrl()
    {
        SchemaValidationETL etl = new();
        string url = etl.GetEndpointUrl("Pet", 39);
        Assert.Equal("https://us.api.blizzard.com/data/wow/pet/39?namespace=static-us&locale=en_US", url);
    }

    [Fact]
    public void ItShouldGenerateCorrectToyEndpointUrl()
    {
        SchemaValidationETL etl = new();
        string url = etl.GetEndpointUrl("Toy", 1131);
        Assert.Equal("https://us.api.blizzard.com/data/wow/toy/1131?namespace=static-us&locale=en_US", url);
    }

    [Fact]
    public void ItShouldThrowForUnknownEndpointType()
    {
        SchemaValidationETL etl = new();
        Assert.Throws<ArgumentException>(() => etl.GetEndpointUrl("Unknown", 123));
    }

    [Fact]
    public void ItShouldGenerateCorrectAchievementEndpointUrl()
    {
        SchemaValidationETL etl = new();
        string url = etl.GetEndpointUrl("Achievement", 9713);
        Assert.Equal("https://us.api.blizzard.com/data/wow/achievement/9713?namespace=static-us&locale=en_US", url);
    }

    [Fact]
    public void ItShouldGenerateCorrectAchievementIndexEndpointUrl()
    {
        SchemaValidationETL etl = new();
        string url = etl.GetEndpointUrl("AchievementIndex", null);
        Assert.Equal("https://us.api.blizzard.com/data/wow/achievement/?namespace=static-us&locale=en_US", url);
    }

    [Fact]
    public void ItShouldGenerateCorrectToyIndexEndpointUrl()
    {
        SchemaValidationETL etl = new();
        string url = etl.GetEndpointUrl("ToyIndex", null);
        Assert.Equal("https://us.api.blizzard.com/data/wow/toy/?namespace=static-us&locale=en_US", url);
    }

    [Fact]
    public void ItShouldGenerateCorrectRecipeEndpointUrl()
    {
        SchemaValidationETL etl = new();
        string url = etl.GetEndpointUrl("Recipe", 1631);
        Assert.Equal("https://us.api.blizzard.com/data/wow/recipe/1631?namespace=static-us&locale=en_US", url);
    }

    [Fact]
    public void ItShouldGenerateCorrectRealmEndpointUrl()
    {
        SchemaValidationETL etl = new();
        string url = etl.GetEndpointUrl("Realm", "tichondrius");
        Assert.Equal("https://us.api.blizzard.com/data/wow/realm/tichondrius?namespace=dynamic-us&locale=en_US", url);
    }
}
