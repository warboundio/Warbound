#pragma warning disable CA1310, SYSLIB1045, IDE0028, CA1866, CS8600, IDE0010, CS1847, IDE0011, CA1847

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

    public List<NpcKillCount> GetNpcKills()
    {
        List<NpcKillCount> list = [];
        bool inSection = false;
        NpcKillCount current = null;

        for (int i = 0; i < _lines.Length; i++)
        {
            string line = _lines[i].Trim();

            if (!inSection)
            {
                if (line.StartsWith("[\"dataKill\"]"))
                {
                    inSection = true;
                    continue;
                }
            }

            if (inSection)
            {
                if (line.StartsWith("[") && line.Contains("] = {"))
                {
                    int open = line.IndexOf('[') + 1;
                    int close = line.IndexOf(']');
                    if (int.TryParse(line[open..close], out int npcId))
                    {
                        current = new NpcKillCount { NpcId = npcId };
                    }
                }
                else if (line.StartsWith("},")) // end of current kill entry
                {
                    if (current != null && current.Count > 0)
                    {
                        list.Add(current);
                    }
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
                        case "count":
                            if (int.TryParse(val, out int parsed)) current.Count = parsed;
                            break;
                    }
                }

                if (line.StartsWith("[\"data") && !line.StartsWith("[\"dataKill\""))
                {
                    break; // Exited dataKill section
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
                                case "factionID": entry.FactionId = parsed; break;
                            }
                        }
                        else if (key == "costType") entry.CostType = val;
                    }
                }
            }
        }

        return list;
    }

    public List<QuestLocation> GetQuestLocations()
    {
        List<QuestLocation> list = [];
        bool inSection = false;
        QuestLocation current = null;

        for (int i = 0; i < _lines.Length; i++)
        {
            string line = _lines[i].Trim();

            if (!inSection)
            {
                if (line.StartsWith("[\"dataNpcQuests\"]"))
                {
                    inSection = true;
                    continue;
                }
            }

            if (inSection)
            {
                if (line.StartsWith("{"))
                {
                    current = new QuestLocation();
                }
                else if (line.StartsWith("},") && current != null)
                {
                    list.Add(current);
                    current = null;
                }
                else if (current != null && line.Contains("="))
                {
                    string[] parts = line.Split('=');
                    string key = parts[0].Trim('[', ']', '"', ' ');
                    string val = parts[1].Trim().Trim(',', '"');

                    switch (key)
                    {
                        case "npcName":
                            current.NpcName = val;
                            break;
                        case "mapID":
                            current.MapId = int.Parse(val);
                            break;
                        case "isStart":
                            current.IsStart = val == "true";
                            break;
                        case "y":
                            if (float.TryParse(val, out float y))
                                current.Y = (int)(y * 10);
                            break;
                        case "questID":
                            current.QuestId = int.Parse(val);
                            break;
                        case "factionID":
                            current.FactionId = int.Parse(val);
                            break;
                        case "x":
                            if (float.TryParse(val, out float x))
                                current.X = (int)(x * 10);
                            break;
                        case "npcID":
                            current.NpcId = int.Parse(val);
                            break;
                    }
                }

                if (line.StartsWith("[\"data") && !line.StartsWith("[\"dataNpcQuests\""))
                {
                    break;
                }
            }
        }

        return [.. list.Where(o => o.QuestId != 0)];
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
