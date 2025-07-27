using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Endpoints;
using Data.BlizzardAPI.Enums;
using Data.BlizzardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.ETLs;

public class QuestDiscoveryETL : RunnableBlizzardETL
{
    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<QuestDiscoveryETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        List<QuestDiscoveryRequest> questDiscoveryRequests = [];
        List<int> existingQuestIds = await Context.Quests.Select(q => q.Id).ToListAsync();

        List<QuestCategory> questCategories = await Context.QuestCategories.ToListAsync();
        foreach (QuestCategory category in questCategories)
        {
            QuestCategoryEndpoint categoryEndpoint = new(category.Id);
            List<int> questIds = await categoryEndpoint.GetAsync();

            foreach (int questId in questIds.Where(id => !existingQuestIds.Contains(id)))
            {
                questDiscoveryRequests.Add(new QuestDiscoveryRequest
                {
                    QuestId = questId,
                    Identifier = QuestIdentifier.CATEGORY,
                    IdentifierId = category.Id
                });
            }
        }

        List<QuestArea> questAreas = await Context.QuestAreas.ToListAsync();
        foreach (QuestArea area in questAreas)
        {
            QuestAreaEndpoint areaEndpoint = new(area.Id);
            List<int> questIds = await areaEndpoint.GetAsync();

            foreach (int questId in questIds.Where(id => !existingQuestIds.Contains(id)))
            {
                questDiscoveryRequests.Add(new QuestDiscoveryRequest
                {
                    QuestId = questId,
                    Identifier = QuestIdentifier.AREA,
                    IdentifierId = area.Id
                });
            }
        }

        List<QuestDiscoveryRequest> uniqueRequests = [.. questDiscoveryRequests.GroupBy(r => r.QuestId).Select(g => g.First())];
        return [.. uniqueRequests.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        await Task.Run(() =>
        {
            QuestDiscoveryRequest request = (QuestDiscoveryRequest)item;
            Quest quest = new()
            {
                Id = request.QuestId,
                Name = string.Empty,
                QuestIdentifier = request.Identifier,
                QuestIdentifierId = request.IdentifierId,
                Status = ETLStateType.NEEDS_ENRICHED,
                LastUpdatedUtc = DateTime.UtcNow
            };
            SaveBuffer.Add(quest);
        });
    }

    private sealed class QuestDiscoveryRequest
    {
        public int QuestId { get; set; }
        public QuestIdentifier Identifier { get; set; }
        public int IdentifierId { get; set; }
    }
}
