using Core.Logs;

namespace Addon;

public static class LuaPublisher
{
    private const string ADDON_NAME = "WarboundIO";
    private static readonly string[] DEFAULT_WOW_PATHS = [
        @"C:\Program Files (x86)\World of Warcraft\_retail_\Interface\AddOns",
        @"C:\Program Files (x86)\World of Warcraft\_ptr_\Interface\AddOns"
    ];

    public static bool Publish() => Publish(null, null);

    public static bool Publish(string? customSourcePath, string[]? customTargetPaths)
    {
        try
        {
            string luaSourcePath = customSourcePath ?? FindLuaSourcePath();
            string[] wowPaths = customTargetPaths ?? DEFAULT_WOW_PATHS;

            foreach (string wowBasePath in wowPaths)
            {
                PublishToWowDirectory(luaSourcePath, wowBasePath);
            }

            Logging.Info(nameof(LuaPublisher), "LUA publishing completed successfully");
            return true;
        }
        catch (Exception ex)
        {
            Logging.Error(nameof(LuaPublisher), "Error during LUA publishing", ex);
            return false;
        }
    }

    private static string FindLuaSourcePath()
    {
        string currentDir = Directory.GetCurrentDirectory();
        DirectoryInfo? dir = new(currentDir);

        while (dir != null)
        {
            string addonLuaPath = Path.Combine(dir.FullName, "Addon", "LUA");
            if (Directory.Exists(addonLuaPath))
            {
                return addonLuaPath;
            }
            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException("Could not find Addon/LUA source directory");
    }

    private static void PublishToWowDirectory(string sourceDir, string wowBasePath)
    {
        string targetDir = Path.Combine(wowBasePath, ADDON_NAME);
        
        Directory.CreateDirectory(targetDir);

        foreach (string file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
        {
            string relativePath = Path.GetRelativePath(sourceDir, file);
            string targetFile = Path.Combine(targetDir, relativePath);
            
            string? targetSubDir = Path.GetDirectoryName(targetFile);
            if (targetSubDir != null)
            {
                Directory.CreateDirectory(targetSubDir);
            }

            File.Copy(file, targetFile, overwrite: true);
        }

        Logging.Info(nameof(LuaPublisher), $"Published to: {targetDir}");
    }
}
