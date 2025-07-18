using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Core.Settings;

namespace Core.GitHub;
public class GitHubIssueService
{
    public async Task Create(string title, string body)
    {
        const string owner = "JasonMCreighton";
        const string repo = "Warbound";

        using HttpClient client = new();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApplicationSettings.Instance.GithubToken);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("WarboundETL");

        var issue = new
        {
            title,
            body
        };

        string json = JsonSerializer.Serialize(issue);
        using StringContent content = new(json, Encoding.UTF8, "application/json");

        string url = $"https://api.github.com/repos/{owner}/{repo}/issues";
        using HttpResponseMessage response = await client.PostAsync(url, content);

        response.EnsureSuccessStatusCode();
    }
}
