using Core.ETL;
using Core.Logs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Core.GitHub;

public sealed class GitHubIssueMonitor : BackgroundService, IDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Dictionary<int, DateTime> _issueTracker = [];
    private Timer? _monitoringTimer;
    private const int INITIAL_DELAY_MINUTES = 5;
    private const int CHECK_INTERVAL_SECONDS = 30;
    private bool _disposed;

    public GitHubIssueMonitor(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await LoadOutstandingIssuesAsync();
        
        _monitoringTimer = new Timer(
            CheckIssuesCallback,
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(CHECK_INTERVAL_SECONDS)
        );

        // Keep the service running until cancellation is requested
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async void CheckIssuesCallback(object? state)
    {
        try
        {
            await CheckAllIssuesAsync();
        }
        catch (Exception ex)
        {
            Logging.Error(nameof(GitHubIssueMonitor), $"Error during issue check cycle: {ex.Message}", ex);
        }
    }

    private async Task LoadOutstandingIssuesAsync()
    {
        try
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            using CoreContext context = scope.ServiceProvider.GetRequiredService<CoreContext>();
            
            List<GitHubIssue> issues = await Task.Run(() => context.GitHubIssues.ToList());
            
            foreach (GitHubIssue issue in issues)
            {
                _issueTracker[issue.IssueId] = issue.CreatedAt;
            }
            
            Logging.Info(nameof(GitHubIssueMonitor), $"Loaded {issues.Count} outstanding issues for monitoring");
        }
        catch (Exception ex)
        {
            Logging.Error(nameof(GitHubIssueMonitor), $"Failed to load outstanding issues: {ex.Message}", ex);
        }
    }

    private async Task CheckAllIssuesAsync()
    {
        if (_issueTracker.Count == 0)
        {
            return;
        }

        List<int> issuesToRemove = [];
        DateTime now = DateTime.UtcNow;

        foreach (KeyValuePair<int, DateTime> issueEntry in _issueTracker.ToList())
        {
            int issueId = issueEntry.Key;
            DateTime createdAt = issueEntry.Value;

            // Skip if not past initial delay
            if (now.Subtract(createdAt).TotalMinutes < INITIAL_DELAY_MINUTES)
            {
                continue;
            }

            try
            {
                PullRequestStatus prStatus = await GitHubIssueService.GetPullRequestStatusAsync(issueId);

                if (!prStatus.Exists || !prStatus.IsOpen)
                {
                    // PR doesn't exist or is closed/merged - remove from database and tracking
                    await RemoveIssueAsync(issueId);
                    issuesToRemove.Add(issueId);
                    Logging.Info(nameof(GitHubIssueMonitor), $"Removed completed issue #{issueId} from monitoring");
                }
                else if (prStatus.WaitingForYou)
                {
                    // PR is waiting for developer action - update database and stop monitoring
                    await UpdateIssueStatusAsync(issueId, true);
                    issuesToRemove.Add(issueId);
                    Logging.Info(nameof(GitHubIssueMonitor), $"Issue #{issueId} is waiting for developer action, stopped monitoring");
                }
                // If PR exists, is open, and not waiting for developer, continue monitoring
            }
            catch (Exception ex)
            {
                Logging.Error(nameof(GitHubIssueMonitor), $"Error checking issue #{issueId}: {ex.Message}", ex);
            }
        }

        // Remove processed issues from tracking
        foreach (int issueId in issuesToRemove)
        {
            _issueTracker.Remove(issueId);
        }
    }

    public async Task AddIssueAsync(int issueId, string name)
    {
        try
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            using CoreContext context = scope.ServiceProvider.GetRequiredService<CoreContext>();
            
            GitHubIssue issue = new()
            {
                IssueId = issueId,
                Name = name,
                CreatedAt = DateTime.UtcNow,
                WaitingForYou = false
            };

            context.GitHubIssues.Add(issue);
            await context.SaveChangesAsync();
            
            _issueTracker[issueId] = issue.CreatedAt;
            
            Logging.Info(nameof(GitHubIssueMonitor), $"Added issue #{issueId} to monitoring: {name}");
        }
        catch (Exception ex)
        {
            Logging.Error(nameof(GitHubIssueMonitor), $"Failed to add issue #{issueId} to monitoring: {ex.Message}", ex);
        }
    }

    public async Task<List<GitHubIssue>> GetActiveWorkflowsAsync()
    {
        try
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            using CoreContext context = scope.ServiceProvider.GetRequiredService<CoreContext>();
            
            // Get all issues that are either in the tracker (running) or waiting for developer
            List<int> activeIssueIds = _issueTracker.Keys.ToList();
            List<GitHubIssue> waitingIssues = await Task.Run(() => 
                context.GitHubIssues.Where(i => i.WaitingForYou).ToList());
            
            List<GitHubIssue> runningIssues = await Task.Run(() =>
                context.GitHubIssues.Where(i => activeIssueIds.Contains(i.IssueId)).ToList());
            
            return runningIssues.Concat(waitingIssues).ToList();
        }
        catch (Exception ex)
        {
            Logging.Error(nameof(GitHubIssueMonitor), $"Failed to get active workflows: {ex.Message}", ex);
            return new List<GitHubIssue>();
        }
    }

    private async Task UpdateIssueStatusAsync(int issueId, bool waitingForYou)
    {
        try
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            using CoreContext context = scope.ServiceProvider.GetRequiredService<CoreContext>();
            
            GitHubIssue? issue = await Task.Run(() => context.GitHubIssues.FirstOrDefault(i => i.IssueId == issueId));
            if (issue != null)
            {
                issue.WaitingForYou = waitingForYou;
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Logging.Error(nameof(GitHubIssueMonitor), $"Failed to update issue #{issueId} status: {ex.Message}", ex);
        }
    }

    private async Task RemoveIssueAsync(int issueId)
    {
        try
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            using CoreContext context = scope.ServiceProvider.GetRequiredService<CoreContext>();
            
            GitHubIssue? issue = await Task.Run(() => context.GitHubIssues.FirstOrDefault(i => i.IssueId == issueId));
            if (issue != null)
            {
                context.GitHubIssues.Remove(issue);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Logging.Error(nameof(GitHubIssueMonitor), $"Failed to remove issue #{issueId}: {ex.Message}", ex);
        }
    }

    public override void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _monitoringTimer?.Dispose();
        _disposed = true;
        base.Dispose();
    }
}