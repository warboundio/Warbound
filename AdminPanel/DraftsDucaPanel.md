# DUCA Admin Panel Drafts

## Draft: Worklow After Adding a GitHub Issue
### Agent
After an issue has been created by hitting submit we need to clear the fields. Show a success message. And insert it into the database. Let's convert ETLContext -> CoreContext. Let's add an object for 'GithHubIssue' that follows the same schema as ETLJob and the table name should be github_issue. Let's have the Issue id. The DateTime Created. a string with max length for the name. I'm looking at the PullRequestStatus object. We also want 'WaitingForYou' to be stored (default false). When we first insert this we should only fill out the id (issue id), the datetime created, and the name. the other fields will be updated later.

## Draft: Monitor Pull Requests
### Agent
Now that we have objects in a database denoting outstanding github issues that are being monitored. We need to create an object to monitor them. Let's create Core/GitHub/GitHubIssueMonitor. It should be a class that follows these rules. 5 minutes after an issue has been created every 30 seconds it 'checks in' with GitHubIssueService.GetPullRequestStatusAsync. If the PR is waiting on us -> there is no reason to continue to check. If the PR is closed does not exist it can be deleted from the database. There should be a thread or task that is continually running and can be 'Booted' at like a Program.cs start. This thread will initially load in all outstanding issues, check to see if they're closed, delete the rows if need be, and then it starts it's 'continuous loop'. IT all should be async. The loop should be identifying which issues are past their 5 minute wait period, and then check in every 30 seconds. We do not need to concern ourselves with multiple processes adding in issues. This is to track issues that were added via the admin panel. So we need to ensure that when we use the GitHubIssueService and Create an issue, it's added to the database via this IssueMonitor.

## Draft: Implement Workflow Thread View
### Agent
In the previous draft we added monitoring to the issues and an object in the database. Now on the same panel that we create an issue, we need to create a live view that shows the state of a running workflow, including links to the associated GitHub issue and pull request, review status, and CI state. It should update to reflect the current state of the GitHubIssueMonitor. It should show a 'greyed' out if the issue is running with a spinner loader. It should show a green background with no loader when it's waiting on us. 

## Draft: Implement Draft Stubs
### Agent
Ideally every draft should be a 'stub' in the UI that can be selected. The developer will then add in the body additional details and requirements. Following that, a GitHub issue will be created to create a Plan for the draft with the supplemental information.