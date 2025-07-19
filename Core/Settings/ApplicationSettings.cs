using System.Text.Json;
using Core.Encrypt;

namespace Core.Settings;

/// <summary>
/// Internal DTO representing the decrypted configuration data.
/// This is only instantiated temporarily during property access.
/// </summary>
public sealed record SettingsDto
{
    public string PostgresConnection { get; set; } = string.Empty;
    public string DiscordWarboundToken { get; set; } = string.Empty;
    public string BattleNetClientId { get; set; } = string.Empty;
    public string BattleNetSecretId { get; set; } = string.Empty;
    public string GithubToken { get; set; } = string.Empty;
}

public sealed class ApplicationSettings
{
    private static readonly JsonSerializerOptions _serializerOptions = new() { WriteIndented = true };
    private static readonly string _defaultFileName = "config.data";
    private static readonly string _defaultDirectory = GetDefaultDirectory();
    private static readonly Lazy<ApplicationSettings> _instance = new(() => Load(_defaultDirectory, _defaultFileName));
    public static ApplicationSettings Instance => _instance.Value;
    private string _directoryPath { get; set; }
    private string _fileName { get; set; }
    private string _configPath => Path.Combine(_directoryPath, _fileName);

    // Store only the encrypted configuration in memory
    private string _encryptedConfig;

    // PROPERTIES TO USE - Read-only getters that decrypt on-demand
    public string PostgresConnection => GetSecret(dto => dto.PostgresConnection);
    public string DiscordWarboundToken => GetSecret(dto => dto.DiscordWarboundToken);
    public string BattleNetClientId => GetSecret(dto => dto.BattleNetClientId);
    public string BattleNetSecretId => GetSecret(dto => dto.BattleNetSecretId);
    public string GithubToken => GetSecret(dto => dto.GithubToken);

    public ApplicationSettings() : this(_defaultDirectory, _defaultFileName) { }

    public ApplicationSettings(string directoryPath, string fileName = "config.data")
    {
        _directoryPath = directoryPath;
        _fileName = fileName;
        _encryptedConfig = LoadEncryptedConfig();
    }

    private ApplicationSettings(string directoryPath, string fileName, string encryptedConfig)
    {
        _directoryPath = directoryPath;
        _fileName = fileName;
        _encryptedConfig = encryptedConfig;
    }

    /// <summary>
    /// Decrypts configuration on-demand, extracts the specified value, and immediately clears memory.
    /// </summary>
    /// <typeparam name="T">Type of the value to extract.</typeparam>
    /// <param name="selector">Function to select the desired value from the decrypted DTO.</param>
    /// <returns>The extracted value.</returns>
    private T GetSecret<T>(Func<SettingsDto, T> selector)
    {
        // Decrypt and deserialize into temporary DTO
        string decryptedJson = Encryption.Instance.Decrypt(_encryptedConfig);
        SettingsDto? dto = JsonSerializer.Deserialize<SettingsDto>(decryptedJson, _serializerOptions);
        
        if (dto == null)
        {
            throw new InvalidOperationException("Configuration file is corrupt or invalid.");
        }

        try
        {
            // Extract the requested value
            return selector(dto);
        }
        finally
        {
            // Clear the decrypted JSON from memory (best effort)
            unsafe
            {
                fixed (char* ptr = decryptedJson)
                {
                    for (int i = 0; i < decryptedJson.Length; i++)
                    {
                        ptr[i] = '\0';
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets a fully decrypted DTO for modification purposes.
    /// The caller is responsible for calling Save() to persist changes.
    /// </summary>
    /// <returns>A decrypted DTO that can be modified.</returns>
    public SettingsDto GetDecryptedSettings()
    {
        string decryptedJson = Encryption.Instance.Decrypt(_encryptedConfig);
        SettingsDto? dto = JsonSerializer.Deserialize<SettingsDto>(decryptedJson, _serializerOptions);
        
        if (dto == null)
        {
            throw new InvalidOperationException("Configuration file is corrupt or invalid.");
        }

        return dto;
    }

    /// <summary>
    /// Saves the provided settings DTO to the encrypted configuration file.
    /// </summary>
    /// <param name="settings">The settings to save.</param>
    public void Save(SettingsDto settings)
    {
        if (!Directory.Exists(_directoryPath))
        {
            Directory.CreateDirectory(_directoryPath);
        }

        string json = JsonSerializer.Serialize(settings, _serializerOptions);
        string encrypted = Encryption.Instance.Encrypt(json);
        File.WriteAllText(_configPath, encrypted);
        
        // Update the in-memory encrypted config
        _encryptedConfig = encrypted;
    }

    /// <summary>
    /// Convenience method to set the PostgreSQL connection string.
    /// </summary>
    /// <param name="connectionString">The connection string to set.</param>
    public void SetPostgresConnection(string connectionString)
    {
        SettingsDto settings = GetDecryptedSettings();
        settings.PostgresConnection = connectionString;
        Save(settings);
    }

    public void Save()
    {
        // Get current decrypted settings and save them
        SettingsDto currentSettings = GetDecryptedSettings();
        Save(currentSettings);
    }

    private string LoadEncryptedConfig()
    {
        try
        {
            return File.ReadAllText(_configPath);
        }
        catch
        {
            // If file doesn't exist or can't be read, create default empty config
            SettingsDto defaultSettings = new();
            string json = JsonSerializer.Serialize(defaultSettings, _serializerOptions);
            return Encryption.Instance.Encrypt(json);
        }
    }

    public static ApplicationSettings Load(string directoryPath, string fileName = "config.data")
    {
        string configPath = Path.Combine(directoryPath, fileName);

        try
        {
            string encryptedConfig = File.ReadAllText(configPath);
            // Validate that we can decrypt it (throws if invalid)
            string testDecrypt = Encryption.Instance.Decrypt(encryptedConfig);
            return new ApplicationSettings(directoryPath, fileName, encryptedConfig);
        }
        catch
        {
            throw new InvalidOperationException("Unable to load Application Settings.");
        }
    }

    private static string GetDefaultDirectory()
    {
        string baseDir = AppContext.BaseDirectory;
        string coreDir = Path.Combine(baseDir, "Core");
        string coreSettingsDir = Path.Combine(coreDir, "Settings");

        return Directory.Exists(coreSettingsDir) ? coreSettingsDir : Path.Combine(AppContext.BaseDirectory, "Settings");
    }
}
