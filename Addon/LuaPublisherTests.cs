namespace Addon;

public class LuaPublisherTests
{
    private const string TEST_BASE_PATH = @"C:\Applications\Warbound\temp";

    [Fact]
    public void ItShouldPublishLuaFilesToTargetDirectories()
    {
        string testDir = Path.Combine(TEST_BASE_PATH, Guid.NewGuid().ToString());
        string sourceDir = Path.Combine(testDir, "source");
        string target1 = Path.Combine(testDir, "target1");
        string target2 = Path.Combine(testDir, "target2");

        try
        {
            Directory.CreateDirectory(sourceDir);
            Directory.CreateDirectory(Path.Combine(sourceDir, "Tools"));

            string luaFile = Path.Combine(sourceDir, "WarboundIO.lua");
            string tocFile = Path.Combine(sourceDir, "WarboundIO.toc");
            string toolFile = Path.Combine(sourceDir, "Tools", "Helper.lua");

            File.WriteAllText(luaFile, "-- Main addon file");
            File.WriteAllText(tocFile, "## Interface: 11503");
            File.WriteAllText(toolFile, "-- Helper functions");

            bool result = LuaPublisher.Publish(sourceDir, [target1, target2]);

            Assert.True(result);
            
            string[] expectedFiles = [
                Path.Combine(target1, "WarboundIO", "WarboundIO.lua"),
                Path.Combine(target1, "WarboundIO", "WarboundIO.toc"),
                Path.Combine(target1, "WarboundIO", "Tools", "Helper.lua"),
                Path.Combine(target2, "WarboundIO", "WarboundIO.lua"),
                Path.Combine(target2, "WarboundIO", "WarboundIO.toc"),
                Path.Combine(target2, "WarboundIO", "Tools", "Helper.lua")
            ];

            foreach (string expectedFile in expectedFiles)
            {
                Assert.True(File.Exists(expectedFile), $"Expected file not found: {expectedFile}");
            }

            Assert.Equal("-- Main addon file", File.ReadAllText(Path.Combine(target1, "WarboundIO", "WarboundIO.lua")));
            Assert.Equal("## Interface: 11503", File.ReadAllText(Path.Combine(target2, "WarboundIO", "WarboundIO.toc")));
        }
        finally
        {
            if (Directory.Exists(testDir))
            {
                Directory.Delete(testDir, recursive: true);
            }
        }
    }

    [Fact]
    public void ItShouldReturnFalseWhenSourceDirectoryNotFound()
    {
        string tempDir = Path.GetTempPath();
        string originalDir = Directory.GetCurrentDirectory();
        
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

    [Fact]
    public void ItShouldReturnFalseOnPublishException()
    {
        string invalidSource = Path.Combine(TEST_BASE_PATH, "nonexistent");
        string validTarget = Path.Combine(TEST_BASE_PATH, "target");

        bool result = LuaPublisher.Publish(invalidSource, [validTarget]);

        Assert.False(result);
    }
}