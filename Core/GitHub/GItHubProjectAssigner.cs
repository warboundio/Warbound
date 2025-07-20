using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Core.Settings;

namespace Core.GitHub;

public class GitHubProjectAssigner : IDisposable
{
    private readonly HttpClient _client;
    private readonly string _token;
    private readonly string _owner;
    private readonly string _repo;

    private readonly Dictionary<int, string> _projectIdCache = [];
    private readonly Dictionary<string, (string fieldId, string optionId)> _statusCache = [];
    private readonly Dictionary<string, string> _projectItemIdCache = [];

    public GitHubProjectAssigner()
    {
        _token = ApplicationSettings.Instance.GithubClassicPAT;
        _owner = "warboundio";
        _repo = "warbound";

        _client = new HttpClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("WarboundApp", "1.0"));
    }

    public async Task SetIssueStatusAsync(int issueNumber, int projectNumber, string statusName)
    {
        string issueNodeId = await GetIssueNodeIdAsync(issueNumber);
        string projectId = await GetCachedProjectIdAsync(projectNumber);
        string projectItemId = await GetCachedProjectItemIdAsync(projectId, issueNodeId);
        (string fieldId, string optionId) = await GetCachedStatusFieldOptionAsync(projectId, statusName);

        string mutation = $@"
            mutation {{
              updateProjectV2ItemFieldValue(input: {{
                projectId: ""{projectId}"",
                itemId: ""{projectItemId}"",
                fieldId: ""{fieldId}"",
                value: {{ singleSelectOptionId: ""{optionId}"" }}
              }}) {{
                projectV2Item {{ id }}
              }}
            }}";

        await RunGraphQLAsync(mutation);
    }

    private async Task<string> GetCachedProjectIdAsync(int projectNumber)
    {
        if (_projectIdCache.TryGetValue(projectNumber, out string? cached)) { return cached; }

        string query = $@"
            query {{
              user(login: ""{_owner}"") {{
                projectV2(number: {projectNumber}) {{
                  id
                }}
              }}
            }}";

        string json = await RunGraphQLAsync(query);
        string projectId = JsonNode.Parse(json)?["data"]?["user"]?["projectV2"]?["id"]?.ToString()
                           ?? throw new ArgumentException("Project V2 not found.");

        _projectIdCache[projectNumber] = projectId;
        return projectId;
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
        return JsonNode.Parse(json)?["data"]?["repository"]?["issue"]?["id"]?.ToString() ?? throw new ArgumentException("Issue node ID not found.");
    }

    private async Task<string> GetCachedProjectItemIdAsync(string projectId, string issueNodeId)
    {
        if (_projectItemIdCache.TryGetValue(issueNodeId, out string? cached)) { return cached; }

        string mutation = $@"
            mutation {{
              addProjectV2ItemById(input: {{
                projectId: ""{projectId}"",
                contentId: ""{issueNodeId}""
              }}) {{
                item {{ id }}
              }}
            }}";

        string json = await RunGraphQLAsync(mutation);
        string itemId = JsonNode.Parse(json)?["data"]?["addProjectV2ItemById"]?["item"]?["id"]?.ToString() ?? throw new ArgumentException("Unable to create/get project item.");

        _projectItemIdCache[issueNodeId] = itemId;
        return itemId;
    }

    private async Task<(string fieldId, string optionId)> GetCachedStatusFieldOptionAsync(string projectId, string statusName)
    {
        string cacheKey = $"{projectId}:{statusName}";
        if (_statusCache.TryGetValue(cacheKey, out (string fieldId, string optionId) cached)) { return cached; }

        string query = $@"
            query {{
              node(id: ""{projectId}"") {{
                ... on ProjectV2 {{
                  fields(first: 20) {{
                    nodes {{
                      ... on ProjectV2SingleSelectField {{
                        id
                        name
                        options {{
                          id
                          name
                        }}
                      }}
                    }}
                  }}
                }}
              }}
            }}";

        string json = await RunGraphQLAsync(query);
        JsonArray? fields = JsonNode.Parse(json)?["data"]?["node"]?["fields"]?["nodes"]?.AsArray();

        foreach (JsonNode? field in fields!)
        {
            if (field?["name"]?.ToString() == "Status")
            {
                JsonArray options = field["options"]!.AsArray();
                foreach (JsonNode? option in options)
                {
                    if (option?["name"]?.ToString() == statusName)
                    {
                        string fieldId = field["id"]!.ToString();
                        string optionId = option["id"]!.ToString();
                        _statusCache[cacheKey] = (fieldId, optionId);
                        return (fieldId, optionId);
                    }
                }
            }
        }

        throw new ArgumentException($"Status '{statusName}' not found in project.");
    }

    private async Task<string> RunGraphQLAsync(string query)
    {
        var body = new { query };
        StringContent content = new(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _client.PostAsync("https://api.github.com/graphql", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public void Dispose() => _client.Dispose();
}
