using Core.Logs;
using Core.Tools;
using Data.Addon;

namespace Addon;

public class AutoPublisher
{
    private static readonly List<PathMonitor> _monitors = [];

    public static void Boot()
    {
        string sourcePath = LUAPublisher.FindLuaSourcePath();
        PathMonitor folderMonitor = new(sourcePath);
        folderMonitor.Changed += (s, fullPath) => { LUAPublisher.Publish(); };

        _monitors.Add(folderMonitor);
    }

    public static void BootSimulatedClient()
    {
        List<string> luaFiles = FindWarboundIODataFiles(@"C:\Program Files (x86)\World of Warcraft\_retail_\WTF\Account\");

        foreach (string file in luaFiles)
        {
            PathMonitor savedVariablesMonitor = new(file);
            savedVariablesMonitor.Changed += (s, e) => { ParseSavedVariables(e.FullPath); };
            _monitors.Add(savedVariablesMonitor);
        }
    }

    private static void ParseSavedVariables(string path)
    {
        Logging.Info(nameof(AutoPublisher), $"Detected change in saved variables file: {path}");
        WarboundDataParser dataParser = new(path);
        List<NpcKillCount> npcKills = dataParser.GetNpcKills();
        List<LootLogEntry> lootData = dataParser.GetLootData();
        List<Vendor> vendors = dataParser.GetVendors();
        List<VendorItem> vendorItems = dataParser.GetVendorItems();
        List<PetBattleLocation> locations = dataParser.GetPetBattleLocations();
    }

    private static List<string> FindWarboundIODataFiles(string accountRootPath)
    {
        if (!Directory.Exists(accountRootPath)) { throw new DirectoryNotFoundException($"Directory not found: '{accountRootPath}'."); }
        List<string> matchingFiles = [.. Directory.EnumerateFiles(accountRootPath, "WarboundIO.lua", SearchOption.AllDirectories)];
        return matchingFiles;
    }
}
