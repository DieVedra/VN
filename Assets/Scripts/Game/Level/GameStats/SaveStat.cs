
using System;

[Serializable]
public class SaveStat : BaseStat
{
    private bool _showKey;

    public bool ShowKey => _showKey;

    public SaveStat(string name, int value, bool showKey) : base(name, value)
    {
        _showKey = showKey;
    }
}