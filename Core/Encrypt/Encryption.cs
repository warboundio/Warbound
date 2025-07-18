using System.Security.Cryptography;
using System.Text;

namespace Core.Encrypt;

/// <summary>
/// Provides AES-256-GCM encryption and decryption functionality using a registered key or environment variables.
/// </summary>
public sealed class Encryption
{
    private const string ENVIRONMENT_VARIABLE_NAME = "WARBOUND";
    private const int NONCE_SIZE = 12; // 96 bits for GCM
    private const int TAG_SIZE = 16;   // 128 bits for GCM
    private const int KEY_SIZE = 32;   // 256 bits for AES-256

    private static readonly Lazy<Encryption> _instance = new(() => new Encryption());
    private readonly byte[] _key;

    /// <summary>
    /// Gets the shared instance that uses the encryption key from the WARBOUND environment variable.
    /// </summary>
    public static Encryption Instance => _instance.Value;

    /// <summary>
    /// Initializes a new instance using the encryption key from the WARBOUND environment variable.
    /// </summary>
    public Encryption()
    {
        string? keyBase64 = Environment.GetEnvironmentVariable(ENVIRONMENT_VARIABLE_NAME);

        if (string.IsNullOrEmpty(keyBase64))
        {
            throw new InvalidOperationException(
                $"Environment variable '{ENVIRONMENT_VARIABLE_NAME}' is missing or empty. Set a base64-encoded 256-bit encryption key in the environment variable.");
        }

        _key = GetKeyFromBase64(keyBase64);
    }

    /// <summary>
    /// Initializes a new instance with the specified encryption key.
    /// </summary>
    /// <param name="keyBase64">Base64-encoded 256-bit encryption key.</param>
    public Encryption(string keyBase64)
    {
        ArgumentNullException.ThrowIfNull(keyBase64);
        _key = GetKeyFromBase64(keyBase64);
    }

    private static byte[] GetKeyFromBase64(string keyBase64)
    {
        try
        {
            byte[] key = Convert.FromBase64String(keyBase64);

            return key.Length != KEY_SIZE
                ? throw new InvalidOperationException(
                    $"Invalid encryption key length. Expected {KEY_SIZE} bytes (256 bits), but got {key.Length} bytes.")
                : key;
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException(
                $"Key contains invalid base64 data.", ex);
        }
    }

    /// <summary>
    /// Encrypts the specified plain text using AES-256-GCM.
    /// </summary>
    /// <param name="plainText">The text to encrypt.</param>
    /// <returns>Base64-encoded string containing nonce, ciphertext, and authentication tag.</returns>
    public string Encrypt(string plainText)
    {
        ArgumentNullException.ThrowIfNull(plainText);

        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] nonce = new byte[NONCE_SIZE];
        byte[] ciphertext = new byte[plainBytes.Length];
        byte[] tag = new byte[TAG_SIZE];

        RandomNumberGenerator.Fill(nonce);

        using AesGcm aes = new(_key, TAG_SIZE);
        aes.Encrypt(nonce, plainBytes, ciphertext, tag);

        // Combine nonce + ciphertext + tag
        byte[] result = new byte[NONCE_SIZE + ciphertext.Length + TAG_SIZE];
        Buffer.BlockCopy(nonce, 0, result, 0, NONCE_SIZE);
        Buffer.BlockCopy(ciphertext, 0, result, NONCE_SIZE, ciphertext.Length);
        Buffer.BlockCopy(tag, 0, result, NONCE_SIZE + ciphertext.Length, TAG_SIZE);

        return Convert.ToBase64String(result);
    }

    /// <summary>
    /// Decrypts the specified encrypted text that was produced by the Encrypt method.
    /// </summary>
    /// <param name="encryptedText">Base64-encoded string containing nonce, ciphertext, and authentication tag.</param>
    /// <returns>The decrypted plain text.</returns>
    public string Decrypt(string encryptedText)
    {
        ArgumentNullException.ThrowIfNull(encryptedText);

        try
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

            if (encryptedBytes.Length < NONCE_SIZE + TAG_SIZE)
            {
                throw new ArgumentException("Invalid encrypted data: too short to contain nonce and tag.", nameof(encryptedText));
            }

            // Extract nonce, ciphertext, and tag
            byte[] nonce = new byte[NONCE_SIZE];
            byte[] ciphertext = new byte[encryptedBytes.Length - NONCE_SIZE - TAG_SIZE];
            byte[] tag = new byte[TAG_SIZE];

            Buffer.BlockCopy(encryptedBytes, 0, nonce, 0, NONCE_SIZE);
            Buffer.BlockCopy(encryptedBytes, NONCE_SIZE, ciphertext, 0, ciphertext.Length);
            Buffer.BlockCopy(encryptedBytes, NONCE_SIZE + ciphertext.Length, tag, 0, TAG_SIZE);

            byte[] plainBytes = new byte[ciphertext.Length];

            using AesGcm aes = new(_key, TAG_SIZE);
            aes.Decrypt(nonce, ciphertext, tag, plainBytes);

            return Encoding.UTF8.GetString(plainBytes);
        }
        catch (FormatException ex)
        {
            throw new ArgumentException("Invalid encrypted data: not valid base64.", nameof(encryptedText), ex);
        }
        catch (CryptographicException ex)
        {
            throw new ArgumentException("Failed to decrypt data. The data may be corrupted or encrypted with a different key.", nameof(encryptedText), ex);
        }
    }
}
