using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Models;

namespace Data.ETLs;

public class ItemExpansionETL : RunnableBlizzardETL
{
    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<ItemExpansionETL>(job);

    protected override Task<List<object>> GetItemsToProcessAsync()
    {
        var warcraftData = WarcraftData.Instance;

        var existingMappings = warcraftData.ItemExpansions.Keys;
        var allItems = warcraftData.Items.Keys;
        var itemsToProcess = allItems.Except(existingMappings).Cast<object>().ToList();

        return Task.FromResult(itemsToProcess);
    }

    protected override Task UpdateItemAsync(object item)
    {
        int itemId = (int)item;
        int expansionId = DetermineExpansionForItem(itemId);

        ItemExpansion itemExpansion = new()
        {
            ItemId = itemId,
            ExpansionId = expansionId,
        };

        SaveBuffer.Add(itemExpansion);
        return Task.CompletedTask;
    }

    private int DetermineExpansionForItem(int itemId)
    {
        var warcraftData = WarcraftData.Instance;

        var encounters = warcraftData.JournalEncounters.Values
            .Where(x => !string.IsNullOrEmpty(x.Items))
            .ToList();

        var matchingEncounters = encounters.Where(encounter =>
        {
            var itemIds = encounter.Items.Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.TryParse(x.Trim(), out int id) ? id : -1)
                .Where(x => x > 0);
            return itemIds.Contains(itemId);
        }).ToList();

        if (!matchingEncounters.Any())
            return -1;

        foreach (var encounter in matchingEncounters)
        {
            if (encounter.InstanceId <= 0)
                continue;

            var expansions = warcraftData.JournalExpansions.Values
                .Where(x => !string.IsNullOrEmpty(x.DungeonIds) || !string.IsNullOrEmpty(x.RaidIds))
                .ToList();

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
                return matchingExpansion.Id;
        }

        return -1;
    }
}
