using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Core.Logs;
using Core.Settings;

namespace Core.GitHub;

/// <summary>
/// Service for creating GitHub issues programmatically using GitHub REST API
/// </summary>
public static class GitHubIssueService
{
    private const string OWNER = "warboundio";
    private const string REPO = "warbound";
    private const string PROJECT_ID = "PVT_kwHODRaglc4A-QJy";
    private const string STATUS_FIELD_ID = "PVTSSF_lAHODRaglc4A-QJyzgxtltU";
    private const string COLUMN_ID = "47fc9ee4";

    /// <summary>
    /// Creates a GitHub issue with specified title and body, and attaches it to the project
    /// </summary>
    /// <param name="title">Issue title</param>
    /// <param name="body">Issue body content</param>
    /// <returns>GitHub issue number</returns>
    public static async Task<int> Create(string title, string body)
    {
        using HttpClient client = new();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApplicationSettings.Instance.GithubToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        client.DefaultRequestHeaders.UserAgent.ParseAdd("WarboundETL");

        // Create the issue
        var issue = new
        {
            title,
            body
        };

        string json = JsonSerializer.Serialize(issue);
        using StringContent content = new(json, Encoding.UTF8, "application/json");

        string url = $"https://api.github.com/repos/{OWNER}/{REPO}/issues";
        using HttpResponseMessage response = await client.PostAsync(url, content);

        response.EnsureSuccessStatusCode();
        
        string responseContent = await response.Content.ReadAsStringAsync();
        using JsonDocument document = JsonDocument.Parse(responseContent);
        JsonElement root = document.RootElement;
        
        int issueNumber = root.GetProperty("number").GetInt32();
        string issueUrl = root.GetProperty("html_url").GetString() ?? string.Empty;
        string issueId = root.GetProperty("node_id").GetString() ?? string.Empty;
        
        Logging.Info(nameof(GitHubIssueService), $"Successfully created GitHub issue #{issueNumber}: {issueUrl}");

        // Add issue to project
        await AddIssueToProject(client, issueId);
        
        return issueNumber;
    }

    private static async Task AddIssueToProject(HttpClient client, string issueId)
    {
        try
        {
            // Add item to project using GraphQL API
            var addToProjectMutation = new
            {
                query = """
                mutation($projectId: ID!, $contentId: ID!) {
                  addProjectV2ItemById(input: {projectId: $projectId, contentId: $contentId}) {
                    item {
                      id
                    }
                  }
                }
                """,
                variables = new
                {
                    projectId = PROJECT_ID,
                    contentId = issueId
                }
            };

            string mutationJson = JsonSerializer.Serialize(addToProjectMutation);
            using StringContent mutationContent = new(mutationJson, Encoding.UTF8, "application/json");

            using HttpResponseMessage mutationResponse = await client.PostAsync("https://api.github.com/graphql", mutationContent);
            mutationResponse.EnsureSuccessStatusCode();

            string mutationResponseContent = await mutationResponse.Content.ReadAsStringAsync();
            using JsonDocument mutationDocument = JsonDocument.Parse(mutationResponseContent);
            
            if (mutationDocument.RootElement.TryGetProperty("data", out JsonElement data) &&
                data.TryGetProperty("addProjectV2ItemById", out JsonElement addResult) &&
                addResult.TryGetProperty("item", out JsonElement item) &&
                item.TryGetProperty("id", out JsonElement itemId))
            {
                string projectItemId = itemId.GetString() ?? string.Empty;
                Logging.Info(nameof(GitHubIssueService), $"Successfully added issue to project with item ID: {projectItemId}");

                // Set status field and column
                await UpdateProjectItemStatus(client, projectItemId);
            }
            else
            {
                Logging.Info(nameof(GitHubIssueService), "Failed to add issue to project - no item ID returned");
            }
        }
        catch (Exception ex)
        {
            Logging.Error(nameof(GitHubIssueService), $"Failed to add issue to project: {ex.Message}", ex);
        }
    }

    private static async Task UpdateProjectItemStatus(HttpClient client, string projectItemId)
    {
        try
        {
            // Update project item status using GraphQL API
            var updateStatusMutation = new
            {
                query = """
                mutation($projectId: ID!, $itemId: ID!, $fieldId: ID!, $value: ProjectV2FieldValue!) {
                  updateProjectV2ItemFieldValue(input: {
                    projectId: $projectId
                    itemId: $itemId
                    fieldId: $fieldId
                    value: $value
                  }) {
                    projectV2Item {
                      id
                    }
                  }
                }
                """,
                variables = new
                {
                    projectId = PROJECT_ID,
                    itemId = projectItemId,
                    fieldId = STATUS_FIELD_ID,
                    value = new
                    {
                        singleSelectOptionId = COLUMN_ID
                    }
                }
            };

            string statusJson = JsonSerializer.Serialize(updateStatusMutation);
            using StringContent statusContent = new(statusJson, Encoding.UTF8, "application/json");

            using HttpResponseMessage statusResponse = await client.PostAsync("https://api.github.com/graphql", statusContent);
            statusResponse.EnsureSuccessStatusCode();

            Logging.Info(nameof(GitHubIssueService), $"Successfully updated project item status");
        }
        catch (Exception ex)
        {
            Logging.Error(nameof(GitHubIssueService), $"Failed to update project item status: {ex.Message}", ex);
        }
    }
}
