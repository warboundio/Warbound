#pragma warning disable CS8618, CS8604

namespace Core.Tools;

public class PathMonitor : IDisposable
{
    private readonly FileSystemWatcher _watcher;
    private readonly System.Timers.Timer _debounceTimer;
    private FileSystemEventArgs _pendingEventArgs;

    public event EventHandler<FileSystemEventArgs> Changed;
    public DateTime LastModified { get; private set; }

    private const double DebounceInterval = 250;

    public PathMonitor(string path)
    {
        LastModified = File.Exists(path) ? File.GetLastWriteTime(path) : Directory.Exists(path) ? Directory.GetLastWriteTime(path) : throw new FileNotFoundException($"No file or directory at '{path}'.");

        _debounceTimer = new System.Timers.Timer(DebounceInterval) { AutoReset = false };
        _debounceTimer.Elapsed += (s, e) =>
        {
            LastModified = DateTime.Now;
            Changed?.Invoke(this, _pendingEventArgs);
        };

        if (File.Exists(path))
        {
            string dir = Path.GetDirectoryName(path)!;
            string filename = Path.GetFileName(path);
            _watcher = new FileSystemWatcher(dir, filename)
            {
                IncludeSubdirectories = false,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
            };
            _watcher.Changed += HandleChange;
            _watcher.Renamed += HandleChange;
            _watcher.Deleted += HandleChange;
        }
        else
        {
            _watcher = new FileSystemWatcher(path)
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName
            };

            _watcher.Changed += HandleChange;
            _watcher.Created += HandleChange;
            _watcher.Deleted += HandleChange;
            _watcher.Renamed += HandleChange;
        }

        _watcher.EnableRaisingEvents = true;
    }

    private void HandleChange(object sender, FileSystemEventArgs e)
    {
        _pendingEventArgs = e;
        _debounceTimer.Stop();
        _debounceTimer.Start();
    }

    public void Dispose()
    {
        _watcher.Dispose();
        _debounceTimer.Dispose();
    }
}
