# GitHub Issue Service

The `GitHubIssueService` provides functionality to programmatically create GitHub issues using the GitHub REST API and automatically attach them to the project.

## Features

- Creates GitHub issues via REST API using stored authentication token
- Automatically attaches issues to GitHub Projects v2
- Sets project status and column for created issues
- Proper error handling and logging
- Returns issue number for verification

## Requirements

- GitHub Personal Access Token (PAT) configured in `ApplicationSettings.GithubToken`
- Target repository: `warboundio/warbound`
- Token must have `repo` and `project` scopes for GitHub Projects v2 integration

## Usage

### Basic Usage

```csharp
using Core.GitHub;

// Create an issue with custom title and body
int issueNumber = await GitHubIssueService.Create("My Issue Title", "Issue description goes here");
Console.WriteLine($"Created issue #{issueNumber}");
```

The service automatically:
- Creates the GitHub issue
- Attaches it to project `PVT_kwHODRaglc4A-QJy`
- Sets the status field `PVTSSF_lAHODRaglc4A-QJyzgxtltU`
- Assigns column `47fc9ee4`

## Configuration

The service automatically reads the GitHub token from `ApplicationSettings.Instance.GithubToken`. 

Ensure your GitHub token has the following permissions:
- `repo` scope (for creating issues)
- `project` scope (for GitHub Projects v2 integration)

## Headers and Authentication

The service automatically includes the required headers:
- `Authorization: Bearer <GH_TOKEN>`
- `Accept: application/vnd.github+json`
- `User-Agent: WarboundETL`

## Logging

The service uses the Warbound logging infrastructure (`Core.Logs.Logging`) to log:
- Successful issue creation (with URL and issue number)
- Project attachment status
- Any errors that occur during the process

## API Endpoints

Issues are created via POST request to:
```
https://api.github.com/repos/warboundio/warbound/issues
```

Project integration uses GitHub GraphQL API:
```
https://api.github.com/graphql
```

## Return Value

The method returns an `int` representing the GitHub issue number that was created. This can be used to construct the full URL: `https://github.com/warboundio/warbound/issues/{issueNumber}`