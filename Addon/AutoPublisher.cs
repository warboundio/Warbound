using Core.Logs;
using Core.Tools;
using Data.Addon;
using Data.BlizzardAPI;

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

        PersistDataToDatabase(npcKills, lootData, vendors, vendorItems, locations);
    }

    private static void PersistDataToDatabase(List<NpcKillCount> npcKills, List<LootLogEntry> lootData,
        List<Vendor> vendors, List<VendorItem> vendorItems, List<PetBattleLocation> locations)
    {
        using BlizzardAPIContext context = new();

        PersistPetBattleLocations(context, locations);
        PersistLootLogEntries(context, lootData);
        PersistNpcKillCounts(context, npcKills);
        PersistVendors(context, vendors);
        PersistVendorItems(context, vendorItems);

        context.SaveChanges();
    }

    private static void PersistPetBattleLocations(BlizzardAPIContext context, List<PetBattleLocation> locations)
    {
        foreach (PetBattleLocation location in locations)
        {
            context.G_PetBattleLocations.Add(location);
        }
    }

    private static void PersistLootLogEntries(BlizzardAPIContext context, List<LootLogEntry> lootData)
    {
        foreach (LootLogEntry entry in lootData)
        {
            context.G_LootLogEntries.Add(entry);
        }
    }

    private static void PersistNpcKillCounts(BlizzardAPIContext context, List<NpcKillCount> npcKills)
    {
        foreach (NpcKillCount newKill in npcKills)
        {
            NpcKillCount? existingKill = context.G_NpcKillCounts.Find(newKill.NpcId);
            if (existingKill != null)
            {
                existingKill.Count += newKill.Count;
            }
            else
            {
                context.G_NpcKillCounts.Add(newKill);
            }
        }
    }

    private static void PersistVendors(BlizzardAPIContext context, List<Vendor> vendors)
    {
        foreach (Vendor vendor in vendors)
        {
            Vendor? existingVendor = context.G_Vendors.Find(vendor.NpcId);
            if (existingVendor != null)
            {
                context.G_Vendors.Remove(existingVendor);
            }
            context.G_Vendors.Add(vendor);
        }
    }

    private static void PersistVendorItems(BlizzardAPIContext context, List<VendorItem> vendorItems)
    {
        HashSet<int> processedVendorIds = [];

        foreach (VendorItem vendorItem in vendorItems)
        {
            if (!processedVendorIds.Contains(vendorItem.VendorId))
            {
                List<VendorItem> existingItems =
                [
                    .. context.G_VendorItems
                                        .Where(vi => vi.VendorId == vendorItem.VendorId)
,
                ];

                context.G_VendorItems.RemoveRange(existingItems);
                processedVendorIds.Add(vendorItem.VendorId);
            }

            context.G_VendorItems.Add(vendorItem);
        }
    }

    private static List<string> FindWarboundIODataFiles(string accountRootPath)
    {
        if (!Directory.Exists(accountRootPath)) { throw new DirectoryNotFoundException($"Directory not found: '{accountRootPath}'."); }
        List<string> matchingFiles = [.. Directory.EnumerateFiles(accountRootPath, "WarboundIO.lua", SearchOption.AllDirectories)];
        return matchingFiles;
    }
}
