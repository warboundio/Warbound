using Core.Logs;

namespace Addon;

public class LUAPublisher
{
    private const string ADDON_NAME = "WarboundIO";
    private static readonly string[] DEFAULT_WOW_PATHS = [
        @"C:\Program Files (x86)\World of Warcraft\_retail_\Interface\AddOns",
        @"C:\Program Files (x86)\World of Warcraft\_ptr_\Interface\AddOns"
    ];

    private readonly string? _customSourcePath;

    public LUAPublisher(string? customSourcePath = null)
    {
        _customSourcePath = customSourcePath;
    }

    public static bool Publish()
    {
        try
        {
            LUAPublisher publisher = new();
            publisher.Publish(DEFAULT_WOW_PATHS[0]);
            publisher.Publish(DEFAULT_WOW_PATHS[1]);
            return true;
        }
        catch (Exception ex)
        {
            Logging.Error(nameof(LUAPublisher), "Error during LUA publishing", ex);
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
        Logging.Info(nameof(LUAPublisher), $"Published to: {targetDir}");
    }

    public static string FindLuaSourcePath()
    {
        string preferredPath = @"C:\Users\DUCA\Source\Repos\WarboundWB\Addon\LUA";
        string fallbackPath = @"C:\Users\jason\source\repos\Warbound\Addon\LUA";
        return Directory.Exists(preferredPath) ? preferredPath : fallbackPath;
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
