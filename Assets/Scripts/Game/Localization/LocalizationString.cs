using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

[Serializable]
public class LocalizationString
{
    private const string _prefix = "str_";
    [field: SerializeField, TextArea] public string DefaultText { get; private set; }
    [field: SerializeField] public string Key { get; private set; }

    public LocalizationString(string defaultText = null, string customKey = null)
    {
        DefaultText = defaultText;
        Key = customKey ?? GenerateStableHash(defaultText);
    }
    public static string GenerateStableHash(string input)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return _prefix + BitConverter.ToString(hashBytes).Replace("-", "").Substring(0, 8);
        }
    }

    public static implicit operator LocalizationString(string text)
    {
        return new LocalizationString(defaultText: text);
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

    public bool TryRegenerateKey()
    {
        bool res = false;
        if (string.IsNullOrEmpty(DefaultText) == false && string.IsNullOrWhiteSpace(DefaultText) == false)
        {
            Key = GenerateStableHash(DefaultText);
            res = true;
        }
        return res;
    }
    public override string ToString()
    {
        return DefaultText;
    }
}