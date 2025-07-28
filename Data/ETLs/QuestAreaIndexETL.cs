using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Endpoints;
using Data.BlizzardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.ETLs;

public class QuestAreaIndexETL : RunnableBlizzardETL
{
    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<QuestAreaIndexETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        QuestAreaIndexEndpoint endpoint = new();
        List<QuestArea> indexQuestAreas = await endpoint.GetAsync();

        List<int> existingIds = await Context.QuestAreas.Select(qa => qa.Id).ToListAsync();

        List<QuestArea> newQuestAreas = [.. indexQuestAreas.Where(qa => !existingIds.Contains(qa.Id))];
        return [.. newQuestAreas.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        await Task.Run(() =>
        {
            QuestArea questArea = (QuestArea)item;
            SaveBuffer.Add(questArea);
        });
    }
}
