using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Discords;
using Core.ETL;
using Core.Logs;
using Microsoft.EntityFrameworkCore;

Logging.Configure();

// Initialize ETL jobs
await InitializeETLJobsAsync();

_ = ETLRunner.RunLoopAsync();

DiscordBot bot = new();
_ = bot.StartAsync();

await Task.Delay(Timeout.Infinite);

static async Task InitializeETLJobsAsync()
{
    using ETLContext context = new();

    // Define the three ETL jobs to be scheduled
    List<ETLJob> requiredJobs =
    [
        new ETLJob
        {
            Name = "ETL.ETLs.RecipeIndexETL.RunAsync",
            CRONSchedule = "0 13 * * *", // 1:00 PM
            LockTimeoutMinutes = 60,
            WasLastRunSuccessful = true
        },
        new ETLJob
        {
            Name = "ETL.ETLs.RecipeMediaETL.RunAsync",
            CRONSchedule = "30 13 * * *", // 1:30 PM
            LockTimeoutMinutes = 60,
            WasLastRunSuccessful = true
        },
        new ETLJob
        {
            Name = "ETL.ETLs.RecipeETL.RunAsync",
            CRONSchedule = "0 14 * * *", // 2:00 PM
            LockTimeoutMinutes = 60,
            WasLastRunSuccessful = true
        }
    ];

    // Check which jobs already exist
    List<string> existingJobNames = await context.Jobs.Select(j => j.Name).ToListAsync();

    // Add only jobs that don't already exist
    foreach (ETLJob job in requiredJobs)
    {
        if (!existingJobNames.Contains(job.Name))
        {
            context.Jobs.Add(job);
            Logging.Info("ETL", $"Added ETL job: {job.Name} with schedule {job.CRONSchedule}");
        }
    }

    // Save changes if any new jobs were added
    int changes = await context.SaveChangesAsync();
    if (changes > 0)
    {
        Logging.Info("ETL", $"Successfully added {changes} new ETL jobs");
    }
}
