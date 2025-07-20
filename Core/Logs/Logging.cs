using Serilog;
using Serilog.Events;

namespace Core.Logs;

public static class Logging
{
    private const string LOG_DIRECTORY = @"C:\Applications\Warbound\logs";

    public static void Configure()
    {
        if (!Directory.Exists(LOG_DIRECTORY))
        {
            Directory.CreateDirectory(LOG_DIRECTORY);
        }

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: $@"{LOG_DIRECTORY}\etl-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 2,
                restrictedToMinimumLevel: LogEventLevel.Information)
            .WriteTo.Sink(new DiscordSink())
            .CreateLogger();
    }

    public static void Info(string classOrApplication, string message) =>
        Log.Information($"{classOrApplication} | {message}", []);

    public static void Warn(string classOrApplication, string message) =>
        Log.Warning($"{classOrApplication} | {message}", []);

    public static void Error(string classOrApplication, string message) =>
        Log.Error($"{classOrApplication} | {message}", []);

    public static void Error(string classOrApplication, string message, Exception ex) =>
        Log.Error(ex, $"{classOrApplication} | {message}", []);

    public static void Fatal(string classOrApplication, string message, Exception ex) =>
        Log.Fatal(ex, $"{classOrApplication} | {message}", []);
}
