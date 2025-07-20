#pragma warning disable CS8600

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Core.Settings;

namespace Core.GitHub;

public sealed class GitHubPullRequestInspector : IDisposable
{
    private readonly HttpClient _client;
    private const string OWNER = "warboundio";
    private const string REPO = "warbound";
    private const string YOUR_USERNAME = "warboundio";

    // Cache: branch name → (PR number, PR url)
    private readonly Dictionary<string, (int Number, string Url)> _prMetadataCache = [];

    public GitHubPullRequestInspector()
    {
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApplicationSettings.Instance.GithubClassicPAT);
        _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("WarboundApp", "1.0"));
    }

    public async Task<PullRequestStatus> GetPullRequestStatusAsync(int issueNumber)
    {
        string branch = $"copilot/fix-{issueNumber}";
        if (_prMetadataCache.TryGetValue(branch, out (int Number, string Url) cachedMeta)) { return await GetLivePullRequestStatusAsync(cachedMeta.Number, cachedMeta.Url); }

        string query = $@"
            query {{
              search(query: ""repo:{OWNER}/{REPO} is:pr head:{branch}"", type: ISSUE, first: 1) {{
                nodes {{
                  ... on PullRequest {{
                    number
                    url
                    state
                    isDraft
                    mergeable
                    reviewDecision
                    reviewRequests(first: 10) {{
                      nodes {{
                        requestedReviewer {{
                          ... on User {{ login }}
                          ... on Team {{ name }}
                        }}
                      }}
                    }}
                  }}
                }}
              }}
            }}";

        string json = await RunGraphQLAsync(query);
        JsonNode? pr = JsonNode.Parse(json)?["data"]?["search"]?["nodes"]?.AsArray()?.FirstOrDefault();
        if (pr == null) { return new PullRequestStatus { Exists = false }; }

        int prNumber = pr["number"]!.GetValue<int>();
        string url = pr["url"]!.ToString();

        // Cache PR metadata
        _prMetadataCache[branch] = (prNumber, url);

        return await GetLivePullRequestStatusAsync(prNumber, url);
    }

    private async Task<PullRequestStatus> GetLivePullRequestStatusAsync(int prNumber, string url)
    {
        string query = $@"
            query {{
              repository(owner: ""{OWNER}"", name: ""{REPO}"") {{
                pullRequest(number: {prNumber}) {{
                  state
                  isDraft
                  mergeable
                  reviewDecision
                  reviewRequests(first: 10) {{
                    nodes {{
                      requestedReviewer {{
                        ... on User {{ login }}
                        ... on Team {{ name }}
                      }}
                    }}
                  }}
                }}
              }}
            }}";

        string json = await RunGraphQLAsync(query);
        JsonNode? pr = JsonNode.Parse(json)?["data"]?["repository"]?["pullRequest"];
        if (pr == null) { return new PullRequestStatus { Exists = false }; }

        string state = pr["state"]!.ToString();
        string mergeable = pr["mergeable"]!.ToString();
        string reviewDecision = pr["reviewDecision"]?.ToString() ?? "REVIEW_REQUIRED";

        JsonArray? reviewers = pr["reviewRequests"]?["nodes"]?.AsArray();
        bool waitingOnYou = false;
        if (reviewers != null)
        {
            foreach (JsonNode reviewerNode in reviewers ?? [])
            {
                JsonNode? requestedReviewer = reviewerNode?["requestedReviewer"];
                string? login = requestedReviewer?["login"]?.ToString();
                string? name = requestedReviewer?["name"]?.ToString();
                if (login == YOUR_USERNAME || name == YOUR_USERNAME)
                {
                    waitingOnYou = true;
                    break;
                }
            }
        }

        return new PullRequestStatus
        {
            Exists = true,
            Url = url,
            IsOpen = state == "OPEN",
            IsMergeable = mergeable == "MERGEABLE",
            IsApproved = reviewDecision == "APPROVED",
            WaitingForYou = waitingOnYou || reviewDecision == "REVIEW_REQUIRED"
        };
    }

    private async Task<string> RunGraphQLAsync(string query)
    {
        var body = new { query };
        using StringContent content = new(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _client.PostAsync("https://api.github.com/graphql", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public void Dispose() => _client.Dispose();
}

public class PullRequestStatus
{
    public bool Exists { get; set; }
    public bool IsOpen { get; set; }
    public bool IsMergeable { get; set; }
    public bool IsApproved { get; set; }
    public bool WaitingForYou { get; set; }
    public string Url { get; set; } = string.Empty;
}
