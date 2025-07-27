using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.ETLs;

public class ItemExpansionETL : RunnableBlizzardETL
{
    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<ItemExpansionETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        // Get all items that don't have expansion mappings yet
        var existingMappings = await Context.ItemExpansions.Select(x => x.ItemId).ToListAsync();
        var allItems = await Context.Items.Select(x => x.Id).ToListAsync();
        var itemsToProcess = allItems.Except(existingMappings).ToList();
        
        return itemsToProcess.Cast<object>().ToList();
    }

    protected override async Task UpdateItemAsync(object item)
    {
        int itemId = (int)item;
        int expansionId = await DetermineExpansionForItemAsync(itemId);
        
        ItemExpansion itemExpansion = new()
        {
            ItemId = itemId,
            ExpansionId = expansionId,
            LastUpdatedUtc = DateTime.UtcNow
        };

        SaveBuffer.Add(itemExpansion);
    }

    private async Task<int> DetermineExpansionForItemAsync(int itemId)
    {
        // Find encounters that contain this item
        var encounters = await Context.JournalEncounters
            .Where(x => !string.IsNullOrEmpty(x.Items))
            .ToListAsync();

        var matchingEncounters = encounters.Where(encounter =>
        {
            var itemIds = encounter.Items.Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.TryParse(x.Trim(), out int id) ? id : -1)
                .Where(x => x > 0);
            return itemIds.Contains(itemId);
        }).ToList();

        if (!matchingEncounters.Any())
        {
            return -1; // Item not found in any encounter
        }

        // For each encounter, find which expansion it belongs to
        foreach (var encounter in matchingEncounters)
        {
            if (encounter.InstanceId <= 0) continue;

            var expansions = await Context.JournalExpansions
                .Where(x => !string.IsNullOrEmpty(x.DungeonIds) || !string.IsNullOrEmpty(x.RaidIds))
                .ToListAsync();

            var matchingExpansion = expansions.FirstOrDefault(expansion =>
            {
                var dungeonIds = expansion.DungeonIds.Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => int.TryParse(x.Trim(), out int id) ? id : -1)
                    .Where(x => x > 0);
                var raidIds = expansion.RaidIds.Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => int.TryParse(x.Trim(), out int id) ? id : -1)
                    .Where(x => x > 0);

                return dungeonIds.Contains(encounter.InstanceId) || raidIds.Contains(encounter.InstanceId);
            });

            if (matchingExpansion != null)
            {
                return matchingExpansion.Id;
            }
        }

        return -1; // No expansion mapping found
    }
}