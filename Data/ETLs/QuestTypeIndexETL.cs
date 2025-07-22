using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace Data.ETLs;

public class QuestTypeIndexETL : RunnableBlizzardETL
{
    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<QuestTypeIndexETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        QuestTypeIndexEndpoint endpoint = new();
        List<QuestType> indexQuestTypes = await endpoint.GetAsync();

        List<int> existingIds = await Context.QuestTypes.Select(q => q.Id).ToListAsync();

        List<QuestType> newQuestTypes = [.. indexQuestTypes.Where(q => !existingIds.Contains(q.Id))];
        return [.. newQuestTypes.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        await Task.Run(() =>
        {
            QuestType questType = (QuestType)item;
            SaveBuffer.Add(questType);
        });
    }
}
