# Plan: Monitor Pull Requests

## Intent

Create a background monitoring system that continuously tracks GitHub issues created through the AdminPanel, monitors their associated pull request status, and automatically manages the database based on PR state changes. This system ensures that the local database accurately reflects the current state of development work and removes completed items automatically.

## Context

The AdminPanel currently creates GitHub issues and stores them in the database, but there's no mechanism to track their progress or clean up completed work. Issues remain in the database indefinitely, even after their corresponding pull requests are merged or closed. This creates data staleness and prevents accurate workflow visibility.

The existing infrastructure provides:
- GitHubIssue entity with tracking fields (IssueId, CreatedAt, Name, WaitingForYou)
- GitHubIssueService.GetPullRequestStatusAsync for status checking
- CoreContext with GitHubIssues DbSet for data persistence

## Scope

### In Scope

1. **GitHubIssueMonitor Class**
   - Background service implementing continuous monitoring logic
   - Timer-based checking with 5-minute initial delay and 30-second intervals
   - Database interaction for reading, updating, and deleting issue records
   - Integration with existing GitHubIssueService for PR status checks

2. **Monitoring Logic**
   - Load all outstanding issues from database on service startup
   - Implement 5-minute delay before first check of newly created issues
   - Check PR status every 30 seconds for issues past the initial delay
   - Stop monitoring when PR is waiting for developer action
   - Remove issues from database when PR is closed or doesn't exist

3. **Database Integration**
   - Modify GitHubIssueService.Create to add issues via monitor
   - Update WaitingForYou field based on PR status
   - Delete completed/closed issues automatically
   - Maintain single source of truth in database

4. **Program.cs Integration**
   - Register GitHubIssueMonitor as hosted service
   - Ensure proper startup sequencing and lifecycle management

5. **State Management**
   - Track monitoring state per issue (waiting, checking, stopped)
   - Proper disposal and cleanup

### Out of Scope

- Manual issue management UI
- Historical tracking of issue state changes
- Notification systems for state changes
- Multiple process/instance coordination
- Performance optimization for large issue volumes
- Custom scheduling beyond the specified intervals


## Technical Approach

### GitHubIssueMonitor Class Design

```csharp
// Core/GitHub/GitHubIssueMonitor.cs
public sealed class GitHubIssueMonitor : BackgroundService, IDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Dictionary<int, DateTime> _issueTracker;
    private readonly Timer _monitoringTimer;
    private const int INITIAL_DELAY_MINUTES = 5;
    private const int CHECK_INTERVAL_SECONDS = 30;
}
```

### Monitoring Flow

1. **Service Startup**:
   - Load all existing GitHubIssue records from database
   - Initialize tracking dictionary with issue IDs and creation times
   - Start background monitoring timer

2. **Per-Check Cycle**:
   - Filter issues that are past 5-minute initial delay
   - Call GitHubIssueService.GetPullRequestStatusAsync for each issue
   - Process PR status and update database accordingly
   - Remove completed issues from tracking

3. **Status Processing**:
   - **Waiting for developer** → Update WaitingForYou = true, stop monitoring
   - **Closed/Merged** → Delete from database and tracking
   - **Non-existent** → Delete from database and tracking
   - **Still open and not waiting** → Continue monitoring

### Database Operations

1. **Modified GitHubIssueService.Create**:
   - After creating GitHub issue, add to database via monitor
   - Monitor maintains single source of truth

2. **Monitor Database Methods**:
   - `LoadOutstandingIssuesAsync()` - Startup data loading
   - `UpdateIssueStatusAsync(int issueId, bool waitingForYou)` - Status updates
   - `RemoveIssueAsync(int issueId)` - Cleanup completed issues

### Program.cs Integration

```csharp
// Register as hosted service in Program.cs
builder.Services.AddHostedService<GitHubIssueMonitor>();
```

### Timer Implementation

- Use `System.Threading.Timer` for precise interval control
- 30-second intervals for continuous monitoring
- Calculate elapsed time since creation for 5-minute delay logic

## Edge Cases & Testing Needs

### Edge Cases
- If rate limiting or database connection issues occur - simply ignore them and try again on the next loop/check/30 seconds

### Error Handling
- Continue monitoring other issues if one fails
- If any errors occur during the 30 second check just call Logging.Error and move forward.

### Testing Strategy
- No testing is required

## Success Criteria

### Functional Requirements
- Monitor loads all existing issues on startup and begins tracking
- Issues created after 5-minute delay are checked every 30 seconds
- WaitingForYou field updated correctly when PR is waiting for developer
- Issues automatically removed when PR is closed/merged/non-existent
- New issues added to monitoring when created via GitHubIssueService.Create

### Technical Requirements
- Service starts and stops cleanly with application lifecycle
- No memory leaks from long-running monitoring operations
- Proper disposal of resources and timers
- All database operations complete successfully
- Error conditions logged appropriately without crashing service

### Data Integrity
- Database accurately reflects current state of tracked issues
- No orphaned records after PR completion
- Consistent state between GitHub and local database
- WaitingForYou field accurately indicates developer action needed

## Implementation Files

### New Files
- `Core/GitHub/GitHubIssueMonitor.cs` - Main monitoring service class

### Modified Files
- `Core/GitHub/GitHubIssueService.cs` - Update Create method to use monitor
- `AdminPanel/Program.cs` - Register hosted service
- `Core/GitHub/` - Potential new interfaces for testability

### Referenced Context
- `Core/GitHub/GitHubIssueService.cs` - Existing PR status checking
- `Core/ETL/GitHubIssue.cs` - Database entity structure
- `Core/CoreContext.cs` - Database context and DbSet
- `Core/GitHub/GitHubPullRequestInspector.cs` - PR status checking implementation
