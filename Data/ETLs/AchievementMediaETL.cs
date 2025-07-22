using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace Data.ETLs;

public class AchievementMediaETL : RunnableBlizzardETL
{
    private List<Achievement> _achievementsToProcess = [];

    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<AchievementMediaETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        List<int> existingAchievementMediaIds = await Context.AchievementMedias.Select(x => x.Id).ToListAsync();
        _achievementsToProcess = await Context.Achievements.Where(x => !existingAchievementMediaIds.Contains(x.Id)).ToListAsync();
        return [.. _achievementsToProcess.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        Achievement casted = (Achievement)item;

        AchievementMediaEndpoint endpoint = new(casted.Id);
        AchievementMedia result = await endpoint.GetAsync();

        SaveBuffer.Add(result);
    }
}
