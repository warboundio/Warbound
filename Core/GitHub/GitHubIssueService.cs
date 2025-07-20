using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Core.Logs;
using Core.Settings;

namespace Core.GitHub;

internal sealed class GitHubIssueService : IDisposable
{
    public static async Task<int> Create(string title, string body, bool assignToCopilot, params string[] labels)
    {
        using GitHubIssueService service = new();
        int issueNumber = await service.CreateIssueAsync(title, body);
        await service.AddLabelsAsync(issueNumber, labels);
        await service._projectAssigner.SetIssueStatusAsync(issueNumber: issueNumber, projectNumber: PROJECT_NUMBER, statusName: STATUS_NAME);
        if (assignToCopilot) { await service.AssignToCopilotAsync(issueNumber); }
        return issueNumber;
    }

    private const int PROJECT_NUMBER = 1;
    private const string STATUS_NAME = "Agents in Progress";
    private const string OWNER = "warboundio";
    private const string REPO = "warbound";
    private readonly HttpClient _client;
    private readonly GitHubCopilotAssigner _copilotAssigner;
    private readonly GitHubProjectAssigner _projectAssigner = new();

    public GitHubIssueService()
    {
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApplicationSettings.Instance.GithubToken);
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        _client.DefaultRequestHeaders.UserAgent.ParseAdd("WarboundETL");
        _copilotAssigner = new GitHubCopilotAssigner();
    }

    public async Task<int> CreateIssueAsync(string title, string body)
    {
        object issue = new { title, body };
        string json = JsonSerializer.Serialize(issue);
        using StringContent content = new(json, Encoding.UTF8, "application/json");

        string url = $"https://api.github.com/repos/{OWNER}/{REPO}/issues";
        using HttpResponseMessage response = await _client.PostAsync(url, content);

        response.EnsureSuccessStatusCode();
        string responseContent = await response.Content.ReadAsStringAsync();
        using JsonDocument document = JsonDocument.Parse(responseContent);

        JsonElement root = document.RootElement;
        int issueNumber = root.GetProperty("number").GetInt32();
        string issueUrl = root.GetProperty("html_url").GetString() ?? string.Empty;
        Logging.Info(nameof(GitHubIssueService), $"Successfully created GitHub issue #{issueNumber}: {issueUrl}");
        return issueNumber;
    }

    public async Task AddLabelsAsync(int issueNumber, params string[] labels)
    {
        if (labels == null || labels.Length == 0)
        {
            return;
        }

        object labelObj = new { labels };
        string labelsJson = JsonSerializer.Serialize(labelObj);
        using StringContent labelContent = new(labelsJson, Encoding.UTF8, "application/json");

        string url = $"https://api.github.com/repos/{OWNER}/{REPO}/issues/{issueNumber}/labels";
        try
        {
            using HttpResponseMessage labelResponse = await _client.PostAsync(url, labelContent);

            labelResponse.EnsureSuccessStatusCode();
            Logging.Info(nameof(GitHubIssueService), $"Successfully added labels '{string.Join(", ", labels)}' to issue #{issueNumber}");
        }
        catch (Exception ex)
        {
            Logging.Error(nameof(GitHubIssueService), $"Failed to add labels to issue: {ex.Message}", ex);
        }
    }

    public async Task AssignToCopilotAsync(int issueNumber) => await _copilotAssigner.AssignToCopilotAsync(issueNumber);

    public void Dispose()
    {
        _client.Dispose();
        _copilotAssigner.Dispose();
    }
}
