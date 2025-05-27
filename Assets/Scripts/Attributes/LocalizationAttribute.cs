
using System;
[AttributeUsage(AttributeTargets.Field)]
public class LocalizationAttribute : Attribute
{
    public string Key { get; }

    public LocalizationAttribute(string key = null)
    {
        Key = key;
    }
}