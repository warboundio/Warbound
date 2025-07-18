using Core.Discords;
using Core.Extensions;
using Serilog.Core;
using Serilog.Events;

namespace Core.Logs;

public class DiscordSink : ILogEventSink
{
    private static readonly string[] separator = [" in "];

    public void Emit(LogEvent logEvent)
    {
        if (logEvent.Level < LogEventLevel.Warning) { return; }

        string emoji = logEvent.Level switch
        {
            LogEventLevel.Warning => "⚠️",
            LogEventLevel.Error => "❌",
            LogEventLevel.Fatal => "💀",
            _ => "🔔"
        };

        string message = $"{emoji} [{logEvent.Timestamp:HH:mm:ss}] **{logEvent.Level}**";

        if (logEvent.Exception is not null)
        {
            Exception innermost = logEvent.Exception.GetInnermost();
            string? location = innermost.StackTrace?
                .Split('\n')
                .FirstOrDefault(l => l.Contains(":line "))
                ?.Split(separator, StringSplitOptions.None)[1]
                .Split('\\').Last()
                .Replace(".cs:line ", ":");

            message += $"\n{logEvent.MessageTemplate} | {location} | {innermost.Message}";
        }
        else
        {
            message += $": {logEvent.RenderMessage()}";
        }

        _ = CommandCenterModule.SendLogAsync(message);
    }
}
