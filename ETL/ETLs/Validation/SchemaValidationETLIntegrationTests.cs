using System;
using Xunit;
using ETL.BlizzardAPI.Endpoints;

namespace ETL.ETLs.Validation;

public class SchemaValidationETLIntegrationTests
{
    [Fact]
    public void ItShouldGenerateCorrectItemEndpointUrl()
    {
        // Arrange
        SchemaValidationETL etl = new();
        
        // Act
        string url = etl.GetEndpointUrl("Item", 19019);
        
        // Assert
        Assert.Equal("https://us.api.blizzard.com/data/wow/item/19019?namespace=static-us&locale=en_US", url);
    }

    [Fact]
    public void ItShouldGenerateCorrectMountEndpointUrl()
    {
        // Arrange
        SchemaValidationETL etl = new();
        
        // Act
        string url = etl.GetEndpointUrl("Mount", 35);
        
        // Assert
        Assert.Equal("https://us.api.blizzard.com/data/wow/mount/35?namespace=static-us&locale=en_US", url);
    }

    [Fact]
    public void ItShouldGenerateCorrectPetEndpointUrl()
    {
        // Arrange
        SchemaValidationETL etl = new();
        
        // Act
        string url = etl.GetEndpointUrl("Pet", 39);
        
        // Assert
        Assert.Equal("https://us.api.blizzard.com/data/wow/pet/39?namespace=static-us&locale=en_US", url);
    }

    [Fact]
    public void ItShouldGenerateCorrectToyEndpointUrl()
    {
        // Arrange
        SchemaValidationETL etl = new();
        
        // Act
        string url = etl.GetEndpointUrl("Toy", 1131);
        
        // Assert
        Assert.Equal("https://us.api.blizzard.com/data/wow/toy/1131?namespace=static-us&locale=en_US", url);
    }

    [Fact]
    public void ItShouldThrowForUnknownEndpointType()
    {
        // Arrange
        SchemaValidationETL etl = new();
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => etl.GetEndpointUrl("Unknown", 123));
    }
}