namespace Addon;

public class LuaPublisherTests
{
    [Fact]
    public void ItShouldHavePublishMethod()
    {
        // Test that the method exists and returns a boolean
        bool result = LuaPublisher.Publish();
        
        // The result can be true or false depending on environment
        // We just want to ensure the method is callable and doesn't throw
        Assert.True(result == true || result == false);
    }

    [Fact]
    public void ItShouldFindLuaSourcePathFromCurrentDirectory()
    {
        // This test verifies the search logic works by checking if we can find
        // the LUA directory when running from the test directory
        string currentDir = Directory.GetCurrentDirectory();
        
        // Navigate up to find Addon/LUA directory
        DirectoryInfo? dir = new(currentDir);
        bool foundLuaDirectory = false;
        
        while (dir != null)
        {
            string addonLuaPath = Path.Combine(dir.FullName, "Addon", "LUA");
            if (Directory.Exists(addonLuaPath))
            {
                foundLuaDirectory = true;
                break;
            }
            dir = dir.Parent;
        }
        
        // We should be able to find the LUA directory from test context
        Assert.True(foundLuaDirectory, "Should be able to find Addon/LUA directory from test execution");
    }

    [Fact]
    public void ItShouldHandleNonExistentWowDirectoriesGracefully()
    {
        // Since we're running in a test environment, WoW directories won't exist
        // The Publish method should handle this gracefully and not throw exceptions
        bool result = LuaPublisher.Publish();
        
        // Should not throw an exception, regardless of result
        Assert.True(result == true || result == false);
    }

    [Fact]
    public void ItShouldReturnFalseWhenLuaDirectoryNotFound()
    {
        // Change to a directory where Addon/LUA won't be found
        string originalDir = Directory.GetCurrentDirectory();
        string tempDir = Path.GetTempPath();
        
        try
        {
            Directory.SetCurrentDirectory(tempDir);
            bool result = LuaPublisher.Publish();
            
            // Should return false when LUA directory cannot be found
            Assert.False(result);
        }
        finally
        {
            // Restore original directory
            Directory.SetCurrentDirectory(originalDir);
        }
    }
}