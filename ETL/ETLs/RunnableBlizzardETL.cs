using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.ETL;
using Core.Logs;
using EFCore.BulkExtensions;
using ETL.BlizzardAPI;

namespace ETL.ETLs;

public abstract class RunnableBlizzardETL : IAsyncDisposable
{
    protected BlizzardAPIContext Context = new();
    protected readonly ConcurrentBag<object> SaveBuffer = [];
    private List<object> _itemsToProcess = [];
    private int _processedCount;

    protected abstract Task<List<object>> GetItemsToProcessAsync();
    protected abstract Task UpdateItemAsync(object item);

    public async Task ProcessAsync()
    {
        _itemsToProcess = await GetItemsToProcessAsync();

        if (_itemsToProcess.Count == 0)
        {
            Logging.Info(GetType().Name, "No items to process.");
            return;
        }

        Logging.Info(GetType().Name, $"Queued {_itemsToProcess.Count} items for enrichment.");

        List<Task> queuedTasks = [];
        foreach (object item in _itemsToProcess) { queuedTasks.Add(WrapItemUpdateAsync(item)); }

        await Task.WhenAll(queuedTasks);

        Logging.Info(GetType().Name, $"All enrichment tasks completed. Processed {_itemsToProcess.Count} items.");
    }

    private async Task WrapItemUpdateAsync(object item)
    {
        try
        {
            await UpdateItemAsync(item);
        }
        catch (Exception ex)
        {
            Logging.Error(GetType().Name, $"Failed to enrich item {item}", ex);
        }
        finally
        {
            Interlocked.Increment(ref _processedCount);
        }
    }

    public static async Task RunAsync<T>(ETLJob? job = null) where T : RunnableBlizzardETL, new()
    {
        await using T instance = new();

        using CancellationTokenSource cancellationTokenSource = new();
        Task? heartbeatTask = null;
        Task? saveHeartbeatTask = null;

        if (job is not null)
        {
            heartbeatTask = KeepHeartbeatAliveAsync(job, cancellationTokenSource.Token);
        }

        saveHeartbeatTask = LogSaveHeartbeatAsync(instance, cancellationTokenSource.Token);

        try
        {
            await instance.ProcessAsync();
        }
        finally
        {
            cancellationTokenSource.Cancel();

            if (heartbeatTask is not null) { try { await heartbeatTask; } catch (OperationCanceledException) { } }

            if (saveHeartbeatTask is not null) { try { await saveHeartbeatTask; } catch (OperationCanceledException) { } }

            if (!instance.SaveBuffer.IsEmpty)
            {
                List<object> toFlush = [];

                while (instance.SaveBuffer.TryTake(out object? entity)) { toFlush.Add(entity); }

                if (toFlush.Count > 0)
                {
                    await instance.Context.BulkInsertOrUpdateAsync(toFlush);
                    await instance.Context.SaveChangesAsync();

                    Logging.Info(instance.GetType().Name, $"Final flush: saved {toFlush.Count} remaining rows.");
                }
            }
        }
    }

    private static async Task KeepHeartbeatAliveAsync(ETLJob job, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(30), cancellationToken);
                if (cancellationToken.IsCancellationRequested) { break; }

                await using ETLContext db = new();
                ETLJob? currentJob = await db.Jobs.FindAsync([job.Id], cancellationToken);

                if (currentJob is not null)
                {
                    currentJob.LockAcquiredAt = DateTime.UtcNow;
                    db.Update(currentJob);
                    await db.SaveChangesAsync(cancellationToken);

                    Logging.Info("ETLHeartbeat", $"Updated heartbeat for job: {currentJob.Name}");
                }
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                Logging.Error("ETLHeartbeat", "Error updating ETL job heartbeat", ex);
            }
        }
    }

    private static async Task LogSaveHeartbeatAsync(RunnableBlizzardETL instance, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
                if (cancellationToken.IsCancellationRequested) { break; }

                List<object> toSave = [];

                while (instance.SaveBuffer.TryTake(out object? entity)) { toSave.Add(entity); }

                if (toSave.Count > 0)
                {
                    await instance.Context.BulkInsertOrUpdateAsync(toSave);
                    instance.Context.SaveChanges();
                }

                int total = instance._itemsToProcess.Count;
                int processed = instance._processedCount;
                double percent = total == 0 ? 100.0 : Math.Round(100 * ((double)processed / total), 2);

                Logging.Info(instance.GetType().Name, $"Processed: {processed}/{total} ({percent}% complete)");
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                Logging.Error("ETLHeartbeat", "Error in save heartbeat loop", ex);
            }
        }
    }

    public ValueTask DisposeAsync() => Context.DisposeAsync();
}
