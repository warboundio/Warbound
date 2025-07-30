using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.ETLs;

public class QuestRewardItemsETL : RunnableBlizzardETL
{
    private List<int> _newItemIds = [];

    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<QuestRewardItemsETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        HashSet<int> existingItemIds = await Context.Items.Select(i => i.Id).ToHashSetAsync();
        List<Quest> completeQuests = await Context.Quests.Where(q => q.Status == ETLStateType.COMPLETE).ToListAsync();
        HashSet<int> discoveredItemIds = [];

        foreach (Quest quest in completeQuests)
        {
            if (string.IsNullOrEmpty(quest.RewardItems))
            {
                continue;
            }

            string[] itemIds = quest.RewardItems.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (string itemIdString in itemIds)
            {
                if (int.TryParse(itemIdString, out int itemId))
                {
                    discoveredItemIds.Add(itemId);
                }
            }
        }

        _newItemIds = [.. discoveredItemIds.Where(id => !existingItemIds.Contains(id))];
        return [.. _newItemIds.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        await Task.Run(() =>
        {
            int id = (int)item;

            SaveBuffer.Add(new Item
            {
                Id = id,
                Status = ETLStateType.NEEDS_ENRICHED,
                LastUpdatedUtc = DateTime.UtcNow
            });
        });
    }
}
