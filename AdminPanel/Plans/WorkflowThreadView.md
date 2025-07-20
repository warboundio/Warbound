# Plan: Implement Workflow Thread View

## Intent

Create a live visual interface that displays the current state of DUCA workflow threads, providing real-time feedback on GitHub issue monitoring, pull request status, and workflow progression. This view enables the developer to quickly assess workflow health and take action when needed.

## Context

The AdminPanel currently provides a form to create DUCA workflows that result in GitHub issues, and the GitHubIssueMonitor tracks their progress in the background. However, there's no visual representation of active workflows or their current state. The developer needs immediate visibility into:

- Which workflows are currently running
- Which workflows are waiting for developer action
- Direct access to associated GitHub issues and pull requests
- Real-time status updates without page refresh

The existing infrastructure provides:
- GitHubIssue entity with tracking fields (IssueId, CreatedAt, Name, WaitingForYou)
- GitHubIssueMonitor background service continuously updating workflow status
- GitHubIssueService with PR status checking capabilities
- Duca.razor page where workflows are created

## Scope

### In Scope

1. **Workflow Thread Display Component**
   - Real-time list view of all active workflows from GitHubIssueMonitor
   - Visual status indicators for different workflow states
   - Direct links to GitHub issues and pull requests

2. **Visual State Management**
   - **Running State**: Greyed background with animated spinner loader
   - **Waiting State**: Green background with no loader
   - **Completed State**: Workflows automatically removed from view
   - Bootstrap-based styling consistent with existing AdminPanel design

3. **Real-time Updates**
   - Periodic refresh mechanism to reflect GitHubIssueMonitor changes
   - Update frequency (10-second cycles)
   - Smooth UI transitions between states

4. **User Interactions**
   - Click-to-navigate functionality to associated pull requests
   - Hover states and visual feedback for interactive elements
   - Clear indication when no active workflows exist

5. **GitHub Integration**
   - Retrieve pull request URLs from GitHub API
   - Display issue and PR numbers with direct links
   - Handle cases where PRs don't exist yet or are closed (no don't do this - this is why we have a 5 minute timer to wait to check)

6. **Layout Integration**
   - Seamless integration into existing Duca.razor page
   - Two-panel layout: creation form and active workflows
   - Proper spacing and visual hierarchy

### Out of Scope

- Historical workflow tracking or archive views
- Manual workflow state manipulation
- Detailed CI/CD pipeline status beyond PR state
- Workflow cancellation or intervention capabilities
- Advanced filtering or search functionality
- Custom refresh interval configuration
- Notification systems or alerts

## Technical Approach

### Component Architecture

```csharp
// New component: AdminPanel/Components/WorkflowThreadView.razor
@using Core.ETL
@using Core.GitHub
@rendermode InteractiveServer

<div class="workflow-threads">
    <h4>Active Workflows</h4>
    @if (activeWorkflows.Any())
    {
        @foreach (var workflow in activeWorkflows)
        {
            <div class="workflow-card @GetWorkflowCssClass(workflow)" 
                 @onclick="() => NavigateToPullRequest(workflow)">
                <!-- Workflow display content -->
            </div>
        }
    }
    else
    {
        <div class="no-workflows">
            <p>No active workflows</p>
        </div>
    }
</div>
```

### State Management Logic

```csharp
private List<GitHubIssue> activeWorkflows = new();
private Timer? refreshTimer;

protected override async Task OnInitializedAsync()
{
    await LoadActiveWorkflows();
    StartPeriodicRefresh();
}

private async Task LoadActiveWorkflows()
{
    using CoreContext db = new();
    activeWorkflows = await Task.Run(() => db.GitHubIssues.ToList());
    StateHasChanged();
}

private string GetWorkflowCssClass(GitHubIssue workflow)
{
    return workflow.WaitingForYou ? "workflow-waiting" : "workflow-running";
}
```

### Visual State CSS Classes

```css
.workflow-running {
    background-color: #f8f9fa;
    opacity: 0.7;
    /* Spinner animation styles */
}

.workflow-waiting {
    background-color: #d4edda;
    border-color: #c3e6cb;
}

.workflow-card {
    cursor: pointer;
    transition: all 0.2s ease-in-out;
}

.workflow-card:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 8px rgba(0,0,0,0.1);
}
```

### GitHub URL Resolution

```csharp
private async Task<string> GetPullRequestUrl(int issueId)
{
    try
    {
        PullRequestStatus prStatus = await GitHubIssueService.GetPullRequestStatusAsync(issueId);
        return prStatus.Exists ? prStatus.PullRequestUrl : $"https://github.com/warboundio/warbound/issues/{issueId}";
    }
    catch
    {
        return $"https://github.com/warboundio/warbound/issues/{issueId}";
    }
}

private async Task NavigateToPullRequest(GitHubIssue workflow)
{
    string url = await GetPullRequestUrl(workflow.IssueId);
    await JSRuntime.InvokeAsync("open", url, "_blank");
}
```

### Periodic Refresh Implementation

```csharp
private void StartPeriodicRefresh()
{
    refreshTimer = new Timer(async _ => 
    {
        await InvokeAsync(async () =>
        {
            await LoadActiveWorkflows();
        });
    }, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
}
```

### Duca.razor Integration

The existing Duca.razor page will be modified to include the WorkflowThreadView component:

```html
<div class="container">
    <div class="row">
        <div class="col-md-6">
            <!-- Existing workflow creation form -->
        </div>
        <div class="col-md-6">
            <WorkflowThreadView />
        </div>
    </div>
</div>
```

## Implementation Details

### Workflow Card Display Elements

Each workflow card will display:
- **Workflow Title**: Truncated for length with tooltip showing full text
- **Issue Number**: Linked to GitHub issue
- **Creation Time**: Relative time (e.g., "2 minutes ago")
- **Status Indicator**: Visual icon showing current state
- **Progress Spinner**: Animated loader for running workflows

### Error Handling

- Display of "Unable to load" states when appropriate

### Edge Cases

1. **No Active Workflows**: Display empty state message
2. **GitHub API Unavailable**: Fall back to issue links

## Success Criteria

### Functional Requirements

- Display all active workflows from GitHubIssueManager
- Show correct visual state based on WaitingForYou field
- Navigate to correct GitHub URLs when clicked
- Update automatically every 10 seconds
- Handle empty states gracefully

### User Experience Requirements

- Clear visual distinction between running and waiting states
- Smooth animations and transitions
- Responsive design working on different screen sizes
- Intuitive click behavior for navigation
- Loading states during initial load and refresh

### Technical Requirements

- Seamless integration with existing Duca.razor page
- Proper disposal of timers and resources
- Clean separation of concerns between components

### Visual Requirements

- Consistent with existing AdminPanel Bootstrap theme
- Professional appearance suitable for developer tools
- Clear typography and readable text
- Appropriate spacing and visual hierarchy
- Hover effects providing good user feedback

## Implementation Files

### New Files
- `AdminPanel/Components/WorkflowThreadView.razor` - Main workflow display component
- `AdminPanel/Components/WorkflowThreadView.razor.css` - Component-specific styles

### Modified Files
- `AdminPanel/Components/Pages/Admin/Duca.razor` - Integration of thread view component
- `AdminPanel/wwwroot/css/site.css` - Global styles for workflow cards (if needed)

### Referenced Context
- `Core/ETL/GitHubIssue.cs` - Data source for workflow information
- `Core/GitHub/GitHubIssueService.cs` - PR URL resolution
- `Core/GitHub/GitHubIssueMonitor.cs` - Background state management
- `Core/CoreContext.cs` - Database access for workflow data

## Layout and Visual Design

### Two-Panel Layout Structure

```
+------------------------+------------------------+
|                        |                        |
|   Workflow Creation    |   Active Workflows     |
|   Form                 |   Thread View          |
|                        |                        |
|   [Project Select]     |   ┌─ Workflow 1 ──┐    |
|   [Title Input]        |   │ 🔄 AdminPanel   │    |
|   [Description]        |   │ Issue #123      │    |
|   [Plan Required?]     |   │ 5 min ago       │    |
|   [Create Button]      |   └─────────────────┘    |
|                        |                        |
|   [Success/Error]      |   ┌─ Workflow 2 ──┐    |
|                        |   │ ✅ Data ETL     │    |
|                        |   │ Issue #124      │    |
|                        |   │ 12 min ago      │    |
|                        |   └─────────────────┘    |
+------------------------+------------------------+
```

### Visual State Indicators

- **Running**: Grey background, subtle pulse animation, spinner icon
- **Waiting**: Green background, checkmark icon, gentle highlight
- **Hover**: Slight elevation, shadow effect, color intensification
- **Loading**: Skeleton loading state during initial load/refresh

This plan provides a comprehensive foundation for implementing a polished, functional workflow thread view that enhances the DUCA AdminPanel's usability and provides essential visibility into workflow states.
