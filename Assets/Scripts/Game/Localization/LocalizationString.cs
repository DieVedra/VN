
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

[Serializable]
public class LocalizationString
{
    [field: SerializeField, TextArea] public string DefaultText { get; private set; }
    [field: SerializeField] public string Key { get; private set; }
    public static List<LocalizationString> LocalizationStrings = new List<LocalizationString>();

    public LocalizationString(string defaultText, string customKey = null)
    {
        DefaultText = defaultText;
        Key = customKey ?? GenerateStableHash(defaultText);
        if (LocalizationStrings == null)
        {
            LocalizationStrings = new List<LocalizationString>();
        }
        LocalizationStrings.Add(this);
    }

    private static string GenerateStableHash(string input)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return "str_" + BitConverter.ToString(hashBytes).Replace("-", "").Substring(0, 8);
        }
    }

    public static implicit operator LocalizationString(string text)
    {
        return new LocalizationString(text);
    }
    public static implicit operator string(LocalizationString localizationString)
    {
        return localizationString?.ToString() ?? string.Empty;
    }
    public static LocalizationString operator +(LocalizationString a, LocalizationString b)
    {
        return new LocalizationString(a.DefaultText + b.DefaultText);
    }

    public void SetText(string text)
    {
        DefaultText = text;
    }
    public override string ToString()
    {
        return DefaultText;
    }
}