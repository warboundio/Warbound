using System;
using System.Collections.Generic;
using System.Linq;

namespace Data.Support;

public static class Base90
{
    private const string Alphabet = "rO'Y{5Lu#2.WzXECaFcVRbqT9oKHN86G/sA~^3@;MJP)gZUf:&h]1ekQlxmI+di4$_np70D,?!Bv-[jt*=y%";
    private const int Base = 90;
    private const int EncodedLength = 3;
    private const ulong MaxValue = 728_999; // 90^3 - 1

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
        buffer.Fill(' ');

        int pos = requiredLength;
        while (number > 0 && pos > 0)
        {
            buffer[--pos] = Alphabet[(int)(number % Base)];
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
            if (c == ' ') { continue; }
            result = checked((result * Base) + (ulong)CharToValue[c]);
        }
        return result;
    }
}
