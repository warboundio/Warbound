using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ETL;
using Data.BlizzardAPI.Endpoints;
using Data.BlizzardAPI.Enums;
using Microsoft.EntityFrameworkCore;

namespace Data.ETLs;

public class AchievementETL : RunnableBlizzardETL
{
    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<AchievementETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        List<Achievement> achievementsToProcess = await Context.Achievements.Where(x => x.Status == ETLStateType.NEEDS_ENRICHED).ToListAsync();
        return [.. achievementsToProcess.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        Achievement achievement = (Achievement)item;

        AchievementEndpoint endpoint = new(achievement.Id);
        Achievement enriched = await endpoint.GetAsync();

        achievement.Description = enriched.Description;
        achievement.CategoryName = enriched.CategoryName;
        achievement.RewardDescription = enriched.RewardDescription;
        achievement.RewardItemId = enriched.RewardItemId;
        achievement.RewardItemName = enriched.RewardItemName;
        achievement.Points = enriched.Points;
        achievement.Icon = enriched.Icon;
        achievement.CriteriaIds = enriched.CriteriaIds;
        achievement.CriteriaTypes = enriched.CriteriaTypes;
        achievement.Status = ETLStateType.COMPLETE;
        achievement.LastUpdatedUtc = DateTime.UtcNow;

        SaveBuffer.Add(achievement);
    }
}
