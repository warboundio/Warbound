using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using Castle.Components.DictionaryAdapter.Xml;
using Core.Logs;
using Core.Settings;
using Discord;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;
using Octokit;
using Octokit.Internal;
using static System.Net.WebRequestMethods;

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

        // {StatusCode: 401, ReasonPhrase: 'Unauthorized', Version: 1.1, Content: System.Net.Http.HttpConnectionResponseContent, Headers:
  //      {
  //      Date: Sat, 19 Jul 2025 18:55:12 GMT
  //        X - GitHub - Media - Type: github.v3; format = json
  //        X - RateLimit - Limit: 60
  //        X - RateLimit - Remaining: 58
  //        X - RateLimit - Reset: 1752954886
  //        X - RateLimit - Used: 2
  //        X - RateLimit - Resource: core
  //        Access - Control - Expose - Headers: ETag, Link, Location, Retry - After, X - GitHub - OTP, X - RateLimit - Limit, X - RateLimit - Remaining, X - RateLimit - Used, X - RateLimit - Resource, X - RateLimit - Reset, X - OAuth - Scopes, X - Accepted - OAuth - Scopes, X - Poll - Interval, X - GitHub - Media - Type, X - GitHub - SSO, X - GitHub - Request - Id, Deprecation, Sunset
  //        Access - Control - Allow - Origin: *
  //Strict - Transport - Security: max - age = 31536000; includeSubdomains; preload
  //        X - Frame - Options: deny
  //        X - Content - Type - Options: nosniff
  //        X - XSS - Protection: 0
  //        Referrer - Policy: origin - when - cross - origin, strict - origin - when - cross - origin
  //        Content - Security - Policy: default - src 'none'
  //        Vary: Accept - Encoding, Accept, X - Requested - With
  //        X - GitHub - Request - Id: C00F: 2BD6D: E22261: 1CDA0E8: 687BEA10
  //Server: github.com
  //        Content - Type: application / json; charset = utf - 8
  //        Content - Length: 95
  //      }
  //  }
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
