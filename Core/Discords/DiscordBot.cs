using System.Reflection;
using Core.Logs;
using Core.Settings;
using Discord;
using Discord.WebSocket;

namespace Core.Discords;

public class DiscordBot : IDisposable
{
    private readonly DiscordSocketClient _client;
    private readonly List<ChannelHandler> _handlers;

    public DiscordBot()
    {
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.MessageContent | GatewayIntents.AllUnprivileged,
            LogLevel = LogSeverity.Warning
        });

        // Set the DiscordClient in the base class so all handlers can use it
        SetDiscordClientInBaseClass(_client);

        // Auto-discover and register all ChannelHandler subclasses
        _handlers = DiscoverChannelHandlers();
    }

    public void Dispose() => _client.Dispose();

    public async Task StartAsync()
    {
        _client.Log += LogAsync;
        _client.MessageReceived += MessageReceivedAsync;

        await _client.LoginAsync(TokenType.Bot, ApplicationSettings.Instance.DiscordWarboundToken);
        await _client.StartAsync();

        Logging.Info("DiscordBot", "Discord bot started successfully.");
        await Task.Delay(Timeout.Infinite);
    }

    private Task LogAsync(LogMessage log)
    {
        if (log.Severity < LogSeverity.Warning) { return Task.CompletedTask; }
        Logging.Info($"[Discord]", log.Message);
        return Task.CompletedTask;
    }

    private async Task MessageReceivedAsync(SocketMessage message)
    {
        if (message.Author.IsBot) { return; }
        foreach (ChannelHandler handler in _handlers) { await handler.HandleMessageAsync(message); }
    }

    private static void SetDiscordClientInBaseClass(DiscordSocketClient client)
    {
        // Use reflection to set the protected static DiscordClient property in the base class
        typeof(ChannelHandler)
            .GetProperty("DiscordClient", BindingFlags.NonPublic | BindingFlags.Static)?
            .SetValue(null, client);
    }

    private static List<ChannelHandler> DiscoverChannelHandlers()
    {
        List<ChannelHandler> handlers = [];

        // Find all types that inherit from ChannelHandler in the current assembly
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();

        foreach (Type type in types)
        {
            if (type.IsSubclassOf(typeof(ChannelHandler)) && !type.IsAbstract)
            {
                // Create an instance of the handler
                if (Activator.CreateInstance(type) is ChannelHandler handler)
                {
                    handlers.Add(handler);
                }
            }
        }

        return handlers;
    }
}
