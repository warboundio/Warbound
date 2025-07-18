using Core.Logs;
using Discord.WebSocket;

namespace Core.Discords;

public delegate Task CommandHandler(SocketMessage message, string[] args);

public record CommandDefinition(string Name, string Description, CommandHandler Handler);

public class CommandRegistry
{
    private readonly Dictionary<string, CommandDefinition> _commands = [];

    public void Register(string command, string description, CommandHandler handler)
    {
        string normalizedCommand = command.ToLowerInvariant();
        _commands[normalizedCommand] = new CommandDefinition(command, description, handler);
    }

    public bool TryExecute(SocketMessage message) => TryExecuteInternal(message.Content, message);

    internal bool TryExecuteInternal(string content, SocketMessage? message = null)
    {
        content = content.Trim();
        if (!content.StartsWith('!')) { return false; }

        // Split command and arguments
        string[] parts = content[1..].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            return false;
        }

        string commandName = parts[0].ToLowerInvariant();
        string[] args = parts.Length > 1 ? parts[1..] : [];

        if (_commands.TryGetValue(commandName, out CommandDefinition? definition))
        {
            _ = Task.Run(async () =>
            {
                try { await definition.Handler(message!, args); }
                catch (Exception ex) { Logging.Error("CommandRegistry", $"An error occurred when trying to kick off the internal handler '{content}'.", ex); }
            });

            return true;
        }

        return false;
    }

    public IEnumerable<CommandDefinition> ListCommands() => _commands.Values;
}
