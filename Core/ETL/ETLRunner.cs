using System.Diagnostics;
using System.Reflection;
using Core.Logs;
using Cronos;
using Microsoft.EntityFrameworkCore;

namespace Core.ETL;

public static class ETLRunner
{
    private static readonly string _computerId = Guid.NewGuid().ToString("N")[..8];
    private static bool _isRunning;

    public static async Task RunLoopAsync(CancellationToken cancellationToken = default)
    {
        if (!_isRunning)
        {
            _isRunning = true;
            Logging.Info("ETLRunner", $"ETL Runner started (machine ID: {_computerId})");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await RunOnceAsync();
                }
                catch (Exception ex)
                {
                    Logging.Error("ETLRunner", "Unhandled error in ETL loop", ex);
                }

                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }

            _isRunning = false;
        }
    }

    private static async Task RunOnceAsync()
    {
        using ETLContext db = new();
        List<ETLJob> jobs = await db.Jobs.ToListAsync();

        foreach (ETLJob job in jobs)
        {
            if (!ShouldRun(job)) { continue; }
            if (!await TryAcquireLockAsync(db, job)) { continue; }

            Logging.Info("ETLRunner", $"🔄 Running ETL job: {job.Name} (ID: {job.Id})");
            _ = RunJobAsync(job);
        }
    }

    private static bool ShouldRun(ETLJob job)
    {
        if (string.IsNullOrWhiteSpace(job.CRONSchedule)) { return true; }

        CronExpression expression = CronExpression.Parse(job.CRONSchedule);
        DateTime utcNow = DateTime.UtcNow;
        DateTime reference = job.LastSuccessAt ?? utcNow.AddYears(-1);
        DateTime? nextUtc = expression.GetNextOccurrence(reference, TimeZoneInfo.Utc);

        if (nextUtc is null) { return false; }

        TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        DateTime nowEst = TimeZoneInfo.ConvertTimeFromUtc(utcNow, estZone);
        nowEst = DateTime.SpecifyKind(nowEst, DateTimeKind.Unspecified);
        DateTime nextEst = DateTime.SpecifyKind(nextUtc.Value, DateTimeKind.Unspecified);

        return nextEst <= nowEst;
    }

    private static async Task<bool> TryAcquireLockAsync(ETLContext db, ETLJob job)
    {
        DateTime utcNow = DateTime.UtcNow;

        bool isLocked = !string.IsNullOrEmpty(job.LockOwner) && job.LockAcquiredAt.HasValue && (utcNow - job.LockAcquiredAt.Value).TotalMinutes < job.LockTimeoutMinutes;
        if (isLocked) { return false; }

        // Acquire the lock
        job.LockOwner = _computerId;
        job.LockAcquiredAt = utcNow;

        await db.SaveChangesAsync();
        await Task.Delay(222); // Give time for race condition to complete

        // Double-check lock was acquired
        ETLJob? refreshed = await db.Jobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == job.Id);
        return refreshed != null && refreshed.LockOwner == _computerId;
    }

    private static void ClearLock(ETLJob job)
    {
        job.LockOwner = null;
        job.LockAcquiredAt = null;
    }

    private static async Task RunJobAsync(ETLJob job)
    {
        using ETLContext db = new();

        Logging.Info("ETLRunner", $"🟡 Starting ETL: {job.Name}");
        Stopwatch sw = Stopwatch.StartNew();

        try
        {
            await InvokeJobAsync(job.Name, job);

            job.LastSuccessAt = DateTime.UtcNow;
            job.WasLastRunSuccessful = true;
            job.LastDurationMilliseconds = (int)sw.ElapsedMilliseconds;
            ClearLock(job);

            Logging.Info("ETLRunner", $"✅ Completed ETL: {job.Name} in {job.LastDurationMilliseconds}ms");
        }
        catch (Exception ex)
        {
            job.WasLastRunSuccessful = false;
            job.LastDurationMilliseconds = (int)sw.ElapsedMilliseconds;
            ClearLock(job);

            Logging.Error("ETLRunner", $"❌ ETL Failed: {job.Name}", ex);
        }

        db.Update(job);
        await db.SaveChangesAsync();
    }

    public static async Task<bool> RunJobManuallyAsync(string jobName)
    {
        using ETLContext db = new();
        ETLJob? job = await db.Jobs.FirstOrDefaultAsync(j => j.Name == jobName);

        if (job is null) { return false; }

        if (!await TryAcquireLockAsync(db, job)) { return false; }

        _ = Task.Run(async () => await RunJobAsync(job));
        return true;
    }

    private static async Task InvokeJobAsync(string fullyQualifiedMethod, ETLJob? job = null)
    {
        string[] parts = fullyQualifiedMethod.Split('.');
        if (parts.Length < 2) { throw new ArgumentException("Invalid ETL method format"); }

        string typeName = string.Join('.', parts.Take(parts.Length - 1));
        string methodName = parts.Last();

        string assemblyName = parts[0];
        string fullTypeName = $"{typeName}, {assemblyName}";

        Type? type = Type.GetType(fullTypeName)
            ?? throw new InvalidOperationException($"Type not found: {fullTypeName}");

        MethodInfo? method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static)
            ?? throw new InvalidOperationException($"Method not found: {fullyQualifiedMethod}");

        object?[] parameters = job is not null ? [job] : [];

        try { if (method.Invoke(null, parameters) is Task task) { await task; return; } }
        catch (TargetParameterCountException)
        {
            if (method.GetParameters().Length == 0) // Try again with no parameters if the method does not accept any
            {
                if (method.Invoke(null, []) is Task taskNoParams)
                {
                    await taskNoParams;
                    return;
                }
            }
            throw;
        }

        throw new InvalidOperationException($"Method {fullyQualifiedMethod} did not return Task");
    }
}
