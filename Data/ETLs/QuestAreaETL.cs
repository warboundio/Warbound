using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Endpoints;
using Data.BlizzardAPI.Enums;
using Microsoft.EntityFrameworkCore;

namespace Data.ETLs;

public class QuestAreaETL : RunnableBlizzardETL
{
    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<QuestAreaETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        List<int> areaIdsWithQuests = await Context.Quests
            .Where(q => q.QuestIdentifier == QuestIdentifier.AREA)
            .Select(q => q.QuestIdentifierId)
            .Distinct()
            .ToListAsync();

        List<QuestArea> questAreasToProcess = await Context.QuestAreas
            .Where(qa => !areaIdsWithQuests.Contains(qa.Id))
            .ToListAsync();

        return [.. questAreasToProcess.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        QuestArea questArea = (QuestArea)item;

        QuestAreaEndpoint endpoint = new(questArea.Id);
        string url = endpoint.BuildUrl();
        JsonElement json = await BlizzardAPI.General.BlizzardAPIRouter.GetJsonAsync(url, false);
        _ = json.GetProperty("id").GetInt32();
        string area = json.GetProperty("area").GetString()!;

        questArea.Name = area;
        questArea.Status = ETLStateType.COMPLETE;
        questArea.LastUpdatedUtc = DateTime.UtcNow;

        SaveBuffer.Add(questArea);

        if (json.TryGetProperty("quests", out JsonElement questsElement))
        {
            foreach (JsonElement questElement in questsElement.EnumerateArray())
            {
                int questId = questElement.GetProperty("id").GetInt32();
                string questName = questElement.GetProperty("name").GetString()!;

                Quest quest = new();
                quest.Id = questId;
                quest.Name = questName;
                quest.QuestIdentifier = QuestIdentifier.AREA;
                quest.QuestIdentifierId = questArea.Id;
                quest.Status = ETLStateType.NEEDS_ENRICHED;
                quest.LastUpdatedUtc = DateTime.UtcNow;

                SaveBuffer.Add(quest);
            }
        }
    }
}
