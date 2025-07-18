namespace Core.Encrypt;

public sealed class EncryptionTests
{
    private const string TEST_KEY_BASE64 = "MDEyMzQ1Njc4OWFiY2RlZjAxMjM0NTY3ODlhYmNkZWY=";

    [Fact]
    public void ItShouldEncryptAndDecryptRoundTrip()
    {
        Encryption settings = new(TEST_KEY_BASE64);
        string originalText = "Hello, World! This is a test message.";

        string encrypted = settings.Encrypt(originalText);
        string decrypted = settings.Decrypt(encrypted);

        Assert.Equal(originalText, decrypted);
        Assert.NotEqual(originalText, encrypted);
    }

    [Fact]
    public void ItShouldHandleEmptyStrings()
    {
        Encryption settings = new(TEST_KEY_BASE64);
        string emptyText = "";

        string encrypted = settings.Encrypt(emptyText);
        string decrypted = settings.Decrypt(encrypted);

        Assert.Equal(emptyText, decrypted);
        Assert.NotEqual(emptyText, encrypted);
    }

    [Fact]
    public void ItShouldProduceDifferentOutputForSameInput()
    {
        Encryption settings = new(TEST_KEY_BASE64);
        string text = "Same input text";

        string encrypted1 = settings.Encrypt(text);
        string encrypted2 = settings.Encrypt(text);

        Assert.NotEqual(encrypted1, encrypted2);

        Assert.Equal(text, settings.Decrypt(encrypted1));
        Assert.Equal(text, settings.Decrypt(encrypted2));
    }

    [Fact]
    public void ItShouldFailDecryptionWithWrongKey()
    {
        string wrongKeyBase64 = "enl4d3Z1dHNycXBvbm1sa2ppaGdmZWRjYmE5ODc2NTQ=";
        string text = "Test message";

        Encryption correctSettings = new(TEST_KEY_BASE64);
        Encryption wrongSettings = new(wrongKeyBase64);

        string encrypted = correctSettings.Encrypt(text);

        Assert.Throws<ArgumentException>(() => wrongSettings.Decrypt(encrypted));
    }

    [Fact]
    public void ItShouldThrowWhenConstructorArgumentIsInvalidBase64()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => new Encryption("not-valid-base64!"));
        Assert.Contains("contains invalid base64 data", exception.Message);
    }

    [Fact]
    public void ItShouldThrowWhenConstructorArgumentHasWrongKeyLength()
    {
        string shortKeyBase64 = "c2hvcnRrZXkxNmJ5dGVz";

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => new Encryption(shortKeyBase64));
        Assert.Contains("Invalid encryption key length", exception.Message);
    }

    [Fact]
    public void ItShouldThrowWhenDecryptingInvalidBase64()
    {
        Encryption settings = new(TEST_KEY_BASE64);

        Assert.Throws<ArgumentException>(() => settings.Decrypt("not-valid-base64!"));
    }

    [Fact]
    public void ItShouldThrowWhenDecryptingTooShortData()
    {
        Encryption settings = new(TEST_KEY_BASE64);
        string tooShortData = Convert.ToBase64String(new byte[10]);

        ArgumentException exception = Assert.Throws<ArgumentException>(() => settings.Decrypt(tooShortData));
        Assert.Contains("too short to contain nonce and tag", exception.Message);
    }

    [Fact]
    public void ItShouldThrowWhenEncryptingNullInput()
    {
        Encryption settings = new(TEST_KEY_BASE64);

        Assert.Throws<ArgumentNullException>(() => settings.Encrypt(null!));
    }

    [Fact]
    public void ItShouldThrowWhenDecryptingNullInput()
    {
        Encryption settings = new(TEST_KEY_BASE64);

        Assert.Throws<ArgumentNullException>(() => settings.Decrypt(null!));
    }

    [Fact]
    public void ItShouldThrowWhenConstructorArgumentIsNull() =>
        Assert.Throws<ArgumentNullException>(() => new Encryption(null!));

    [Fact]
    public void ItShouldAllowMultipleInstancesWithDifferentKeys()
    {
        string differentKeyBase64 = "enl4d3Z1dHNycXBvbm1sa2ppaGdmZWRjYmE5ODc2NTQ=";
        string text = "Test message for multiple instances";

        Encryption settings1 = new(TEST_KEY_BASE64);
        Encryption settings2 = new(differentKeyBase64);

        string encrypted1 = settings1.Encrypt(text);
        string encrypted2 = settings2.Encrypt(text);

        Assert.NotEqual(encrypted1, encrypted2);
        Assert.Equal(text, settings1.Decrypt(encrypted1));
        Assert.Equal(text, settings2.Decrypt(encrypted2));

        Assert.Throws<ArgumentException>(() => settings1.Decrypt(encrypted2));
        Assert.Throws<ArgumentException>(() => settings2.Decrypt(encrypted1));
    }
}
