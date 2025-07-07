
using System.Collections.Generic;
using UnityEngine;

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
    public SaveStat[] GetStatsToSave()
    {
        List<SaveStat> baseStats = new List<SaveStat>(_stats.Count);
        for (int i = 0; i < _stats.Count; i++)
        {
            baseStats.Add(new SaveStat(_stats[i].Name, _stats[i].Value, _stats[i].ShowKey));
        }
    
        return baseStats.ToArray();
    }

    public List<Stat> GetGameStatsForm()
    {
        List<Stat> stats = new List<Stat>(_stats.Count);
        for (int i = 0; i < _stats.Count; i++)
        {
            stats.Add(new Stat(_stats[i].Name, 0, _stats[i].ShowKey, _stats[i].ColorField));
        }

        return stats;
    }

    public List<BaseStat> GetGameBaseStatsForm()
    {
        List<BaseStat> stats = new List<BaseStat>(_stats.Count);
        for (int i = 0; i < _stats.Count; i++)
        {
            stats.Add(new BaseStat(_stats[i].Name, 0));
        }

        return stats;
    }

    public void UpdateStatFromSave(SaveStat[] saveStats)
    {
        for (int i = 0; i < _stats.Count; i++)
        {
            for (int j = 0; j < saveStats.Length; j++)
            {
                if (_stats[i].Name == saveStats[j].Name && (saveStats[j].Value != _stats[i].Value) == false)
                {
                    _stats[i] = new Stat(_stats[i].Name, saveStats[j].Value, saveStats[j].ShowKey, _stats[i].ColorField);
                }
            }
        }
    }

    public void UpdateStat(List<BaseStat> addStats)
    {
        var addStatsDictionary = addStats.ToDictionaryDistinct(stat => stat.Key);
        for (int i = 0; i < _stats.Count; i++)
        {
            if (addStatsDictionary.TryGetValue(_stats[i].Key, out BaseStat stat))
            {
                _stats[i] = new Stat(_stats[i].Name, _stats[i].Key, _stats[i].Value + stat.Value, _stats[i].ShowKey,
                    _stats[i].ColorField);
            }
        }
    }

    public void UpdateStat(List<Stat> addStats)
    {
        var addStatsDictionary = addStats.ToDictionaryDistinct(stat => stat.Key);
        for (int i = 0; i < _stats.Count; i++)
        {
            if (addStatsDictionary.TryGetValue(_stats[i].Key, out Stat stat))
            {
                _stats[i] = new Stat(_stats[i].Name, _stats[i].Key,_stats[i].Value + stat.Value, stat.ShowKey,
                    _stats[i].ColorField);
            }
        }
    }

    public List<Stat> ReinitStats(List<Stat> oldStats)
    {
        List<Stat> newStats = GetGameStatsForm();
        var oldStatsDictionary = oldStats.ToDictionaryDistinct(stat => stat.Key);
        var result = new List<Stat>(newStats.Count);

        for (int i = 0; i < newStats.Count; i++)
        {
            if (oldStatsDictionary.TryGetValue(newStats[i].Key, out Stat oldStat))
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
        var oldStatsDictionary = oldStats.ToDictionaryDistinct(stat => stat.Key);
        var result = new List<BaseStat>(newStats.Count);
        for (int i = 0; i < newStats.Count; i++)
        {
            if (oldStatsDictionary.TryGetValue(newStats[i].Key, out BaseStat oldStat))
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
    public void AddNextSeriaStats(List<Stat> stats)
    {
        if (stats != null && stats.Count > 0)
        {
            _stats.AddRange(stats);
        }
    }
}