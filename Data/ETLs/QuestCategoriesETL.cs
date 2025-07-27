using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Endpoints;
using Data.BlizzardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.ETLs;

public class QuestCategoriesETL : RunnableBlizzardETL
{
    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<QuestCategoriesETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        QuestCategoryIndexEndpoint endpoint = new();
        List<QuestCategory> indexQuestCategories = await endpoint.GetAsync();

        List<int> existingIds = await Context.QuestCategories.Select(q => q.Id).ToListAsync();

        List<QuestCategory> newQuestCategories = [.. indexQuestCategories.Where(q => !existingIds.Contains(q.Id))];
        return [.. newQuestCategories.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        await Task.Run(() =>
        {
            QuestCategory questCategory = (QuestCategory)item;
            SaveBuffer.Add(questCategory);
        });
    }
}
