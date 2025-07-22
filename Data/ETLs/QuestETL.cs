using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Endpoints;
using Data.BlizzardAPI.Enums;
using Microsoft.EntityFrameworkCore;

namespace Data.ETLs;

public class QuestETL : RunnableBlizzardETL
{
    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<QuestETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        List<Quest> questsToProcess = await Context.Quests.Where(x => x.Status == ETLStateType.NEEDS_ENRICHED).ToListAsync();
        return [.. questsToProcess.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        Quest quest = (Quest)item;

        QuestEndpoint endpoint = new(quest.Id);
        Quest enriched = await endpoint.GetAsync();

        quest.Name = enriched.Name;
        quest.QuestTypeId = enriched.QuestTypeId;
        quest.QuestIdentifier = enriched.QuestIdentifier;
        quest.QuestIdentifierId = enriched.QuestIdentifierId;
        quest.RewardItems = enriched.RewardItems;
        quest.Status = ETLStateType.COMPLETE;
        quest.LastUpdatedUtc = DateTime.UtcNow;

        SaveBuffer.Add(quest);
    }
}
