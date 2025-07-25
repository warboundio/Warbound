using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Data.Support;

namespace Data.Serialization;

public class EncodingStringBuilder
{
    protected char Prefix { get; }
    private List<PropertyInfo> encodedProperties = [];
    private Dictionary<string, int> propertyMaxLengths = [];
    protected List<PropertyInfo> StringProps = [];
    protected List<PropertyInfo> NonStringProps = [];
    protected Dictionary<Type, int> UnknownEncodingCache = [];

    public EncodingStringBuilder(char prefix, object obj)
    {
        Prefix = prefix;
        SetupEncodedProperties(obj);
        SetupOrderedProperties();
    }

    private void SetupEncodedProperties(object obj)
    {
        Type type = obj.GetType();
        encodedProperties = [.. type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<EncodedFieldAttribute>() != null)
            .OrderBy(p => p.Name)];

        propertyMaxLengths = [];
        foreach (PropertyInfo prop in encodedProperties)
        {
            int maxLength = GetMaxLength(prop);
            propertyMaxLengths[prop.Name] = maxLength;
        }
    }

    private void SetupOrderedProperties()
    {
        StringProps = [.. encodedProperties.Where(p => p.PropertyType == typeof(string)).OrderBy(p => p.Name)];
        NonStringProps = [.. encodedProperties.Where(p => p.PropertyType != typeof(string)).OrderBy(p => p.Name)];
    }

    private int GetMaxLength(PropertyInfo property)
    {
        EncodedFieldAttribute? attribute = property.GetCustomAttribute<EncodedFieldAttribute>();
        if (attribute != null && attribute.MaxLength > 0) { return attribute.MaxLength; }
        if (property.PropertyType == typeof(string)) { return -1; }

        Type type = property.PropertyType;
        if (type.IsEnum)
        {
            Array values = Enum.GetValues(type);
            long maxEnumValue = 0;
            foreach (object? value in values)
            {
                long intValue = Convert.ToInt64(value);
                if (intValue > maxEnumValue) { maxEnumValue = intValue; }
            }

            int length = 1;
            long range = 90;
            while (maxEnumValue >= range)
            {
                length++;
                range *= 90;
            }
            return length;
        }

        return -1;
    }

    public string GetEncodingTranslation()
    {
        StringBuilder sb = new();
        sb.Append($"{Prefix}|");

        foreach (PropertyInfo prop in StringProps)
        {
            int maxLength = propertyMaxLengths[prop.Name];
            sb.Append($"{prop.Name}_({maxLength})|");
        }

        foreach (PropertyInfo prop in NonStringProps)
        {
            int maxLength = propertyMaxLengths[prop.Name];
            sb.Append($"{prop.Name}({maxLength})|");
        }
        return sb.ToString().TrimEnd('|');
    }

    public string GetEncodedString(object obj)
    {
        StringBuilder sb = new();

        sb.Append($"{Prefix}|");
        foreach (PropertyInfo prop in StringProps) { sb.Append($"{prop.GetValue(obj)}|"); }

        foreach (PropertyInfo prop in NonStringProps)
        {
            int value = (int)prop.GetValue(obj)!;
            int maxLength = propertyMaxLengths[prop.Name];

            if (prop.PropertyType.IsEnum)
            {
                value = (int)Convert.ToInt64(value);
                if (value == -1) { value = GetUnknownEncodingValue(prop.PropertyType); }
            }

            sb.Append($"{Base90.Encode(value, maxLength)}");
        }

        return sb.ToString();
    }

    private int GetUnknownEncodingValue(Type enumType)
    {
        if (!UnknownEncodingCache.TryGetValue(enumType, out int cachedValue))
        {
            Array enumValues = Enum.GetValues(enumType);
            cachedValue = -1;
            foreach (object enumValue in enumValues)
            {
                string name = Enum.GetName(enumType, enumValue) ?? "";
                if (name == "UNKNOWN_ENCODING")
                {
                    cachedValue = (int)Convert.ToInt64(enumValue);
                    break;
                }
            }
            UnknownEncodingCache[enumType] = cachedValue;
        }
        return cachedValue;
    }
}
