# DUCA Admin Panel Drafts

## Draft: Implement Workflow Thread View
### Agent
In the previous draft we added monitoring to the issues and an object in the database. Now on the same panel that we create an issue, we need to create a live view that shows the state of a running workflow, including links to the associated GitHub issue and pull request, review status, and CI state. It should update to reflect the current state of the GitHubIssueMonitor. It should show a 'greyed' out if the issue is running with a spinner loader. It should show a green background with no loader when it's waiting on us. 

## Draft: Implement Draft Stubs
### Agent
Ideally every draft should be a 'stub' in the UI that can be selected. The developer will then add in the body additional details and requirements. Following that, a GitHub issue will be created to create a Plan for the draft with the supplemental information.