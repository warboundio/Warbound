using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Core.Extensions;
using Core.Logs;
using Core.Services;

namespace ETL.BlizzardAPI.General;

public class BlizzardAPIRouterImplementation : IBlizzardAPIRouter
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IBlizzardTokenProvider _tokenProvider;
    private readonly ConcurrentQueue<Func<Task>> _retryQueue = new();
    private readonly ConcurrentQueue<Func<Task>> _highPriorityQueue = new();
    private readonly ConcurrentQueue<Func<Task>> _lowPriorityQueue = new();

    private CancellationToken _token = CancellationToken.None;
    private DateTime _backoffUntil = DateTime.MinValue;
    private int _dripRate = 135;
    private readonly TimeSpan _dispatchInterval;

    public const string FOLDER_PATH = @"C:\Applications\Warbound\blizzardapi\";

    public BlizzardAPIRouterImplementation(IHttpClientFactory httpClientFactory, IBlizzardTokenProvider tokenProvider)
    {
        _httpClientFactory = httpClientFactory;
        _tokenProvider = tokenProvider;
        _dispatchInterval = TimeSpan.FromMilliseconds(_dripRate);
        _ = Task.Run(ProcessQueueAsync);
    }

    public void SetCancellationToken(CancellationToken token) => _token = token;

    public Task<JsonElement> GetJsonAsync(string url, bool forceLiveCall = false, PriorityLevel priority = PriorityLevel.LOW)
    {
        TaskCompletionSource<JsonElement> tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Enqueue(() => ExecuteJobAsync(url, tcs, forceLiveCall), priority);
        return tcs.Task;
    }

    public Task<string> GetJsonRawAsync(string url, bool forceLiveCall = false, PriorityLevel priority = PriorityLevel.LOW)
    {
        TaskCompletionSource<string> tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Enqueue(() => ExecuteRawJobAsync(url, tcs, forceLiveCall), priority);
        return tcs.Task;
    }

    private void Enqueue(Func<Task> job, PriorityLevel priority)
    {
        if (priority == PriorityLevel.HIGH) { _highPriorityQueue.Enqueue(job); }
        else { _lowPriorityQueue.Enqueue(job); }
    }

    private async Task ExecuteJobAsync(string url, TaskCompletionSource<JsonElement> tcs, bool forceLiveCall)
    {
        JsonElement result = await InternalGetJsonAsync(url, forceLiveCall);
        tcs.SetResult(result);
    }

    private async Task ExecuteRawJobAsync(string url, TaskCompletionSource<string> tcs, bool forceLiveCall)
    {
        string result = await InternalGetJsonRawAsync(url, forceLiveCall);
        tcs.SetResult(result);
    }

    private async Task ProcessQueueAsync()
    {
        while (true)
        {
            while (DateTime.UtcNow < _backoffUntil) { await Task.Delay(1000, _token); }

            Func<Task>? job = null;

            if (_retryQueue.TryDequeue(out Func<Task>? retryJob)) { job = retryJob; }
            else if (_highPriorityQueue.TryDequeue(out Func<Task>? highJob)) { job = highJob; }
            else if (_lowPriorityQueue.TryDequeue(out Func<Task>? lowJob)) { job = lowJob; }

            if (job != null)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await job();
                    }
                    catch (HttpRequestException ex) when ((int?)ex.StatusCode == 429)
                    {
                        _backoffUntil = DateTime.UtcNow.AddMinutes(5);
                        _dripRate += 10;
                        Logging.Warn("BlizzardAPIRouter", $"BlizzardAPIRouter: Rate limited. Backing off. DripRate += 10 => {_dripRate}");
                    }
                    catch (Exception ex)
                    {
                        if (ex.IsRetryableNetworkError())
                        {
                            _retryQueue.Enqueue(job);
                            Logging.Info("BlizzardAPIRouter", "Retryable network error has occurred. Retrying.");
                        }
                        else
                        {
                            Logging.Error("BlizzardAPIRouter", "BlizzardAPIRouter: API call failed.", ex);
                        }
                    }
                });
            }

            await Task.Delay(_dispatchInterval, _token); // Enforce steady drip
        }
    }

    private async Task<JsonElement> InternalGetJsonAsync(string url, bool forceLiveCall)
    {
        string json = await InternalGetJsonRawAsync(url, forceLiveCall);
        return JsonSerializer.Deserialize<JsonElement>(json);
    }

    private async Task<string> InternalGetJsonRawAsync(string url, bool forceLiveCall)
    {
        string hashFileName = url.Hash();
        string filePath = Path.Combine(FOLDER_PATH, $"{hashFileName}.json");

        if (!forceLiveCall && File.Exists(filePath))
        {
            return await File.ReadAllTextAsync(filePath, _token);
        }

        string token = await _tokenProvider.GetAccessTokenAsync();

        using HttpClient client = _httpClientFactory.CreateClient("BlizzardAPI");
        HttpRequestMessage request = new(HttpMethod.Get, url);
        request.Headers.Authorization = new("Bearer", token);
        HttpResponseMessage? response = null;

        try
        {
            response = await client.SendAsync(request, _token);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            await File.WriteAllTextAsync(filePath, json, _token);
            return json;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            ex.Data["Response"] = response;
            throw;
        }
    }
}

/// <summary>
/// Static facade for BlizzardAPIRouter to maintain backward compatibility
/// </summary>
public static class BlizzardAPIRouter
{
    public const string FOLDER_PATH = BlizzardAPIRouterImplementation.FOLDER_PATH;

    public static void SetCancellationToken(CancellationToken token)
    {
        var router = ServiceProvider.GetServiceOrDefault<IBlizzardAPIRouter>();
        router?.SetCancellationToken(token);
    }

    public static Task<JsonElement> GetJsonAsync(string url, bool forceLiveCall = false, PriorityLevel priority = PriorityLevel.LOW)
    {
        var router = ServiceProvider.GetService<IBlizzardAPIRouter>();
        return router.GetJsonAsync(url, forceLiveCall, priority);
    }

    public static Task<string> GetJsonRawAsync(string url, bool forceLiveCall = false, PriorityLevel priority = PriorityLevel.LOW)
    {
        var router = ServiceProvider.GetService<IBlizzardAPIRouter>();
        return router.GetJsonRawAsync(url, forceLiveCall, priority);
    }
}
