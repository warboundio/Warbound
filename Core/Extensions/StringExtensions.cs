using System.Security.Cryptography;
using System.Text;

namespace Core.Extensions;

public static class StringExtensions
{
    public static string Hash(this string input)
    {
        if (input == null) { return string.Empty; }
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = SHA256.HashData(bytes);
        StringBuilder builder = new(); foreach (byte b in hashBytes) { builder.Append(b.ToString("x2")); }
        return builder.ToString();
    }
}
