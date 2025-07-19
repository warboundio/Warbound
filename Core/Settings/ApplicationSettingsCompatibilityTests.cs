using Core.Settings;

namespace Core.Settings;

/// <summary>
/// Tests to verify that existing usage patterns continue to work
/// after the encryption at rest refactoring.
/// </summary>
public sealed class ApplicationSettingsCompatibilityTests
{
    private const string TEST_KEY_BASE64 = "MDEyMzQ1Njc4OWFiY2RlZjAxMjM0NTY3ODlhYmNkZWY=";
    private readonly string _testDirectory;

    public ApplicationSettingsCompatibilityTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), "CompatibilityTests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDirectory);
        
        // Set test encryption key
        Environment.SetEnvironmentVariable("WARBOUND", TEST_KEY_BASE64);
    }

    [Fact]
    public void ItShouldWorkWithExistingBlizzardAPIUsagePattern()
    {
        // Simulate how BlizzardTokenProvider uses the settings
        ApplicationSettings settings = new(_testDirectory, "test.data");
        SettingsDto dto = settings.GetDecryptedSettings();
        dto.BattleNetClientId = "test-client-id";
        dto.BattleNetSecretId = "test-secret-id";
        settings.Save(dto);

        // Simulate the existing usage: ApplicationSettings.Instance.BattleNetClientId
        string clientId = settings.BattleNetClientId;
        string secretId = settings.BattleNetSecretId;

        Assert.Equal("test-client-id", clientId);
        Assert.Equal("test-secret-id", secretId);
    }

    [Fact]
    public void ItShouldWorkWithExistingDatabaseConnectionUsagePattern()
    {
        // Simulate how ETLContext uses the settings
        ApplicationSettings settings = new(_testDirectory, "test.data");
        settings.SetPostgresConnection("postgres://localhost:5432/warbound");

        // Simulate the existing usage: ApplicationSettings.Instance.PostgresConnection
        string connectionString = settings.PostgresConnection;

        Assert.Equal("postgres://localhost:5432/warbound", connectionString);
    }

    [Fact]
    public void ItShouldWorkWithExistingGitHubUsagePattern()
    {
        // Simulate how GitHubIssueService uses the settings
        ApplicationSettings settings = new(_testDirectory, "test.data");
        SettingsDto dto = settings.GetDecryptedSettings();
        dto.GithubToken = "ghp_test_token_12345";
        settings.Save(dto);

        // Simulate the existing usage: ApplicationSettings.Instance.GithubToken
        string token = settings.GithubToken;

        Assert.Equal("ghp_test_token_12345", token);
    }

    [Fact]
    public void ItShouldWorkWithExistingDiscordUsagePattern()
    {
        // Simulate how DiscordBot uses the settings
        ApplicationSettings settings = new(_testDirectory, "test.data");
        SettingsDto dto = settings.GetDecryptedSettings();
        dto.DiscordWarboundToken = "discord-bot-token-12345";
        settings.Save(dto);

        // Simulate the existing usage: ApplicationSettings.Instance.DiscordWarboundToken
        string discordToken = settings.DiscordWarboundToken;

        Assert.Equal("discord-bot-token-12345", discordToken);
    }

    [Fact]
    public void ItShouldAllowMultiplePropertyAccessesInSameCodeBlock()
    {
        // Simulate a scenario where multiple properties are accessed in the same method
        ApplicationSettings settings = new(_testDirectory, "test.data");
        SettingsDto dto = settings.GetDecryptedSettings();
        dto.BattleNetClientId = "client-123";
        dto.BattleNetSecretId = "secret-456";
        dto.PostgresConnection = "postgres://localhost/db";
        settings.Save(dto);

        // Simulate multiple accesses like in BlizzardTokenProvider
        string clientId = settings.BattleNetClientId;
        string secretId = settings.BattleNetSecretId;
        string postgresConnection = settings.PostgresConnection;

        Assert.Equal("client-123", clientId);
        Assert.Equal("secret-456", secretId);
        Assert.Equal("postgres://localhost/db", postgresConnection);
    }

    [Fact]
    public void ItShouldMaintainEmptyStringDefaultsForUnsetProperties()
    {
        // Ensure that unset properties return empty strings, not null
        ApplicationSettings settings = new(_testDirectory, "test.data");

        Assert.Equal(string.Empty, settings.PostgresConnection);
        Assert.Equal(string.Empty, settings.DiscordWarboundToken);
        Assert.Equal(string.Empty, settings.BattleNetClientId);
        Assert.Equal(string.Empty, settings.BattleNetSecretId);
        Assert.Equal(string.Empty, settings.GithubToken);
    }
}