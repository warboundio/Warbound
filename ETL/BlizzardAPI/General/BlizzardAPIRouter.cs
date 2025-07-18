using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Core.Extensions;
using Core.Logs;

namespace ETL.BlizzardAPI.General;

public static class BlizzardAPIRouter
{
    private static CancellationToken _token = CancellationToken.None;
    public static void SetCancellationToken(CancellationToken token) => _token = token;

    private static readonly HttpClient _httpClient = new();
    private static readonly ConcurrentQueue<Func<Task>> _retryQueue = new();
    private static readonly ConcurrentQueue<Func<Task>> _highPriorityQueue = new();
    private static readonly ConcurrentQueue<Func<Task>> _lowPriorityQueue = new();

    private static DateTime _backoffUntil = DateTime.MinValue;
    private static int _dripRate = 135;
    private static readonly TimeSpan _dispatchInterval = TimeSpan.FromMilliseconds(_dripRate);

    public const string FOLDER_PATH = @"C:\Applications\Warbound\blizzardapi\";

    static BlizzardAPIRouter()
    {
        _ = Task.Run(ProcessQueueAsync);
    }

    public static Task<JsonElement> GetJsonAsync(string url, bool forceLiveCall = false, PriorityLevel priority = PriorityLevel.LOW)
    {
        TaskCompletionSource<JsonElement> tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Enqueue(() => ExecuteJobAsync(url, tcs, forceLiveCall), priority);
        return tcs.Task;
    }

    public static Task<string> GetJsonRawAsync(string url, bool forceLiveCall = false, PriorityLevel priority = PriorityLevel.LOW)
    {
        TaskCompletionSource<string> tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Enqueue(() => ExecuteRawJobAsync(url, tcs, forceLiveCall), priority);
        return tcs.Task;
    }

    private static void Enqueue(Func<Task> job, PriorityLevel priority)
    {
        if (priority == PriorityLevel.HIGH) { _highPriorityQueue.Enqueue(job); }
        else { _lowPriorityQueue.Enqueue(job); }
    }

    private static async Task ExecuteJobAsync(string url, TaskCompletionSource<JsonElement> tcs, bool forceLiveCall)
    {
        JsonElement result = await InternalGetJsonAsync(url, forceLiveCall);
        tcs.SetResult(result);
    }

    private static async Task ExecuteRawJobAsync(string url, TaskCompletionSource<string> tcs, bool forceLiveCall)
    {
        string result = await InternalGetJsonRawAsync(url, forceLiveCall);
        tcs.SetResult(result);
    }

    private static async Task ProcessQueueAsync()
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

    private static async Task<JsonElement> InternalGetJsonAsync(string url, bool forceLiveCall)
    {
        string json = await InternalGetJsonRawAsync(url, forceLiveCall);
        return JsonSerializer.Deserialize<JsonElement>(json);
    }

    private static async Task<string> InternalGetJsonRawAsync(string url, bool forceLiveCall)
    {
        string hashFileName = url.Hash();
        string filePath = Path.Combine(FOLDER_PATH, $"{hashFileName}.json");

        if (!forceLiveCall && File.Exists(filePath))
        {
            return await File.ReadAllTextAsync(filePath, _token);
        }

        string token = BlizzardTokenProvider.Instance.GetAccessToken();

        HttpRequestMessage request = new(HttpMethod.Get, url);
        request.Headers.Authorization = new("Bearer", token);
        HttpResponseMessage? response = null;

        try
        {
            response = await _httpClient.SendAsync(request, _token);
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
