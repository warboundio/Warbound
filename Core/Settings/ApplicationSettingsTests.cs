using System.Text.Json;
using Core.Encrypt;

namespace Core.Settings;

public sealed class ApplicationSettingsTests
{
    private const string TEST_KEY_BASE64 = "MDEyMzQ1Njc4OWFiY2RlZjAxMjM0NTY3ODlhYmNkZWY=";
    private readonly string _testDirectory;

    public ApplicationSettingsTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), "ApplicationSettingsTests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDirectory);
        
        // Set test encryption key
        Environment.SetEnvironmentVariable("WARBOUND", TEST_KEY_BASE64);
    }

    [Fact]
    public void ItShouldCreateDefaultInstanceWithEmptySettings()
    {
        ApplicationSettings settings = new(_testDirectory, "test.data");
        
        Assert.Equal(string.Empty, settings.PostgresConnection);
        Assert.Equal(string.Empty, settings.DiscordWarboundToken);
        Assert.Equal(string.Empty, settings.BattleNetClientId);
        Assert.Equal(string.Empty, settings.BattleNetSecretId);
        Assert.Equal(string.Empty, settings.GithubToken);
    }

    [Fact]
    public void ItShouldDecryptOnDemandAndNotStoreSecretsInMemory()
    {
        ApplicationSettings settings = new(_testDirectory, "test.data");
        SettingsDto dto = settings.GetDecryptedSettings();
        dto.PostgresConnection = "postgres://test-connection";
        dto.GithubToken = "github-test-token";
        settings.Save(dto);

        // Create new instance to test loading
        ApplicationSettings loadedSettings = new(_testDirectory, "test.data");
        
        Assert.Equal("postgres://test-connection", loadedSettings.PostgresConnection);
        Assert.Equal("github-test-token", loadedSettings.GithubToken);
        Assert.Equal(string.Empty, loadedSettings.DiscordWarboundToken);
    }

    [Fact]
    public void ItShouldSupportSettingPostgresConnection()
    {
        ApplicationSettings settings = new(_testDirectory, "test.data");
        string connectionString = "postgres://localhost/testdb";
        
        settings.SetPostgresConnection(connectionString);
        
        // Verify the change was persisted
        ApplicationSettings reloadedSettings = new(_testDirectory, "test.data");
        Assert.Equal(connectionString, reloadedSettings.PostgresConnection);
    }

    [Fact]
    public void ItShouldSaveAndLoadAllProperties()
    {
        ApplicationSettings settings = new(_testDirectory, "test.data");
        SettingsDto dto = settings.GetDecryptedSettings();
        
        dto.PostgresConnection = "postgres://localhost/db";
        dto.DiscordWarboundToken = "discord-token-123";
        dto.BattleNetClientId = "battlenet-client-456";
        dto.BattleNetSecretId = "battlenet-secret-789";
        dto.GithubToken = "github-token-abc";
        
        settings.Save(dto);

        // Load fresh instance and verify all properties
        ApplicationSettings loadedSettings = new(_testDirectory, "test.data");
        Assert.Equal("postgres://localhost/db", loadedSettings.PostgresConnection);
        Assert.Equal("discord-token-123", loadedSettings.DiscordWarboundToken);
        Assert.Equal("battlenet-client-456", loadedSettings.BattleNetClientId);
        Assert.Equal("battlenet-secret-789", loadedSettings.BattleNetSecretId);
        Assert.Equal("github-token-abc", loadedSettings.GithubToken);
    }

    [Fact]
    public void ItShouldLoadExistingEncryptedFile()
    {
        // Create encrypted config file manually
        SettingsDto originalDto = new()
        {
            PostgresConnection = "postgres://manual-test",
            GithubToken = "manual-github-token"
        };
        
        string json = JsonSerializer.Serialize(originalDto);
        string encrypted = Encryption.Instance.Encrypt(json);
        string configPath = Path.Combine(_testDirectory, "manual.data");
        File.WriteAllText(configPath, encrypted);

        // Load using ApplicationSettings
        ApplicationSettings settings = ApplicationSettings.Load(_testDirectory, "manual.data");
        Assert.Equal("postgres://manual-test", settings.PostgresConnection);
        Assert.Equal("manual-github-token", settings.GithubToken);
    }

    [Fact]
    public void ItShouldThrowWhenLoadingInvalidFile()
    {
        string configPath = Path.Combine(_testDirectory, "invalid.data");
        File.WriteAllText(configPath, "not-valid-encrypted-data");

        Assert.Throws<InvalidOperationException>(() => 
            ApplicationSettings.Load(_testDirectory, "invalid.data"));
    }

    [Fact]
    public void ItShouldCallSaveWithoutParametersToSaveCurrentState()
    {
        ApplicationSettings settings = new(_testDirectory, "test.data");
        
        // Modify via GetDecryptedSettings and Save with parameter
        SettingsDto dto = settings.GetDecryptedSettings();
        dto.PostgresConnection = "initial-connection";
        settings.Save(dto);

        // Modify via SetPostgresConnection
        settings.SetPostgresConnection("updated-connection");
        
        // Call Save() without parameters
        settings.Save();

        // Verify persistence
        ApplicationSettings reloadedSettings = new(_testDirectory, "test.data");
        Assert.Equal("updated-connection", reloadedSettings.PostgresConnection);
    }

    [Fact]
    public void ItShouldCreateDirectoryIfNotExists()
    {
        string nonExistentDir = Path.Combine(_testDirectory, "new-directory");
        Assert.False(Directory.Exists(nonExistentDir));

        ApplicationSettings settings = new(nonExistentDir, "test.data");
        SettingsDto dto = settings.GetDecryptedSettings();
        dto.PostgresConnection = "test-connection";
        settings.Save(dto);

        Assert.True(Directory.Exists(nonExistentDir));
        Assert.True(File.Exists(Path.Combine(nonExistentDir, "test.data")));
    }

    [Fact]
    public void ItShouldHandleMultiplePropertyAccessesCorrectly()
    {
        ApplicationSettings settings = new(_testDirectory, "test.data");
        SettingsDto dto = settings.GetDecryptedSettings();
        dto.PostgresConnection = "postgres-test";
        dto.GithubToken = "github-test";
        settings.Save(dto);

        // Multiple accesses should all work correctly
        Assert.Equal("postgres-test", settings.PostgresConnection);
        Assert.Equal("github-test", settings.GithubToken);
        Assert.Equal("postgres-test", settings.PostgresConnection); // Second access
        Assert.Equal(string.Empty, settings.DiscordWarboundToken); // Empty property
    }

    [Fact]
    public void ItShouldHandleCorruptConfigurationGracefully()
    {
        ApplicationSettings settings = new(_testDirectory, "test.data");
        SettingsDto dto = settings.GetDecryptedSettings();
        dto.PostgresConnection = "valid-connection";
        settings.Save(dto);

        // Corrupt the file by writing invalid JSON (but valid encryption)
        string invalidJson = "{ invalid json content }";
        string encrypted = Encryption.Instance.Encrypt(invalidJson);
        string configPath = Path.Combine(_testDirectory, "test.data");
        File.WriteAllText(configPath, encrypted);

        ApplicationSettings corruptedSettings = new(_testDirectory, "test.data");
        Assert.Throws<JsonException>(() => corruptedSettings.PostgresConnection);
    }
}