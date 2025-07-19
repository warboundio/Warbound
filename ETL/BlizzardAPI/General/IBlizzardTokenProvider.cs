using System.Threading.Tasks;

namespace ETL.BlizzardAPI.General;

/// <summary>
/// Interface for providing Blizzard OAuth tokens
/// </summary>
public interface IBlizzardTokenProvider
{
    /// <summary>
    /// Gets a valid access token, refreshing if necessary
    /// </summary>
    /// <returns>Valid access token</returns>
    string GetAccessToken();

    /// <summary>
    /// Gets a valid access token asynchronously, refreshing if necessary
    /// </summary>
    /// <returns>Valid access token</returns>
    Task<string> GetAccessTokenAsync();
}