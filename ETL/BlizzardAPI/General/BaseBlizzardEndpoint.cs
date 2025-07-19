using System;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Logs;
using Core.Services;

namespace ETL.BlizzardAPI.General;

public abstract class BaseBlizzardEndpoint<T>
{
    public abstract string BuildUrl();
    public abstract T Parse(JsonElement json);

    public async Task<T> GetAsync()
    {
        string url = BuildUrl();
        var router = ServiceProvider.GetService<IBlizzardAPIRouter>();
        JsonElement json = await router.GetJsonAsync(url, false);

        try { return Parse(json); }
        catch (Exception ex)
        {
            Logging.Error("BaseBlizzardEndpoint", $"Error parsing response from {url}", ex);
            throw;
        }
    }
}
