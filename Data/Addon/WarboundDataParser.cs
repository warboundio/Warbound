#pragma warning disable CA1310, SYSLIB1045, IDE0028, CA1866, CS8600, IDE0010, CS1847, IDE0011, CA1847

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data.BlizzardAPI;
using Data.BlizzardAPI.Models;

namespace Data.Addon;

public class WarboundDataParser
{
    private readonly string[] _lines;

    public WarboundDataParser(string filePath)
    {
        _lines = File.ReadAllLines(filePath);

        foreach (string line in _lines)
        {
            if (line.Trim().StartsWith("[\"dataCreatedAt\"]"))
            {
                //string value = line.Split('=')[1].Trim().TrimEnd(',');
                break;
            }
        }
    }

    private int GetSectionEnd(int start)
    {
        for (int i = start + 1; i < _lines.Length; i++)
        {
            string line = _lines[i].Trim();
            if (line.StartsWith("[\"data"))
            {
                return i;
            }
        }
        return _lines.Length;
    }

    public Dictionary<int, int> GetItemIdToMountIdMapping()
    {
        Dictionary<int, int> mapping = new();

        for (int i = 0; i < _lines.Length; i++)
        {
            string line = _lines[i].Trim();
            if (line.StartsWith("[\"dataMountItemIdMapping\"]"))
            {
                int eqIdx = line.IndexOf('=');
                if (eqIdx >= 0)
                {
                    string value = line[(eqIdx + 1)..].Trim().Trim(',', '"');
                    string[] pairs = value.Split('|', StringSplitOptions.RemoveEmptyEntries);

                    foreach (string pair in pairs)
                    {
                        string[] parts = pair.Split(':', StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 2 &&
                            int.TryParse(parts[0], out int itemId) &&
                            int.TryParse(parts[1], out int mountId))
                        {
                            mapping[itemId] = mountId;
                        }
                    }
                }
            }
        }

        return mapping;
    }

    public List<ObjectExpansionMapping> GetItemExpansionMappings()
    {
        List<ObjectExpansionMapping> mappings = [];

        for (int i = 0; i < _lines.Length; i++)
        {
            string line = _lines[i].Trim();
            if (line.StartsWith("[\"dataExpansionItemIdMapping\"]"))
            {
                int eqIdx = line.IndexOf('=');
                if (eqIdx >= 0)
                {
                    string value = line[(eqIdx + 1)..].Trim().Trim(',', '"');
                    int idx = 0;
                    while (idx < value.Length)
                    {
                        int start = value.IndexOf('|', idx);
                        if (start == -1) break;
                        int end = value.IndexOf('|', start + 1);
                        if (end == -1) break;
                        int nextPipe = value.IndexOf('|', end + 1);
                        int expansionId = int.Parse(value.Substring(start + 1, end - start - 1));
                        int itemsStart = end + 1;
                        int itemsEnd = nextPipe != -1 ? nextPipe : value.Length;
                        string itemsSection = value[itemsStart..itemsEnd];
                        string[] itemIds = itemsSection.Split(';', StringSplitOptions.RemoveEmptyEntries);
                        foreach (string itemIdStr in itemIds)
                        {
                            if (int.TryParse(itemIdStr, out int itemId))
                            {
                                mappings.Add(new ObjectExpansionMapping
                                {
                                    Id = itemId,
                                    CollectionType = 'I',
                                    ExpansionId = expansionId
                                });
                            }
                        }
                        idx = itemsEnd;
                    }
                }
            }
        }

        using BlizzardAPIContext context = new();
        Dictionary<int, int> expansionsByLuaId = context.JournalExpansions.ToDictionary(e => e.ExpansionIdLUA, e => e.Id);

        List<ObjectExpansionMapping> filteredMappings = [];
        foreach (ObjectExpansionMapping mapping in mappings)
        {
            if (mapping.ExpansionId == -1) continue;
            if (expansionsByLuaId.TryGetValue(mapping.ExpansionId, out int expansionId))
            {
                mapping.ExpansionId = expansionId;
                filteredMappings.Add(mapping);
            }
        }

        return filteredMappings;
    }

    public List<NpcKillCount> GetNpcKills()
    {
        List<NpcKillCount> list = new();

        for (int i = 0; i < _lines.Length; i++)
        {
            if (_lines[i].Trim().StartsWith("[\"dataKill\"]"))
            {
                int end = GetSectionEnd(i);
                for (int j = i + 1; j < end; j++)
                {
                    string line = _lines[j].Trim();
                    if (line.StartsWith("[") && line.Contains("="))
                    {
                        string[] parts = line.Split('=');
                        if (int.TryParse(parts[0].Trim('[', ']', ' '), out int npcID) &&
                            int.TryParse(parts[1].Trim().TrimEnd(','), out int count))
                        {
                            list.Add(new NpcKillCount { NpcId = npcID, Count = count });
                        }
                    }
                }
            }
        }

        return list;
    }

    public (List<LootItemSummary> itemSummaries, List<LootLocationEntry> locationEntries) GetLootData()
    {
        Dictionary<(int NpcId, int ItemId), int> itemQuantities = new();
        HashSet<(int NpcId, int X, int Y, int ZoneId)> uniqueLocations = new();

        for (int i = 0; i < _lines.Length; i++)
        {
            if (_lines[i].Trim().StartsWith("[\"dataLoot\"]"))
            {
                int end = GetSectionEnd(i);
                int npcId = 0, itemId = 0, quantity = 0, zoneId = 0, x = 0, y = 0;

                for (int j = i + 1; j < end; j++)
                {
                    string line = _lines[j].Trim();

                    if (line == "{")
                    {
                        npcId = itemId = quantity = zoneId = x = y = 0;
                    }
                    else if (line.StartsWith("},"))
                    {
                        // Aggregate item quantities
                        (int NpcId, int ItemId) itemKey = (npcId, itemId);
                        if (itemQuantities.ContainsKey(itemKey))
                        {
                            itemQuantities[itemKey] += quantity;
                        }
                        else
                        {
                            itemQuantities[itemKey] = quantity;
                        }

                        // Track unique locations
                        (int NpcId, int X, int Y, int ZoneId) locationKey = (npcId, x, y, zoneId);
                        uniqueLocations.Add(locationKey);
                    }
                    else if (line.StartsWith("[\""))
                    {
                        string[] parts = line.Split('=');
                        string key = parts[0].Trim('[', ']', '"', ' ');
                        string val = parts[1].Trim().TrimEnd(',');

                        if (int.TryParse(val, out int parsed))
                        {
                            switch (key)
                            {
                                case "npcID": npcId = parsed; break;
                                case "itemID": itemId = parsed; break;
                                case "quantity": quantity = parsed; break;
                                case "zoneID": zoneId = parsed; break;
                                case "x": x = parsed; break;
                                case "y": y = parsed; break;
                            }
                        }
                    }
                }
            }
        }

        List<LootItemSummary> itemSummaries = [.. itemQuantities.Select(kvp => new LootItemSummary
        {
            NpcId = kvp.Key.NpcId,
            ItemId = kvp.Key.ItemId,
            Quantity = kvp.Value
        })];

        List<LootLocationEntry> locationEntries = [.. uniqueLocations.Select(loc => new LootLocationEntry
        {
            NpcId = loc.NpcId,
            X = loc.X,
            Y = loc.Y,
            ZoneId = loc.ZoneId
        })];

        return (itemSummaries.Where(o => o.NpcId > 0).ToList(), locationEntries.Where(o => o.NpcId > 0).ToList());
    }

    public List<Vendor> GetVendors()
    {
        List<Vendor> list = [];
        bool inSection = false;
        Vendor current = null;

        for (int i = 0; i < _lines.Length; i++)
        {
            string line = _lines[i].Trim();

            if (!inSection)
            {
                if (line.StartsWith("[\"dataVendor\"]"))
                {
                    inSection = true;
                    continue;
                }
            }

            if (inSection)
            {
                if (line.StartsWith("[") && line.Contains("] = {"))
                {
                    string idPart = line.Split(']')[0].Trim('[', ']');
                    if (int.TryParse(idPart, out int npcId))
                    {
                        current = new Vendor { NpcId = npcId };
                        list.Add(current);
                    }
                }
                else if (line.StartsWith("},")) // end of current vendor
                {
                    current = null;
                }
                else if (current != null && line.Contains("="))
                {
                    string[] parts = line.Split('=');
                    string key = parts[0].Trim('[', ']', '"', ' ');
                    string val = parts[1].Trim().Trim(',', '"');

                    switch (key)
                    {
                        case "name": current.Name = val; break;
                        case "type": current.Type = val; break;
                        case "faction": current.Faction = val; break;
                        case "mapID": current.MapId = int.Parse(val); break;
                        case "x": current.X = int.Parse(val); break;
                        case "y": current.Y = int.Parse(val); break;
                    }
                }

                if (line.StartsWith("[\"data") && !line.StartsWith("[\"dataVendor\""))
                {
                    break; // we hit the next top-level section
                }
            }
        }

        return list;
    }

    public List<VendorItem> GetVendorItems()
    {
        List<VendorItem> list = new();

        for (int i = 0; i < _lines.Length; i++)
        {
            if (_lines[i].Trim().StartsWith("[\"dataVendorItems\"]"))
            {
                int end = GetSectionEnd(i);
                VendorItem entry = null;

                for (int j = i + 1; j < end; j++)
                {
                    string line = _lines[j].Trim();

                    if (line == "{") entry = new VendorItem();
                    else if (line.StartsWith("},") && entry != null)
                    {
                        list.Add(entry);
                        entry = null;
                    }
                    else if (entry != null && line.Contains("="))
                    {
                        string[] parts = line.Split('=');
                        string key = parts[0].Trim('[', ']', '"', ' ');
                        string val = parts[1].Trim().Trim(',', '"');

                        if (int.TryParse(val, out int parsed))
                        {
                            switch (key)
                            {
                                case "itemID": entry.ItemId = parsed; break;
                                case "quantity": entry.Quantity = parsed; break;
                                case "cost": entry.Cost = parsed; break;
                                case "costID": entry.CostId = parsed; break;
                                case "vendorID": entry.VendorId = parsed; break;
                            }
                        }
                        else if (key == "costType") entry.CostType = val;
                    }
                }
            }
        }

        return list;
    }

    public List<PetBattleLocation> GetPetBattleLocations()
    {
        List<PetBattleLocation> list = new();

        for (int i = 0; i < _lines.Length; i++)
        {
            if (_lines[i].Trim().StartsWith("[\"dataBattlePetLocations\"]"))
            {
                int end = GetSectionEnd(i);
                PetBattleLocation entry = null;

                for (int j = i + 1; j < end; j++)
                {
                    string line = _lines[j].Trim();

                    if (line == "{") entry = new PetBattleLocation();
                    else if (line.StartsWith("},") && entry != null)
                    {
                        list.Add(entry);
                        entry = null;
                    }
                    else if (entry != null && line.Contains("="))
                    {
                        string[] parts = line.Split('=');
                        string key = parts[0].Trim('[', ']', '"', ' ');
                        string val = parts[1].Trim().Trim(',', '"');

                        if (int.TryParse(val, out int parsed))
                        {
                            switch (key)
                            {
                                case "mapID": entry.MapId = parsed; break;
                                case "x": entry.X = parsed; break;
                                case "y": entry.Y = parsed; break;
                                case "id": entry.PetSpeciesId = parsed; break;
                            }
                        }
                    }
                }
            }
        }

        return list;
    }
}
