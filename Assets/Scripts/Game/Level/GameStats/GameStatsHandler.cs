
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameStatsHandler
{
    private readonly List<Stat> _stats;

    public List<Stat> Stats => _stats;

    public GameStatsHandler(List<Stat> stats)
    {
        _stats = stats;
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
    public Dictionary<string, Stat> GetGameStatsFormDictionary()
    {
        Dictionary<string, Stat> stats = new Dictionary<string, Stat>(_stats.Count);
        for (int i = 0; i < _stats.Count; i++)
        {
            stats.Add(_stats[i].Name, new Stat(_stats[i].Name, 0, _stats[i].ShowKey, _stats[i].ColorField));
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

    public void UpdateStat(List<BaseStat> AddStats)
    {
        for (int i = 0; i < _stats.Count; i++)
        {
            _stats[i] = new Stat(_stats[i].Name, _stats[i].Value + AddStats[i].Value, _stats[i].ShowKey,
                _stats[i].ColorField);
        }
    }

    public void UpdateStat(List<Stat> AddStats)
    {
        for (int i = 0; i < _stats.Count; i++)
        {
            _stats[i] = new Stat(_stats[i].Name, _stats[i].Value + AddStats[i].Value, AddStats[i].ShowKey,
                _stats[i].ColorField);
        }
    }

    public List<Stat> ReinitStats(List<Stat> oldStats)
    {
        List<Stat> newStats = GetGameStatsForm();
        Dictionary<string, Stat> dictionaryOldStats = GetDictionary(oldStats);
        for (int i = 0; i < newStats.Count; i++)
        {
            if (dictionaryOldStats.TryGetValue(newStats[i].Name, out Stat stat))
            {
                newStats.Insert(i, new Stat(stat.Name, stat.Value, stat.ShowKey, stat.ColorField));
            }
        }
        return newStats;
    }

    public void AddNextSeriaStats(List<Stat> stats)
    {
        if (stats != null && stats.Count > 0)
        {
            _stats.AddRange(stats);
            // _statsChangedReactiveCommand?.Execute();
        }
    }

    private Dictionary<string, Stat> GetDictionary(List<Stat> oldStats)
    {
        var dictionaryOldStats = new Dictionary<string, Stat>();
        foreach (var stat in oldStats)
        {
            if (!dictionaryOldStats.ContainsKey(stat.Name))
            {
                dictionaryOldStats.Add(stat.Name, stat);
            }
        }

        return dictionaryOldStats;
    }
}