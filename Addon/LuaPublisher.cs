using Core.Logs;

namespace Addon;

public static class LuaPublisher
{
    private const string ADDON_NAME = "WarboundIO";
    private static readonly string[] WOW_PATHS = [
        @"C:\Program Files (x86)\World of Warcraft\_retail_\Interface\AddOns",
        @"C:\Program Files (x86)\World of Warcraft\_ptr_\Interface\AddOns"
    ];

    public static bool Publish()
    {
        try
        {
            string? luaSourcePath = FindLuaSourcePath();
            if (luaSourcePath == null)
            {
                Logging.Error(nameof(LuaPublisher), "Could not find Addon/LUA source directory");
                return false;
            }

            bool anySuccess = false;
            foreach (string wowBasePath in WOW_PATHS)
            {
                if (PublishToWowDirectory(luaSourcePath, wowBasePath)) { anySuccess = true; }
            }

            return anySuccess;
        }
        catch (Exception ex)
        {
            Logging.Error(nameof(LuaPublisher), "Error during LUA publishing", ex);
            return false;
        }
    }

    private static string? FindLuaSourcePath()
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

        return null;
    }

    private static bool PublishToWowDirectory(string sourceDir, string wowBasePath)
    {
        try
        {
            if (!Directory.Exists(wowBasePath))
            {
                Logging.Warn(nameof(LuaPublisher), $"WoW directory not found: {wowBasePath}");
                return false;
            }

            string targetDir = Path.Combine(wowBasePath, ADDON_NAME);
            
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
                Logging.Info(nameof(LuaPublisher), $"Created directory: {targetDir}");
            }

            foreach (string file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                string relativePath = Path.GetRelativePath(sourceDir, file);
                string targetFile = Path.Combine(targetDir, relativePath);
                
                string? targetSubDir = Path.GetDirectoryName(targetFile);
                if (targetSubDir != null && !Directory.Exists(targetSubDir))
                {
                    Directory.CreateDirectory(targetSubDir);
                }

                File.Copy(file, targetFile, overwrite: true);
                Logging.Info(nameof(LuaPublisher), $"Copied: {relativePath} -> {targetFile}");
            }

            Logging.Info(nameof(LuaPublisher), $"Successfully published to: {targetDir}");
            return true;
        }
        catch (Exception ex)
        {
            Logging.Error(nameof(LuaPublisher), $"Error publishing to {wowBasePath}", ex);
            return false;
        }
    }
}
