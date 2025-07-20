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
   - Handle different PR states (waiting, approved, closed, non-existent)
   - Proper disposal and cleanup

### Out of Scope

- Manual issue management UI
- Historical tracking of issue state changes
- Notification systems for state changes
- Multiple process/instance coordination
- Performance optimization for large issue volumes
- Custom scheduling beyond the specified intervals

## DUCA Constraints

### Database Operations
- **No automatic Entity Framework migrations**: Do not create migration files. Developers handle database changes manually.
- **Entity definition only**: Work with existing GitHubIssue entity structure.

### Code Design
- **Instance-based design**: Avoid static classes - GitHubIssueMonitor should be instantiated
- **Fail-fast philosophy**: Assume happy path, throw exceptions for errors
- **Use Core.Logs.Logging**: Never Console.WriteLine for logging

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
- GitHub API rate limiting or failures during status checks
- Database connection issues during monitoring cycles
- Service shutdown during active monitoring operations
- Issues created while service is starting up
- Multiple rapid issue creations

### Error Handling
- Graceful degradation when GitHub API is unavailable
- Retry logic for transient database failures
- Proper logging of all error conditions using Core.Logs.Logging
- Continue monitoring other issues if one fails

### Testing Strategy
- Unit tests for timing logic and status processing
- Unit tests for database operations with in-memory context
- Unit tests for PR status interpretation
- Integration tests for full monitoring cycle
- Mock GitHubIssueService for testing different PR states

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