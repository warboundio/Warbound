# DUCA Admin Panel Drafts


## Draft: Implement Core/GitHub/GitHubIssueService
### Agent
This class allows us to create and manage GitHub issues via the GitHub API. We're going to need an object to store in Entity Framework and track it's progress.

## Draft: Implement Workflow Thread View
### Agent
Create a live view that shows the state of a running workflow, including links to the associated GitHub issue and pull request, review status, and CI state. Add a polling mechanism to update the view every 30 seconds.

## Draft: Implement Draft Stubs
### Agent
Ideally every draft should be a 'stub' in the UI that can be selected. The developer will then add in the body additional details and requirements. Following that, a GitHub issue will be created to create a Plan for the draft with the supplemental information.