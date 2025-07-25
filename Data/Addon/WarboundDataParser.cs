#pragma warning disable CA1310, SYSLIB1045, IDE0028, CA1866, CS8600, IDE0010, CS1847, IDE0011, CA1847

using System;
using System.Collections.Generic;
using System.IO;

namespace Data.Addon;

public class WarboundDataParser
{
    private readonly string[] _lines;
    private readonly long _createdAt;

    public WarboundDataParser(string filePath)
    {
        _lines = File.ReadAllLines(filePath);

        foreach (string line in _lines)
        {
            if (line.Trim().StartsWith("[\"dataCreatedAt\"]"))
            {
                string value = line.Split('=')[1].Trim().TrimEnd(',');
                _createdAt = long.TryParse(value, out long ts) ? ts : 0;
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

    public List<LootLogEntry> GetLootData()
    {
        List<LootLogEntry> list = new();

        for (int i = 0; i < _lines.Length; i++)
        {
            if (_lines[i].Trim().StartsWith("[\"dataLoot\"]"))
            {
                int end = GetSectionEnd(i);
                LootLogEntry entry = null;

                for (int j = i + 1; j < end; j++)
                {
                    string line = _lines[j].Trim();

                    if (line == "{") entry = new LootLogEntry();
                    else if (line.StartsWith("},") && entry != null)
                    {
                        entry.Id = Guid.NewGuid();
                        entry.CreatedAt = DateTimeOffset.FromUnixTimeSeconds(_createdAt).UtcDateTime;
                        list.Add(entry);
                        entry = null;
                    }
                    else if (entry != null && line.StartsWith("[\""))
                    {
                        string[] parts = line.Split('=');
                        string key = parts[0].Trim('[', ']', '"', ' ');
                        string val = parts[1].Trim().TrimEnd(',');

                        if (int.TryParse(val, out int parsed))
                        {
                            switch (key)
                            {
                                case "npcID": entry.NpcId = parsed; break;
                                case "itemID": entry.ItemId = parsed; break;
                                case "quantity": entry.Quantity = parsed; break;
                                case "zoneID": entry.ZoneId = parsed; break;
                                case "x": entry.X = parsed; break;
                                case "y": entry.Y = parsed; break;
                            }
                        }
                    }
                }
            }
        }

        return list;
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
