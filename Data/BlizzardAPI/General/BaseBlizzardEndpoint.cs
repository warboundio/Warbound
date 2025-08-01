using System;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Logs;

namespace Data.BlizzardAPI.General;

public abstract class BaseBlizzardEndpoint<T>
{
    public abstract string BuildUrl();
    public abstract T Parse(JsonElement json);

    public async Task<T> GetAsync(bool forceFreshPull = false)
    {
        string url = BuildUrl();
        JsonElement json = await BlizzardAPIRouter.GetJsonAsync(url, forceFreshPull);

        try { return Parse(json); }
        catch (Exception ex)
        {
            Logging.Error("BaseBlizzardEndpoint", $"Error parsing response from {url}", ex);
            throw;
        }
    }
}
