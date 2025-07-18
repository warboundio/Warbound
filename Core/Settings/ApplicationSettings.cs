using System.Text.Json;
using Core.Encrypt;

namespace Core.Settings;

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

    // PROPERTIES TO USE
    public string PostgresConnection { get; set; } = string.Empty;
    public string DiscordWarboundToken { get; set; } = string.Empty;
    public string BattleNetClientId { get; set; } = string.Empty;
    public string BattleNetSecretId { get; set; } = string.Empty;
    public string GithubToken { get; set; } = string.Empty;


    public ApplicationSettings() : this(_defaultDirectory, _defaultFileName) { }

    public ApplicationSettings(string directoryPath, string fileName = "config.data")
    {
        _directoryPath = directoryPath;
        _fileName = fileName;
    }

    public void Save()
    {
        if (!Directory.Exists(_directoryPath))
        {
            Directory.CreateDirectory(_directoryPath);
        }

        string json = JsonSerializer.Serialize(this, _serializerOptions);
        string encrypted = Encryption.Instance.Encrypt(json);
        File.WriteAllText(_configPath, encrypted);
    }

    public static ApplicationSettings Load(string directoryPath, string fileName = "config.data")
    {
        string configPath = Path.Combine(directoryPath, fileName);

        try
        {
            string encrypted = File.ReadAllText(configPath);
            string json = Encryption.Instance.Decrypt(encrypted);
            ApplicationSettings? loaded = JsonSerializer.Deserialize<ApplicationSettings>(json, _serializerOptions) ?? throw new InvalidOperationException("Configuration file is corrupt or invalid.");
            loaded._directoryPath = directoryPath;
            loaded._fileName = fileName;
            return loaded;
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
