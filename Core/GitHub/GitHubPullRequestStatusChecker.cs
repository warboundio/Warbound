using System.Net.Http.Headers;
using System.Text.Json;
using Core.Settings;

namespace Core.GitHub;

public sealed class GitHubPullRequestStatusChecker : IDisposable
{
    private readonly HttpClient _client;
    private readonly Dictionary<int, int> _prNumberCache = [];
    private const string OWNER = "warboundio";
    private const string REPO = "warbound";

    public GitHubPullRequestStatusChecker()
    {
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApplicationSettings.Instance.GithubToken);
        _client.DefaultRequestHeaders.UserAgent.ParseAdd("WarboundAgent");
        _client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
    }

    public async Task<bool> IsPullRequestMergedAsync(int issueNumber)
    {
        int? prNumber = await GetPullRequestNumberAsync(issueNumber);
        if (prNumber == null) { return false; }

        HttpResponseMessage response = await _client.GetAsync($"https://api.github.com/repos/{OWNER}/{REPO}/pulls/{prNumber}/merge");
        return response.StatusCode == System.Net.HttpStatusCode.NoContent;
    }

    private async Task<int?> GetPullRequestNumberAsync(int issueNumber)
    {
        if (_prNumberCache.TryGetValue(issueNumber, out int cachedPrNumber)) { return cachedPrNumber; }

        string query = $@"
        query {{
          repository(owner: ""{OWNER}"", name: ""{REPO}"") {{
            pullRequests(first: 5, headRefName: ""copilot/fix-{issueNumber}"") {{
              nodes {{
                number
              }}
            }}
          }}
        }}";

        var requestBody = new { query };
        StringContent content = new(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _client.PostAsync("https://api.github.com/graphql", content);
        response.EnsureSuccessStatusCode();
        string json = await response.Content.ReadAsStringAsync();

        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;
        JsonElement? node = root
            .GetProperty("data")
            .GetProperty("repository")
            .GetProperty("pullRequests")
            .GetProperty("nodes")
            .EnumerateArray()
            .FirstOrDefault();

        if (node is not null && node.Value.TryGetProperty("number", out JsonElement numberElement))
        {
            int prNumber = numberElement.GetInt32();
            _prNumberCache[issueNumber] = prNumber;
            return prNumber;
        }

        return null;
    }

    public void Dispose() => _client.Dispose();
}
