using Core.Logs;

namespace Addon;

public class LuaPublisher
{
    private const string ADDON_NAME = "WarboundIO";
    private static readonly string[] DEFAULT_WOW_PATHS = [
        @"C:\Program Files (x86)\World of Warcraft\_retail_\Interface\AddOns",
        @"C:\Program Files (x86)\World of Warcraft\_ptr_\Interface\AddOns"
    ];

    private string? _customSourcePath;

    public LuaPublisher(string? customSourcePath = null)
    {
        _customSourcePath = customSourcePath;
    }

    public static bool Publish()
    {
        try
        {
            LuaPublisher publisher = new();
            publisher.Publish(DEFAULT_WOW_PATHS[0]);
            publisher.Publish(DEFAULT_WOW_PATHS[1]);
            return true;
        }
        catch (Exception ex)
        {
            Logging.Error(nameof(LuaPublisher), "Error during LUA publishing", ex);
            return false;
        }
    }

    public void Publish(string targetFolder)
    {
        string luaSourcePath = _customSourcePath ?? FindLuaSourcePath();
        string targetDir = Path.Combine(targetFolder, ADDON_NAME);

        if (Directory.Exists(targetDir)) { Directory.Delete(targetDir, recursive: true); }
        else { Directory.CreateDirectory(targetDir); }

        CopyDirectory(luaSourcePath, targetDir);        
        Logging.Info(nameof(LuaPublisher), $"Published to: {targetDir}");
    }

    private string FindLuaSourcePath()
    {
        string currentDir = Directory.GetCurrentDirectory();
        DirectoryInfo? dir = new(currentDir);

        while (dir != null)
        {
            string addonLuaPath = Path.Combine(dir.FullName, "Addon", "LUA");
            if (Directory.Exists(addonLuaPath)) { return addonLuaPath; }
            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException("Could not find Addon/LUA source directory");
    }

    private void CopyDirectory(string sourceDir, string targetDir)
    {
        foreach (string file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
        {
            string relativePath = Path.GetRelativePath(sourceDir, file);
            string targetFile = Path.Combine(targetDir, relativePath);
            
            string? targetSubDir = Path.GetDirectoryName(targetFile);
            if (targetSubDir != null) { Directory.CreateDirectory(targetSubDir); }

            File.Copy(file, targetFile, overwrite: true);
        }
    }
}
