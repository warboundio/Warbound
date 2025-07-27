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

public class AchievementsIndexETL : RunnableBlizzardETL
{
    public static async Task RunAsync(ETLJob? job = null) => await RunAsync<AchievementsIndexETL>(job);

    protected override async Task<List<object>> GetItemsToProcessAsync()
    {
        HashSet<int> existingIds = await Context.Achievements.Select(x => x.Id).ToHashSetAsync();

        AchievementIndexEndpoint endpoint = new();
        List<Achievement> achievements = await endpoint.GetAsync();

        List<Achievement> newAchievements = [.. achievements.Where(achievement => !existingIds.Contains(achievement.Id))];

        return [.. newAchievements.Cast<object>()];
    }

    protected override async Task UpdateItemAsync(object item)
    {
        await Task.Run(() =>
        {
            Achievement achievement = (Achievement)item;

            SaveBuffer.Add(new Achievement
            {
                Id = achievement.Id,
                Name = achievement.Name,
                Status = ETLStateType.NEEDS_ENRICHED,
                LastUpdatedUtc = DateTime.UtcNow
            });
        });
    }
}
