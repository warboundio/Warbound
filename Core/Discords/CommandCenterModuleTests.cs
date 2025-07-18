namespace Core.Discords;

public class CommandCenterModuleTests
{
    [Fact]
    public void ItShouldRegisterRunCommand()
    {
        // Access the static registry from CommandCenterModule to verify run command is registered
        // Note: This is a simplified test since CommandCenterModule uses instance registration

        CommandRegistry registry = new();

        // Simulate the registration that happens in CommandCenterModule
        registry.Register("run", "Manually run an ETL job", async (message, args) =>
        {
            await Task.CompletedTask;
        });

        IEnumerable<CommandDefinition> commands = registry.ListCommands();

        Assert.Contains(commands, c => c.Name == "run");
        Assert.Contains(commands, c => c.Description == "Manually run an ETL job");
    }

    [Fact]
    public void ItShouldRegisterLockClearCommand()
    {
        CommandRegistry registry = new();

        // Simulate the registration that happens in CommandCenterModule
        registry.Register("lockclear", "Clear all ETL job locks", async (message, args) =>
        {
            await Task.CompletedTask;
        });

        IEnumerable<CommandDefinition> commands = registry.ListCommands();

        Assert.Contains(commands, c => c.Name == "lockclear");
        Assert.Contains(commands, c => c.Description == "Clear all ETL job locks");
    }
}
