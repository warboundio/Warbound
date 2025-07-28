using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data.BlizzardAPI;
using Data.BlizzardAPI.Models;
using Data.Support;
using Microsoft.EntityFrameworkCore;

namespace Data.Export;

public class CollectionOverallDataBuilder
{
    public static void WriteCollectionsOverallFile()
    {
        using BlizzardAPIContext _context = new();

        List<ObjectExpansionMapping> mappings = [.. _context.ObjectExpansionMappings.AsNoTracking()];

        Dictionary<char, IQueryable<int>> collectionTypes = new()
        {
            { 'P', _context.Pets.AsNoTracking().Select(x => x.Id) },
            { 'T', _context.Toys.AsNoTracking().Select(x => x.Id) },
            { 'M', _context.Mounts.AsNoTracking().Select(x => x.Id) },
            { 'A', _context.ItemAppearances.AsNoTracking().Select(x => x.Id) },
            { 'R', _context.Recipes.AsNoTracking().Select(x => x.Id) }
        };

        // Build a lookup: (type, id) => set of expansionIds
        Dictionary<(char CollectionType, int Id), HashSet<int>> mappingLookup = mappings
            .GroupBy(m => (m.CollectionType, m.Id))
            .ToDictionary(
                g => g.Key,
                g => g.Select(m => m.ExpansionId).ToHashSet()
            );

        HashSet<int> allExpansionIds = [.. mappings.Select(m => m.ExpansionId)];
        List<string> lines = [];

        foreach (KeyValuePair<char, IQueryable<int>> kvp in collectionTypes)
        {
            char type = kvp.Key;
            List<int> allIds = [.. kvp.Value];

            Dictionary<int, List<int>> expansionIdToIds = [];
            List<int> unmappedIds = [];

            foreach (int id in allIds)
            {
                if (mappingLookup.TryGetValue((type, id), out HashSet<int>? expansions) && expansions.Count > 0)
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
                lines.Add(line);
            }

            if (unmappedIds.Count > 0)
            {
                List<string> encoded = Base90.Encode(unmappedIds, 3);
                string line = $"{type}|-1|{string.Concat(encoded)}";
                lines.Add(line);
            }
        }

        Directory.CreateDirectory(@"C:\Applications\Warbound\cached");
        File.WriteAllLines(@"C:\Applications\Warbound\cached\CollectionsOverall.data", lines);
    }
}
