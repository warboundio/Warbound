using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Core.Settings;

namespace Core.GitHub;

public class GitHubCopilotAssigner : IDisposable
{
    private readonly HttpClient _client;
    private readonly string _token;
    private readonly string _owner;
    private readonly string _repo;
    private string? _copilotActorId;

    public GitHubCopilotAssigner()
    {
        _token = ApplicationSettings.Instance.GithubToken;
        _owner = "warboundio";
        _repo = "warbound";

        _client = new HttpClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("WarboundApp", "1.0"));
    }

    public async Task AssignToCopilotAsync(int issueNumber)
    {
        string issueId = await GetIssueNodeIdAsync(issueNumber);
        string copilotId = await GetCopilotActorIdAsync();

        string mutation = $@"
            mutation {{
              replaceActorsForAssignable(input: {{
                assignableId: ""{issueId}"",
                actorIds: [""{copilotId}""]
              }}) {{
                assignable {{
                  ... on Issue {{
                    id
                    title
                    assignees(first: 5) {{
                      nodes {{
                        login
                      }}
                    }}
                  }}
                }}
              }}
            }}";

        await RunGraphQLAsync(mutation);
    }

    private async Task<string> GetIssueNodeIdAsync(int issueNumber)
    {
        string query = $@"
            query {{
              repository(owner: ""{_owner}"", name: ""{_repo}"") {{
                issue(number: {issueNumber}) {{
                  id
                }}
              }}
            }}";

        string json = await RunGraphQLAsync(query);
        JsonNode? root = JsonNode.Parse(json);
        return root?["data"]?["repository"]?["issue"]?["id"]?.ToString() ?? throw new ArgumentException("Issue node ID not found.");
    }

    private async Task<string> GetCopilotActorIdAsync()
    {
        if (_copilotActorId != null) { return _copilotActorId; }

        string query = $@"
            query {{
              repository(owner: ""{_owner}"", name: ""{_repo}"") {{
                suggestedActors(capabilities: [CAN_BE_ASSIGNED], first: 50) {{
                  nodes {{
                    login
                    __typename
                    ... on Bot {{ id }}
                  }}
                }}
              }}
            }}";

        string json = await RunGraphQLAsync(query);
        JsonNode? root = JsonNode.Parse(json);
        JsonArray? nodes = root?["data"]?["repository"]?["suggestedActors"]?["nodes"]?.AsArray();

        foreach (JsonNode? actor in nodes!)
        {
            if (actor?["login"]?.ToString() == "copilot-swe-agent")
            {
                _copilotActorId = actor["id"]!.ToString();
                return _copilotActorId;
            }
        }

        throw new ArgumentException("Copilot actor ID not found.");
    }

    private async Task<string> RunGraphQLAsync(string query)
    {
        var body = new { query };
        StringContent content = new(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _client.PostAsync("https://api.github.com/graphql", content);

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public void Dispose() => _client?.Dispose();
}
