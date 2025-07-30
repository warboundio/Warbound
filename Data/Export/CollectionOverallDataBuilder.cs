#pragma warning disable CS8618

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data.BlizzardAPI;
using Data.BlizzardAPI.Models;
using Data.Mappings;
using Data.Support;
using Microsoft.EntityFrameworkCore;

namespace Data.Export;

public class CollectionOverallDataBuilder : IDisposable
{
    private BlizzardAPIContext _context;
    private List<ObjectExpansionMapping> _mappings;
    private Dictionary<(char CollectionType, int Id), HashSet<int>> _mappingLookup;
    private Dictionary<char, IQueryable<int>> _collectionTypes;
    private List<string> _lines;

    public void Process()
    {
        Setup();
        UpdateData();
        ProcessPets();
        ProcessToys();
        ProcessMounts();
        ProcessAppearances();
        ProcessRecipes();
        WriteOutput();
    }

    private void UpdateData()
    {
        RecipeExpansionMappings mappings = new();
        mappings.MapAndPersist();
    }

    private void Setup()
    {
        _context = new BlizzardAPIContext();
        _mappings = [.. _context.ObjectExpansionMappings.AsNoTracking()];
        _mappingLookup = _mappings
            .GroupBy(m => (m.CollectionType, m.Id))
            .ToDictionary(
                g => g.Key,
                g => g.Select(m => m.ExpansionId).ToHashSet()
            );
        _collectionTypes = new()
        {
            { 'P', _context.Pets.AsNoTracking().Select(x => x.Id) },
            { 'T', _context.Toys.AsNoTracking().Select(x => x.Id) },
            { 'M', _context.Mounts.AsNoTracking().Select(x => x.Id) },
            { 'A', _context.ItemAppearances.AsNoTracking().Select(x => x.Id) },
            { 'R', _context.Recipes.AsNoTracking().Select(x => x.Id) }
        };
        _lines = [];
    }

    private void ProcessAppearances()
    {
        List<ItemAppearance> allAppearances = [.. _context.ItemAppearances.AsNoTracking()];
        Dictionary<int, List<int>> expansionIdToAppearanceIds = [];
        List<int> unmappedAppearanceIds = [];

        foreach (ItemAppearance? appearance in allAppearances)
        {
            List<int> itemIds = [.. appearance.ItemIds.Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .OrderBy(id => id)];

            int? expansionId = null;
            foreach (int itemId in itemIds)
            {
                if (_mappingLookup.TryGetValue(('I', itemId), out HashSet<int>? expansions) && expansions.Count > 0)
                {
                    expansionId = expansions.Min();
                    break;
                }
            }

            if (expansionId.HasValue)
            {
                if (!expansionIdToAppearanceIds.TryGetValue(expansionId.Value, out List<int>? list))
                {
                    list = [];
                    expansionIdToAppearanceIds[expansionId.Value] = list;
                }
                list.Add(appearance.Id);
            }
            else
            {
                unmappedAppearanceIds.Add(appearance.Id);
            }
        }

        foreach (KeyValuePair<int, List<int>> kv in expansionIdToAppearanceIds.OrderBy(x => x.Key))
        {
            List<string> encoded = Base90.Encode(kv.Value, 3);
            string line = $"A|{kv.Key}|{string.Concat(encoded)}";
            _lines.Add(line);
        }

        if (unmappedAppearanceIds.Count > 0)
        {
            List<string> encoded = Base90.Encode(unmappedAppearanceIds, 3);
            string line = $"A|-1|{string.Concat(encoded)}";
            _lines.Add(line);
        }
    }

    private void ProcessPets() => ProcessType('P');
    private void ProcessToys() => ProcessType('T');
    private void ProcessMounts()
    {
        List<Mount> allMounts = [.. _context.Mounts.AsNoTracking()];
        Dictionary<int, List<int>> expansionIdToMountIds = [];
        List<int> unmappedMountIds = [];

        foreach (Mount mount in allMounts)
        {
            int itemId = mount.ItemId;
            int mountId = mount.Id;

            if (_mappingLookup.TryGetValue(('I', itemId), out HashSet<int>? expansions) && expansions.Count > 0)
            {
                foreach (int expansionId in expansions)
                {
                    if (!expansionIdToMountIds.TryGetValue(expansionId, out List<int>? list))
                    {
                        list = [];
                        expansionIdToMountIds[expansionId] = list;
                    }
                    list.Add(mountId);
                }
            }
            else
            {
                unmappedMountIds.Add(mountId);
            }
        }

        foreach (KeyValuePair<int, List<int>> kv in expansionIdToMountIds.OrderBy(x => x.Key))
        {
            List<string> encoded = Base90.Encode(kv.Value, 3);
            string line = $"M|{kv.Key}|{string.Concat(encoded)}";
            _lines.Add(line);
        }

        if (unmappedMountIds.Count > 0)
        {
            List<string> encoded = Base90.Encode(unmappedMountIds, 3);
            string line = $"M|-1|{string.Concat(encoded)}";
            _lines.Add(line);
        }
    }

    private void ProcessRecipes() => ProcessType('R');

    private void ProcessType(char type)
    {
        List<int> allIds = [.. _collectionTypes[type]];
        Dictionary<int, List<int>> expansionIdToIds = [];
        List<int> unmappedIds = [];

        foreach (int id in allIds)
        {
            if (_mappingLookup.TryGetValue((type, id), out HashSet<int>? expansions) && expansions.Count > 0)
            {
                foreach (int expansionId in expansions)
                {
                    if (!expansionIdToIds.TryGetValue(expansionId, out List<int>? list))
                    {
                        list = [];
                        expansionIdToIds[expansionId] = list;
                    }
                    list.Add(id);
                }
            }
            else
            {
                unmappedIds.Add(id);
            }
        }

        foreach (KeyValuePair<int, List<int>> kv in expansionIdToIds.OrderBy(x => x.Key))
        {
            List<string> encoded = Base90.Encode(kv.Value, 3);
            string line = $"{type}|{kv.Key}|{string.Concat(encoded)}";
            _lines.Add(line);
        }

        if (unmappedIds.Count > 0)
        {
            List<string> encoded = Base90.Encode(unmappedIds, 3);
            string line = $"{type}|-1|{string.Concat(encoded)}";
            _lines.Add(line);
        }
    }

    private void WriteOutput()
    {
        Directory.CreateDirectory(@"C:\Applications\Warbound\cached");
        File.WriteAllLines(@"C:\Applications\Warbound\cached\CollectionsOverall.data", _lines);
    }

    public void Dispose() => _context.Dispose();
}
