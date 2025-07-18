using Discord;
using Discord.WebSocket;

namespace Core.Discords;

public abstract class ChannelHandler
{
    public ulong ChannelId { get; }
    protected static DiscordSocketClient DiscordClient { get; set; } = null!;
    protected CommandRegistry CommandRegistry { get; }

    protected ChannelHandler(ulong channelId)
    {
        ChannelId = channelId;
        CommandRegistry = new CommandRegistry();
        RegisterDefaultCommands();
    }

    public async Task HandleMessageAsync(SocketMessage message)
    {
        if (message.Channel.Id != ChannelId) { return; }
        if (CommandRegistry.TryExecute(message)) { return; }
        await HandleMessageInternalAsync(message);
    }

    protected abstract Task HandleMessageInternalAsync(SocketMessage message);

    private void RegisterDefaultCommands()
    {
        CommandRegistry.Register("help", "Show available commands", HandleHelpCommand);
        CommandRegistry.Register("clear", "Clear recent messages", HandleClearCommand);
    }

    private async Task HandleHelpCommand(SocketMessage message, string[] args)
    {
        IEnumerable<string> commandList = CommandRegistry.ListCommands()
            .Select(cmd => $"- !{cmd.Name} – {cmd.Description}");

        string helpMessage = "Available commands:\n" + string.Join("\n", commandList);
        await message.Channel.SendMessageAsync(helpMessage);
    }

    private async Task HandleClearCommand(SocketMessage message, string[] args)
    {
        if (message.Channel is not SocketTextChannel textChannel)
        {
            await message.Channel.SendMessageAsync("This command can only be used in text channels.");
            return;
        }

        try
        {
            const int DISCORD_LIMIT = 100;
            int totalDeleted = 0;

            while (true)
            {
                IEnumerable<IMessage> messages = await textChannel.GetMessagesAsync(limit: DISCORD_LIMIT).FlattenAsync();
                List<IMessage> deletable = [.. messages.Where(m => (DateTimeOffset.UtcNow - m.Timestamp).TotalDays < 14)];

                if (deletable.Count == 0) { break; }

                await textChannel.DeleteMessagesAsync(deletable);
                totalDeleted += deletable.Count;

                await Task.Delay(1500);
            }

            await message.Channel.SendMessageAsync($"✅ Channel cleared. Deleted {totalDeleted} messages.");
        }
        catch (Exception ex)
        {
            await message.Channel.SendMessageAsync($"❌ Failed to clear messages: {ex.Message}");
        }
    }


    protected async Task SendLogAsync(string content)
    {
        if (DiscordClient.ConnectionState != ConnectionState.Connected)
        {
            return;
        }

        IMessageChannel? channel = DiscordClient.GetChannel(ChannelId) as IMessageChannel;
        if (channel is not null)
        {
            await channel.SendMessageAsync(content);
        }
    }
}
