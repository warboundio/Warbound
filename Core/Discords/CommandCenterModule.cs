using Core.ETL;
using Core.GitHub;
using Core.Logs;
using Cronos;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace Core.Discords;

public class CommandCenterModule : ChannelHandler
{
    public const ulong COMMAND_CENTER_CHANNEL_ID = 1394682978164015255;
    private static CommandCenterModule? _instance;

    public CommandCenterModule() : base(COMMAND_CENTER_CHANNEL_ID)
    {
        _instance = this;
        RegisterCustomCommands();
    }

    private void RegisterCustomCommands()
    {
        CommandRegistry.Register("etl", "View ETL job status", HandleEtlCommand);
        CommandRegistry.Register("issue", "Report an issue", HandleCreateIssue);
        CommandRegistry.Register("run", "Manually run an ETL job", HandleRunCommand);
        CommandRegistry.Register("lockclear", "Clear all ETL job locks", HandleClearLocks);
        CommandRegistry.Register("clearandrun", "Clear lock and run an ETL job", HandleClearAndRunCommand);
    }

    protected override async Task HandleMessageInternalAsync(SocketMessage message) => await message.Channel.SendMessageAsync("📡 Command center received: " + message.Content);

    private async Task HandleCreateIssue(SocketMessage message, string[] args)
    {
        try
        {
            await GitHubIssueService.Create(args[0], string.Join(' ', args.Skip(1)), false);
            await message.Channel.SendMessageAsync($"Issue Successfully Created.");
        }
        catch (Exception ex)
        {
            await message.Channel.SendMessageAsync($"Error creating issue: {ex.Message}");
        }
    }

    private async Task HandleRunCommand(SocketMessage message, string[] args)
    {
        if (args.Length == 0)
        {
            await message.Channel.SendMessageAsync("❌ Usage: !run <ETL job name>");
            return;
        }

        string jobName = args[0];

        try
        {
            Logging.Info(nameof(CommandCenterModule), $"Manual run requested for job: {jobName}");

            bool lockAcquired = await ETLRunner.RunJobManuallyAsync(jobName);

            if (lockAcquired)
            {
                await message.Channel.SendMessageAsync($"✅ `{jobName}` kicked off");
                Logging.Info(nameof(CommandCenterModule), $"Manual run started for job: {jobName}");
            }
            else
            {
                await message.Channel.SendMessageAsync($"🔒 `{jobName}` is already locked or not found — skipping run");
                Logging.Warn(nameof(CommandCenterModule), $"Manual run skipped for job: {jobName} (locked or not found)");
            }
        }
        catch (Exception ex)
        {
            await message.Channel.SendMessageAsync($"❌ Error running `{jobName}`: {ex.Message}");
            Logging.Error(nameof(CommandCenterModule), $"Manual run failed for job: {jobName}", ex);
        }
    }

    private async Task HandleEtlCommand(SocketMessage message, string[] args)
    {
        try
        {
            using ETLContext db = new();
            List<ETLJob> jobs = await db.Jobs.ToListAsync();

            if (!jobs.Any())
            {
                await message.Channel.SendMessageAsync("No ETL jobs found.");
                return;
            }

            string statusMessage = FormatETLJobStatuses(jobs);
            await message.Channel.SendMessageAsync($"```\n{statusMessage}\n```");
        }
        catch (Exception ex)
        {
            await message.Channel.SendMessageAsync($"Error retrieving ETL status: {ex.Message}");
        }
    }

    private async Task HandleClearLocks(SocketMessage message, string[] args)
    {
        try
        {
            using ETLContext db = new();
            List<ETLJob> jobs = await db.Jobs.ToListAsync();

            foreach (ETLJob job in jobs)
            {
                job.LockOwner = null;
                job.LockAcquiredAt = null;
            }

            await db.SaveChangesAsync();

            await message.Channel.SendMessageAsync("✅ All ETL job locks have been cleared.");
            Logging.Info(nameof(CommandCenterModule), "All ETL job locks cleared via !lockclear command");
        }
        catch (Exception ex)
        {
            await message.Channel.SendMessageAsync($"❌ Error clearing ETL locks: {ex.Message}");
            Logging.Error(nameof(CommandCenterModule), "Failed to clear ETL locks", ex);
        }
    }

    private async Task HandleClearAndRunCommand(SocketMessage message, string[] args)
    {
        if (args.Length == 0)
        {
            await message.Channel.SendMessageAsync("❌ Usage: !clearandrun <ETL job name>");
            return;
        }

        string jobName = args[0];

        try
        {
            Logging.Info(nameof(CommandCenterModule), $"Clear and run requested for job: {jobName}");
            await message.Channel.SendMessageAsync($"🔓 Clearing lock for {jobName}...");

            using (ETLContext db = new())
            {
                ETLJob? job = await db.Jobs.FirstOrDefaultAsync(j => j.Name == jobName);

                if (job is null)
                {
                    await message.Channel.SendMessageAsync($"❌ Job `{jobName}` not found");
                    Logging.Warn(nameof(CommandCenterModule), $"Clear and run failed: job not found: {jobName}");
                    return;
                }

                job.LockOwner = null;
                job.LockAcquiredAt = null;
                await db.SaveChangesAsync();

                Logging.Info(nameof(CommandCenterModule), $"Lock cleared for job: {jobName}");
            }

            bool lockAcquired = await ETLRunner.RunJobManuallyAsync(jobName);
            if (!lockAcquired) { Logging.Warn(nameof(CommandCenterModule), $"Clear and run failed to acquire lock for job: {jobName}"); }
        }
        catch (Exception ex)
        {
            Logging.Error(nameof(CommandCenterModule), $"Clear and run failed for job: {jobName}", ex);
        }
    }

    private string FormatETLJobStatuses(List<ETLJob> jobs)
    {
        IEnumerable<string> statusLines = jobs.Select(job =>
        {
            string lastRunDelta = FormatLastRunDelta(job.LastSuccessAt);
            string successIndicator = job.WasLastRunSuccessful ? "✅" : "❌";
            string nextRunCountdown = FormatNextRunCountdown(job);

            return $"{job.Name}: Last run {lastRunDelta} {successIndicator} – next run {nextRunCountdown}";
        });

        return string.Join("\n", statusLines);
    }

    private string FormatLastRunDelta(DateTime? lastSuccessAt)
    {
        if (lastSuccessAt is null) { return "never"; }

        TimeSpan delta = DateTime.UtcNow - lastSuccessAt.Value;

        if (delta.TotalDays >= 1)
        {
            int days = (int)delta.TotalDays;
            return $"{days} day{(days == 1 ? "" : "s")} ago";
        }

        if (delta.TotalHours >= 1)
        {
            int hours = (int)delta.TotalHours;
            return $"{hours} hour{(hours == 1 ? "" : "s")} ago";
        }

        int minutes = Math.Max(1, (int)delta.TotalMinutes);
        return $"{minutes} minute{(minutes == 1 ? "" : "s")} ago";
    }

    private string FormatNextRunCountdown(ETLJob job)
    {
        if (string.IsNullOrWhiteSpace(job.CRONSchedule)) { return "no schedule"; }

        try
        {
            CronExpression expression = CronExpression.Parse(job.CRONSchedule);
            DateTime reference = job.LastSuccessAt ?? DateTime.UtcNow;
            DateTime? nextRun = expression.GetNextOccurrence(reference, TimeZoneInfo.Utc);

            if (nextRun is null) { return "no future runs"; }

            TimeSpan countdown = nextRun.Value - DateTime.UtcNow;

            if (countdown.TotalDays >= 1)
            {
                int days = (int)countdown.TotalDays;
                return $"in {days} day{(days == 1 ? "" : "s")}";
            }

            if (countdown.TotalHours >= 1)
            {
                int hours = (int)countdown.TotalHours;
                return $"in {hours} hour{(hours == 1 ? "" : "s")}";
            }

            if (countdown.TotalMinutes >= 1)
            {
                int minutes = (int)countdown.TotalMinutes;
                return $"in {minutes} minute{(minutes == 1 ? "" : "s")}";
            }

            return "now";
        }
        catch { return "invalid schedule"; }
    }

    public static new async Task SendLogAsync(string content)
    {
        if (_instance is not null)
        {
            await _instance.SendLogInternalAsync(content);
        }
    }

    private async Task SendLogInternalAsync(string content) => await base.SendLogAsync(content);
}
