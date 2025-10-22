using System.Collections.Generic;

public class GameStatsHandler
{
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
            baseStats.Add(new SaveStat(_stats[i].NameKey, _stats[i].Value, _stats[i].ShowKey));
        }
    
        return baseStats;
    }

    public List<Stat> GetGameStatsForm()
    {
        List<Stat> stats = new List<Stat>(_stats.Count);
        for (int i = 0; i < _stats.Count; i++)
        {
            stats.Add(new Stat(_stats[i].NameKey, 0, _stats[i].ShowKey, _stats[i].ColorField));
        }

        return stats;
    }

    public List<BaseStat> GetGameBaseStatsForm()
    {
        List<BaseStat> stats = new List<BaseStat>(_stats.Count);
        for (int i = 0; i < _stats.Count; i++)
        {
            stats.Add(new BaseStat(_stats[i].NameText, 0));
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
                    _stats[i] = new Stat(_stats[i].NameKey, saveStats[j].Value, saveStats[j].ShowKey, _stats[i].ColorField);
                }
            }
        }
    }

    public void UpdateStats(List<BaseStat> addStats)
    {
        TryRegenerateKeys(addStats);
        var addStatsDictionary = addStats.ToDictionaryDistinct(stat => stat.NameKey);
        for (int i = 0; i < _stats.Count; i++)
        {
            if (addStatsDictionary.TryGetValue(_stats[i].NameKey, out BaseStat stat))
            {
                _stats[i] = new Stat(_stats[i].NameText, _stats[i].NameKey, _stats[i].Value + stat.Value, _stats[i].ShowKey,
                    _stats[i].ColorField);
            }
        }
    }

    public void UpdateStats(List<Stat> addStats)
    {
        TryRegenerateKeys(addStats);
        var addStatsDictionary = addStats.ToDictionaryDistinct(stat => stat.NameKey);
        for (int i = 0; i < _stats.Count; i++)
        {
            if (addStatsDictionary.TryGetValue(_stats[i].NameKey, out Stat stat))
            {
                _stats[i] = new Stat(_stats[i].NameText, _stats[i].NameKey,_stats[i].Value + stat.Value, stat.ShowKey,
                    _stats[i].ColorField);
            }
        }
    }

    public List<Stat> ReinitStats(List<Stat> oldStats)
    {
        List<Stat> newStats = GetGameStatsForm();
        TryRegenerateKeys(oldStats);
        var oldStatsDictionary = oldStats.ToDictionaryDistinct(stat => stat.NameKey);
        var result = new List<Stat>(newStats.Count);

        for (int i = 0; i < newStats.Count; i++)
        {
            if (oldStatsDictionary.TryGetValue(newStats[i].NameKey, out Stat oldStat))
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
    public List<BaseStat> ReinitBaseStats(List<BaseStat> oldStats)
    {
        List<BaseStat> newStats = GetGameBaseStatsForm();
        TryRegenerateKeys(oldStats);
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

    private void TryRegenerateKeys(List<Stat> oldStats)
    {
        foreach (var stat in oldStats)
        {
            RegenerateKey(stat);
        }
    }
    private void TryRegenerateKeys(List<BaseStat> oldStats)
    {
        foreach (var stat in oldStats)
        {
            RegenerateKey(stat);
        }
    }
    private void RegenerateKey(BaseStat baseStat)
    {
        baseStat.LocalizationName.TryRegenerateKey();
    }
}