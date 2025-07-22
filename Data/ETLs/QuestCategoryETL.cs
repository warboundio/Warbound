using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Endpoints;
using Data.BlizzardAPI.Enums;
using Microsoft.EntityFrameworkCore;

namespace Data.ETLs;

public class QuestCategoryETL : RunnableBlizzardETL
{
    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<QuestCategoryETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        List<QuestCategory> questCategoriesToProcess = await Context.QuestCategories
            .Where(x => x.Status == ETLStateType.NEEDS_ENRICHED)
            .ToListAsync();

        return [.. questCategoriesToProcess.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        QuestCategory questCategory = (QuestCategory)item;
        QuestCategoryEndpoint endpoint = new(questCategory.Id);
        QuestCategory enriched = await endpoint.GetAsync();

        questCategory.Name = enriched.Name;
        questCategory.Status = ETLStateType.COMPLETE;
        questCategory.LastUpdatedUtc = System.DateTime.UtcNow;

        SaveBuffer.Add(questCategory);
    }
}
