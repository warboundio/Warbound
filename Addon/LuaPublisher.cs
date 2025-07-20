namespace Addon;

/// <summary>
/// Utility for publishing LUA addon files to World of Warcraft AddOns directories
/// </summary>
public static class LuaPublisher
{
    private const string ADDON_NAME = "WarboundIO";
    private static readonly string[] WOW_PATHS = [
        @"C:\Program Files (x86)\World of Warcraft\_retail_\Interface\AddOns",
        @"C:\Program Files (x86)\World of Warcraft\_ptr_\Interface\AddOns"
    ];

    /// <summary>
    /// Publishes LUA files from the Addon/LUA folder to WoW AddOns directories
    /// </summary>
    /// <returns>True if successful, false otherwise</returns>
    public static bool Publish()
    {
        try
        {
            string? luaSourcePath = FindLuaSourcePath();
            if (luaSourcePath == null)
            {
                Console.WriteLine("Error: Could not find Addon/LUA source directory");
                return false;
            }

            Console.WriteLine($"Found LUA source directory: {luaSourcePath}");

            bool anySuccess = false;
            foreach (string wowBasePath in WOW_PATHS)
            {
                if (PublishToWowDirectory(luaSourcePath, wowBasePath))
                {
                    anySuccess = true;
                }
            }

            return anySuccess;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during LUA publishing: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Finds the Addon/LUA directory by searching up from current directory
    /// </summary>
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

    /// <summary>
    /// Publishes LUA files to a specific WoW AddOns directory
    /// </summary>
    private static bool PublishToWowDirectory(string sourceDir, string wowBasePath)
    {
        try
        {
            if (!Directory.Exists(wowBasePath))
            {
                Console.WriteLine($"Warning: WoW directory not found: {wowBasePath}");
                return false;
            }

            string targetDir = Path.Combine(wowBasePath, ADDON_NAME);
            
            // Create target directory if it doesn't exist
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
                Console.WriteLine($"Created directory: {targetDir}");
            }

            // Copy all files from LUA directory
            foreach (string file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                string relativePath = Path.GetRelativePath(sourceDir, file);
                string targetFile = Path.Combine(targetDir, relativePath);
                
                // Ensure target subdirectory exists
                string? targetSubDir = Path.GetDirectoryName(targetFile);
                if (targetSubDir != null && !Directory.Exists(targetSubDir))
                {
                    Directory.CreateDirectory(targetSubDir);
                }

                File.Copy(file, targetFile, overwrite: true);
                Console.WriteLine($"Copied: {relativePath} -> {targetFile}");
            }

            Console.WriteLine($"Successfully published to: {targetDir}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error publishing to {wowBasePath}: {ex.Message}");
            return false;
        }
    }
}