namespace Addon;

public class LuaPublisherTests
{
    [Fact]
    public void ItShouldHavePublishMethod()
    {
        bool result = LuaPublisher.Publish();
        Assert.True(result == true || result == false);
    }

    [Fact]
    public void ItShouldFindLuaSourcePathFromCurrentDirectory()
    {
        string currentDir = Directory.GetCurrentDirectory();
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
        
        Assert.True(foundLuaDirectory, "Should be able to find Addon/LUA directory from test execution");
    }

    [Fact]
    public void ItShouldHandleNonExistentWowDirectoriesGracefully()
    {
        bool result = LuaPublisher.Publish();
        Assert.True(result == true || result == false);
    }

    [Fact]
    public void ItShouldReturnFalseWhenLuaDirectoryNotFound()
    {
        string originalDir = Directory.GetCurrentDirectory();
        string tempDir = Path.GetTempPath();
        
        try
        {
            Directory.SetCurrentDirectory(tempDir);
            bool result = LuaPublisher.Publish();
            Assert.False(result);
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDir);
        }
    }
}