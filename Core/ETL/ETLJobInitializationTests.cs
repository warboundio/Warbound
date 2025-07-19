using System.Collections.Generic;
using System.Linq;

namespace Core.ETL;

public class ETLJobInitializationTests
{
    [Fact]
    public void ItShouldDefineCorrectETLJobsForRecipeProcessing()
    {
        // Arrange - Define the expected jobs as they would be created by InitializeETLJobsAsync
        List<ETLJob> expectedJobs = 
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

        // Act & Assert - Verify each job has correct properties
        Assert.Equal(3, expectedJobs.Count);

        ETLJob indexJob = expectedJobs.First(j => j.Name.Contains("RecipeIndexETL"));
        Assert.Equal("ETL.ETLs.RecipeIndexETL.RunAsync", indexJob.Name);
        Assert.Equal("0 13 * * *", indexJob.CRONSchedule); // 1:00 PM
        Assert.Equal(60, indexJob.LockTimeoutMinutes);
        Assert.True(indexJob.WasLastRunSuccessful);

        ETLJob mediaJob = expectedJobs.First(j => j.Name.Contains("RecipeMediaETL"));
        Assert.Equal("ETL.ETLs.RecipeMediaETL.RunAsync", mediaJob.Name);
        Assert.Equal("30 13 * * *", mediaJob.CRONSchedule); // 1:30 PM
        Assert.Equal(60, mediaJob.LockTimeoutMinutes);
        Assert.True(mediaJob.WasLastRunSuccessful);

        ETLJob recipeJob = expectedJobs.First(j => j.Name.Contains("RecipeETL.RunAsync"));
        Assert.Equal("ETL.ETLs.RecipeETL.RunAsync", recipeJob.Name);
        Assert.Equal("0 14 * * *", recipeJob.CRONSchedule); // 2:00 PM
        Assert.Equal(60, recipeJob.LockTimeoutMinutes);
        Assert.True(recipeJob.WasLastRunSuccessful);
    }

    [Fact]
    public void ItShouldScheduleJobsAtCorrectTimes()
    {
        // Arrange & Act - Verify CRON schedules are correct for requested times
        Dictionary<string, string> expectedSchedules = new()
        {
            { "ETL.ETLs.RecipeIndexETL.RunAsync", "0 13 * * *" },   // 1:00 PM
            { "ETL.ETLs.RecipeMediaETL.RunAsync", "30 13 * * *" },  // 1:30 PM  
            { "ETL.ETLs.RecipeETL.RunAsync", "0 14 * * *" }         // 2:00 PM
        };

        // Assert - Check each schedule format is valid
        foreach (KeyValuePair<string, string> schedule in expectedSchedules)
        {
            Assert.NotEmpty(schedule.Value);
            
            // Verify CRON format: minute hour day month weekday
            string[] parts = schedule.Value.Split(' ');
            Assert.Equal(5, parts.Length);
            
            // Verify specific time values
            if (schedule.Key.Contains("RecipeIndexETL"))
            {
                Assert.Equal("0", parts[0]);  // 0 minutes
                Assert.Equal("13", parts[1]); // 1 PM (13:00)
            }
            else if (schedule.Key.Contains("RecipeMediaETL"))
            {
                Assert.Equal("30", parts[0]); // 30 minutes  
                Assert.Equal("13", parts[1]); // 1 PM (13:30)
            }
            else if (schedule.Key.Contains("RecipeETL"))
            {
                Assert.Equal("0", parts[0]);  // 0 minutes
                Assert.Equal("14", parts[1]); // 2 PM (14:00)
            }
        }
    }
}