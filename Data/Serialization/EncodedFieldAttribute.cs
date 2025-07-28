using System;

namespace Data.Serialization;

[AttributeUsage(AttributeTargets.Property)]
public class EncodedFieldAttribute : Attribute
{
    public int MaxLength { get; set; } = -1;

    /// <summary>
    /// 90 ^ 1 = 0-89
    /// 90 ^ 2 == 0-8099
    /// 90 ^ 3 == 0-728999
    /// </summary>
    /// <param name="maxLength"></param>
    public EncodedFieldAttribute(int maxLength = -1)
    {
        MaxLength = maxLength;
    }
}
