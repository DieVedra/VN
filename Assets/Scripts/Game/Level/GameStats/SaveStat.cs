
using System;

[Serializable]
public class SaveStat
{
    public string NameKey { get; private set; }
    public int Value { get; private set; }

    public bool ShowKey { get; private set; }

    public SaveStat(string nameKey, int value, bool showKey)
    {
        NameKey = nameKey;
        Value = value;
        ShowKey = showKey;
    }
}