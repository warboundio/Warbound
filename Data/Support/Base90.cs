using System;
using System.Collections.Generic;
using System.Linq;

namespace Data.Support;

public static class Base90
{
    public const string Alphabet = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!#$%&()*+,-./:<=>?@[\]^_`{}~";
    private const int Base = 90;
    private const int EncodedLength = 3;

    private static readonly Dictionary<char, int> CharToValue = Alphabet.Select((c, i) => new { Char = c, Index = i }).ToDictionary(x => x.Char, x => x.Index);

    public static string Encode(int number, int requiredLength) => Encode((ulong)number, requiredLength);

    public static List<string> Encode(List<int> numbers, int requiredLength)
    {
        List<string> values = new(numbers.Count);
        foreach (int number in numbers) { values.Add(Encode(number, requiredLength)); }
        return values;
    }

    public static string Encode(ulong number, int requiredLength)
    {
        if (requiredLength < 1 || requiredLength > Alphabet.Length) { throw new ArgumentOutOfRangeException(nameof(requiredLength), "Required length must be between 1 and 90."); }

        ulong maxValue = (ulong)Math.Pow(Base, requiredLength) - 1;
        if (number > maxValue) { throw new ArgumentOutOfRangeException(nameof(number), $"Value {number} exceeds maximum for length {requiredLength} (max {maxValue})."); }

        Span<char> buffer = stackalloc char[requiredLength];
        buffer.Fill(Alphabet[0]);

        int pos = requiredLength - 1;
        while (number > 0 && pos >= 0)
        {
            buffer[pos--] = Alphabet[(int)(number % Base)];
            number /= Base;
        }

        return new string(buffer);
    }

    public static ulong Decode(ReadOnlySpan<char> input)
    {
        if (input.Length != EncodedLength) { throw new ArgumentException($"Base90 encoded strings must be {EncodedLength} characters.", nameof(input)); }

        ulong result = 0;
        foreach (char c in input)
        {
            result = checked((result * Base) + (ulong)CharToValue[c]);
        }
        return result;
    }
}
