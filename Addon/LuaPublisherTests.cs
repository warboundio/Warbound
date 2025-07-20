namespace Addon;

public class LuaPublisherTests
{
    private const string TEST_BASE_PATH = @"C:\Applications\Warbound\temp";

    [Fact]
    public void ItShouldPublishLuaFilesToTargetDirectory()
    {
        string testDir = Path.Combine(TEST_BASE_PATH, Guid.NewGuid().ToString());
        string sourceDir = Path.Combine(testDir, "source");
        string targetDir = Path.Combine(testDir, "target");

        try
        {
            Directory.CreateDirectory(sourceDir);
            Directory.CreateDirectory(Path.Combine(sourceDir, "Tools"));

            File.WriteAllText(Path.Combine(sourceDir, "WarboundIO.lua"), "-- Main addon file");
            File.WriteAllText(Path.Combine(sourceDir, "WarboundIO.toc"), "## Interface: 11503");
            File.WriteAllText(Path.Combine(sourceDir, "Tools", "Helper.lua"), "-- Helper functions");

            LuaPublisher publisher = new(sourceDir);
            publisher.Publish(targetDir);

            Assert.True(File.Exists(Path.Combine(targetDir, "WarboundIO", "WarboundIO.lua")));
            Assert.True(File.Exists(Path.Combine(targetDir, "WarboundIO", "WarboundIO.toc")));
            Assert.True(File.Exists(Path.Combine(targetDir, "WarboundIO", "Tools", "Helper.lua")));
        }
        finally
        {
            if (Directory.Exists(testDir))
            {
                Directory.Delete(testDir, recursive: true);
            }
        }
    }
}