using System.Collections.Generic;

public class GameStatsHandler
{
    private const int _defaultValue = 0;
    private readonly List<Stat> _stats;

    public List<Stat> Stats => _stats;

    public GameStatsHandler(List<Stat> stats)
    {
        _stats = stats;
    }

    public GameStatsHandler()
    {
        _stats = new List<Stat>();
    }
    public IReadOnlyList<SaveStat> GetStatsToSave()
    {
        List<SaveStat> baseStats = new List<SaveStat>(_stats.Count);
        for (int i = 0; i < _stats.Count; i++)
        {
            baseStats.Add(new SaveStat(_stats[i].NameKey, _stats[i].Value));
        }
    
        return baseStats;
    }

    public List<Stat> GetGameStatsForm()
    {
        List<Stat> stats = new List<Stat>(_stats.Count);
        for (int i = 0; i < _stats.Count; i++)
        {
            stats.Add(new Stat(_stats[i].NameText, _stats[i].NameKey, 0, _stats[i].ColorField));
        }
        return stats;
    }
    // public List<Stat> GetGameStatsForm()
    // {
    //     List<Stat> stats = new List<Stat>(_stats.Count);
    //     for (int i = 0; i < _stats.Count; i++)
    //     {
    //         stats.Add(new Stat(_stats[i].NameText, _stats[i].NameKey, 0, _stats[i].ColorField));
    //     }
    //     return stats;
    // }
    public List<CustomizationStat> GetGameCustomizationStatsForm()
    {
        List<CustomizationStat> stats = new List<CustomizationStat>(_stats.Count);
        for (int i = 0; i < _stats.Count; i++)
        {
            stats.Add(new CustomizationStat(_stats[i].NameText, _stats[i].NameKey, _defaultValue, false, _stats[i].ColorField));
        }
        return stats;
    }
    
    public List<CustomizationStat> GetCustomizationStatsForm()
    {
        List<CustomizationStat> stats = new List<CustomizationStat>(_stats.Count);
        for (int i = 0; i < _stats.Count; i++)
        {
            stats.Add(new CustomizationStat(_stats[i].NameText, _stats[i].NameKey, _defaultValue, _stats[i].NotificationKey, _stats[i].ColorField));
        }
        return stats;
    }
    public List<BaseStat> GetGameBaseStatsForm()
    {
        List<BaseStat> stats = new List<BaseStat>(_stats.Count);
        for (int i = 0; i < _stats.Count; i++)
        {
            stats.Add(new BaseStat(_stats[i].NameText, _stats[i].NameKey, _defaultValue, _stats[i].ColorField));
        }
    
        return stats;
    }

    public void UpdateStatFromSave(IReadOnlyList<SaveStat> saveStats)
    {
        for (int i = 0; i < _stats.Count; i++)
        {
            for (int j = 0; j < saveStats.Count; j++)
            {
                if (_stats[i].NameKey == saveStats[j].NameKey && (saveStats[j].Value != _stats[i].Value) == false)
                {
                    _stats[i] = new Stat(_stats[i].NameKey, _stats[i].NameKey, saveStats[j].Value, _stats[i].ColorField);
                }
            }
        }
    }

    public void UpdateStats(List<BaseStat> addStats)
    {
        // TryRegenerateKeys(addStats);
        var addStatsDictionary = addStats.ToDictionaryDistinct(stat => stat.NameKey);
        for (int i = 0; i < _stats.Count; i++)
        {
            if (addStatsDictionary.TryGetValue(_stats[i].NameKey, out BaseStat stat))
            {
                _stats[i] = new Stat(_stats[i].NameText, _stats[i].NameKey, _stats[i].Value + stat.Value,
                    _stats[i].ColorField);
            }
        }
    }

    public void UpdateStats(List<CustomizationStat> addStats)
    {
        // TryRegenerateKeys(addStats);
        var addStatsDictionary = addStats.ToDictionaryDistinct(stat => stat.NameKey);
        for (int i = 0; i < _stats.Count; i++)
        {
            if (addStatsDictionary.TryGetValue(_stats[i].NameKey, out CustomizationStat stat))
            {
                _stats[i] = new Stat(_stats[i].NameText, _stats[i].NameKey,_stats[i].Value + stat.Value,
                    _stats[i].ColorField);
            }
        }
    }

    public List<CustomizationStat> ReinitCustomizationStats(List<CustomizationStat> oldStats)
    {
        List<CustomizationStat> newStats = GetCustomizationStatsForm();
        // TryRegenerateKeys(oldStats);
        Dictionary<string, CustomizationStat> oldStatsDictionary = oldStats.ToDictionaryDistinct(stat => stat.NameKey);

        for (int i = 0; i < newStats.Count; i++)
        {
            if (oldStatsDictionary.TryGetValue(newStats[i].NameKey, out CustomizationStat oldStat))
            {
                newStats[i].ShowKey = oldStat.ShowKey;
                newStats[i].CustomizationStatValue = oldStat.CustomizationStatValue;
                newStats[i].CustomizationStatNotificationKey = oldStat.CustomizationStatNotificationKey;
            }
        }
        return newStats;
    }
    public List<BaseStat> ReinitBaseStats(List<BaseStat> oldStats)
    {
        List<BaseStat> newStats = GetGameBaseStatsForm();
        // TryRegenerateKeys(oldStats);
        var oldStatsDictionary = oldStats.ToDictionaryDistinct(stat => stat.NameKey);
        var result = new List<BaseStat>(newStats.Count);
        for (int i = 0; i < newStats.Count; i++)
        {
            if (oldStatsDictionary.TryGetValue(newStats[i].NameKey, out BaseStat oldStat))
            {
                result.Add(oldStat);
            }
            else
            {
                result.Add(newStats[i]);
            }
        }
        return result;
    }
    public void AddNextSeriaStats(IReadOnlyList<Stat> stats)
    {
        if (stats != null && stats.Count > 0)
        {
            _stats.AddRange(stats);
        }
    }

    // private void TryRegenerateKeys(List<Stat> oldStats)
    // {
    //     foreach (var stat in oldStats)
    //     {
    //         RegenerateKey(stat);
    //     }
    // }
    // private void TryRegenerateKeys(List<BaseStat> oldStats)
    // {
    //     foreach (var stat in oldStats)
    //     {
    //         RegenerateKey(stat);
    //     }
    // }
    // private void RegenerateKey(BaseStat baseStat)
    // {
    //     baseStat.LocalizationName.TryRegenerateKey();
    // }
}