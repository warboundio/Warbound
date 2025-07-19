using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ETL.BlizzardAPI.General;

/// <summary>
/// Interface for routing Blizzard API requests
/// </summary>
public interface IBlizzardAPIRouter
{
    /// <summary>
    /// Gets JSON data from Blizzard API endpoint
    /// </summary>
    /// <param name="url">API endpoint URL</param>
    /// <param name="forceLiveCall">Force live API call instead of cache</param>
    /// <param name="priority">Request priority level</param>
    /// <returns>JSON element</returns>
    Task<JsonElement> GetJsonAsync(string url, bool forceLiveCall = false, PriorityLevel priority = PriorityLevel.LOW);

    /// <summary>
    /// Gets raw JSON string from Blizzard API endpoint
    /// </summary>
    /// <param name="url">API endpoint URL</param>
    /// <param name="forceLiveCall">Force live API call instead of cache</param>
    /// <param name="priority">Request priority level</param>
    /// <returns>Raw JSON string</returns>
    Task<string> GetJsonRawAsync(string url, bool forceLiveCall = false, PriorityLevel priority = PriorityLevel.LOW);

    /// <summary>
    /// Sets cancellation token for stopping the API router
    /// </summary>
    /// <param name="token">Cancellation token</param>
    void SetCancellationToken(CancellationToken token);
}