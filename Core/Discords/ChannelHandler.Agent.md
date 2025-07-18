# Warbound Pattern: Discord Channel Modules

This document defines how automation agents should implement and register **modular Discord channel handlers** for the Warbound system. Each module handles a specific channel and follows a strict pattern for message processing and response.

---

## Pattern Summary

Each Discord channel module should:

- Inherit from `ChannelHandler` abstract class
- Pass the target channel ID to the base constructor
- Implement `HandleMessageInternalAsync()` for channel-specific logic
- Expose a `SendLogAsync()` static helper (optional)
- Be registered in `DiscordBot.cs` during construction

---

## Example: `CommandCenterModule`

```csharp
using Discord;
using Discord.WebSocket;

namespace Core.Discords;

public class CommandCenterModule : ChannelHandler
{
    public const ulong COMMAND_CENTER_CHANNEL_ID = 1394682978164015255;
    public static DiscordSocketClient DiscordClient { get; set; } = null!;

    public CommandCenterModule() : base(COMMAND_CENTER_CHANNEL_ID) { }

    protected override async Task HandleMessageInternalAsync(SocketMessage message)
    {
        await message.Channel.SendMessageAsync("\ud83d\udce1 Command center received: " + message.Content);
    }

    public static async Task SendLogAsync(string content)
    {
        if (DiscordClient.ConnectionState != ConnectionState.Connected) { return; }

        IMessageChannel? channel = DiscordClient.GetChannel(COMMAND_CENTER_CHANNEL_ID) as IMessageChannel;
        if (channel is not null)
        {
            await channel.SendMessageAsync(content);
        }
    }
}
```

---

## Registering the Module

In `DiscordBot.cs`, register the module in the `_handlers` list and assign the shared `DiscordClient` reference:

```csharp
public class DiscordBot : IDisposable
{
    private readonly DiscordSocketClient _client;
    private readonly List<ChannelHandler> _handlers;

    public DiscordBot()
    {
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        });

        CommandCenterModule.DiscordClient = _client;

        _handlers =
        [
            new CommandCenterModule()
        ];
    }
}
```

---

## Abstract Class: `ChannelHandler`

All modules must inherit from:

```csharp
public abstract class ChannelHandler
{
    public ulong ChannelId { get; }

    protected ChannelHandler(ulong channelId)
    {
        ChannelId = channelId;
    }

    public Task HandleMessageAsync(SocketMessage message)
    {
        if (message.Channel.Id != ChannelId)
        {
            return Task.CompletedTask;
        }

        return HandleMessageInternalAsync(message);
    }

    protected abstract Task HandleMessageInternalAsync(SocketMessage message);
}
```

---

## Key Rules

- **One module = one channel.** Use constants to declare the bound channel ID.
- **Channel filtering is automatic** – the abstract class handles channel gating.
- **Implement `HandleMessageInternalAsync()`** for your channel-specific logic.
- **Static SendLogAsync()** is encouraged for sending messages externally (e.g. from ETL jobs).
- **Do not reuse client references** � assign from the central bot only.

---

## When to Update

Update this pattern if:

- Message types change (e.g. SlashCommands support added)
- Modules start subscribing to events other than messages
- Multi-channel modules are needed (not currently allowed)

---

## Purpose

This pattern ensures:

- Clear ownership of each channel�s logic
- Easy plug-and-play modularity for future Discord integrations
- Compatibility with automation agents and monitoring tools
- Reduced boilerplate through shared channel filtering logic
