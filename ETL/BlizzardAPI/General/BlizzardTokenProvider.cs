using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Logs;
using Core.Settings;

namespace ETL.BlizzardAPI.General;
public sealed class BlizzardTokenProvider
{
    private static readonly HttpClient _http = new();
    private static readonly object _lock = new();

    private static BlizzardTokenProvider? _instance;

    private string? _cachedToken;
    private DateTime _expiry;

    private BlizzardTokenProvider() { }

    public static BlizzardTokenProvider Instance
    {
        get
        {
            if (_instance == null) { lock (_lock) { _instance ??= new(); } }
            return _instance;
        }
    }

    public string GetAccessToken()
    {
        if (_cachedToken != null && DateTime.UtcNow < _expiry) { return _cachedToken; }

        lock (_lock)
        {
            return _cachedToken != null && DateTime.UtcNow < _expiry ? _cachedToken : RefreshTokenInternalAsync().GetAwaiter().GetResult();
        }
    }

    private async Task<string> RefreshTokenInternalAsync()
    {
        try
        {
            string authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ApplicationSettings.Instance.BattleNetClientId}:{ApplicationSettings.Instance.BattleNetSecretId}"));

            const string URL = "https://oauth.battle.net/token";
            HttpRequestMessage request = new(HttpMethod.Post, URL)
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" }
                })
            };

            request.Headers.Authorization = new("Basic", authHeader);

            HttpResponseMessage response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Blizzard token request failed: {response.StatusCode} - {error}");
            }

            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();
            JsonElement tokenData = JsonSerializer.Deserialize<JsonElement>(responseJson);

            _cachedToken = tokenData.GetProperty("access_token").GetString();
            int expiresIn = tokenData.GetProperty("expires_in").GetInt32();

            _expiry = DateTime.UtcNow.AddSeconds(expiresIn - 240);

            return _cachedToken!;
        }
        catch (Exception ex)
        {
            Logging.Error("BlizzardTokenProvider", "An unhandled exception has occurred.", ex);
            throw;
        }
    }
}
