using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameStatsHandler
{
    private const int _defaultValue = 0;
    private readonly List<Stat> _stats;

    private readonly Dictionary<string, Stat> _statsDictionary;
    public IReadOnlyList<Stat> Stats => _stats;
    public IReadOnlyDictionary<string, Stat> StatsDictionary => _statsDictionary;

    public GameStatsHandler(List<Stat> stats)
    {
        _stats = stats;
        _statsDictionary = stats.ToDictionary(x => x.NameKey);
    }

    public GameStatsHandler()
    {
        _stats = new List<Stat>();
        _statsDictionary = new Dictionary<string, Stat>();
    }
    public void FillSaveStats(StoryData storyData)
    {
        storyData.Stats.Clear();
        foreach (var stat in _stats)
        {
            storyData.Stats.Add(new SaveStat(stat.NameKey, stat.Value));
        }
    }
    public List<CustomizationStat> GetGameCustomizationStatsForm()
    {
        List<CustomizationStat> stats = new List<CustomizationStat>(_stats.Count);
        foreach (var t in _stats)
        {
            stats.Add(new CustomizationStat(t.NameText, t.NameKey, _defaultValue, false, t.ColorField));
        }
        return stats;
    }
    
    public List<CustomizationStat> GetCustomizationStatsForm()
    {
        List<CustomizationStat> stats = new List<CustomizationStat>(_stats.Count);
        foreach (var t in _stats)
        {
            stats.Add(new CustomizationStat(t.NameText, t.NameKey, _defaultValue, t.NotificationKey, t.ColorField));
        }
        return stats;
    }
    public List<BaseStat> GetGameBaseStatsForm()
    {
        List<BaseStat> stats = new List<BaseStat>(_stats.Count);
        foreach (var t in _stats)
        {
            stats.Add(new BaseStat(t.NameText, t.NameKey, _defaultValue, t.ColorField));
        }
    
        return stats;
    }
    public List<CaseBaseStat> CreateCaseBaseStatForm()
    {
        List<CaseBaseStat> caseStats = new List<CaseBaseStat>(_stats.Count);
        foreach (var t in _stats)
        {
            caseStats.Add(new CaseBaseStat(t.NameText, t.Value, _defaultValue, false));
        }
        return caseStats;
    }
    public void UpdateStatFromSave(IReadOnlyList<SaveStat> saveStats)
    {
        for (int i = 0; i < _stats.Count; i++)
        {
            foreach (var ss in saveStats)
            {
                if (_stats[i].NameKey == ss.NameKey && ss.Value != _stats[i].Value)
                {
                    _stats[i] = new Stat(_stats[i].NameText, _stats[i].NameKey, ss.Value, _stats[i].ColorField);
                    TryAddToDictionary(_stats[i].NameKey, _stats[i]);
                }
            }
        }
    }

    public void UpdateStats(IReadOnlyList<BaseStat> addStats)
    {
        var addStatsDictionary = addStats.ToDictionaryDistinct(stat => stat.NameKey);
        for (int i = 0; i < _stats.Count; i++)
        {
            if (addStatsDictionary.TryGetValue(_stats[i].NameKey, out BaseStat stat))
            {
                _stats[i] = new Stat(_stats[i].NameText, _stats[i].NameKey, _stats[i].Value + stat.Value,
                    _stats[i].ColorField);
                TryAddToDictionary(_stats[i].NameKey, _stats[i]);
            }
        }
    }

    public void UpdateStats(List<CustomizationStat> addStats)
    {
        var addStatsDictionary = addStats.ToDictionaryDistinct(stat => stat.NameKey);
        for (int i = 0; i < _stats.Count; i++)
        {
            if (addStatsDictionary.TryGetValue(_stats[i].NameKey, out CustomizationStat stat))
            {
                _stats[i] = new Stat(_stats[i].NameText, _stats[i].NameKey,_stats[i].Value + stat.Value,
                    _stats[i].ColorField);
                TryAddToDictionary(_stats[i].NameKey, _stats[i]);
            }
        }
    }

    public List<CustomizationStat> ReinitCustomizationStats(List<CustomizationStat> oldStats)
    {
        List<CustomizationStat> newStats = GetCustomizationStatsForm();
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
    public List<BaseStat> ReinitBaseStats(IReadOnlyList<BaseStat> oldStats)
    {
        List<BaseStat> newStats = GetGameBaseStatsForm();
        var oldStatsDictionary = oldStats.ToDictionaryDistinct(stat => stat.NameKey);
        for (int i = 0; i < newStats.Count; i++)
        {
            if (oldStatsDictionary.TryGetValue(newStats[i].NameKey, out BaseStat oldStat))
            {
                newStats[i] = oldStat;
            }
        }
        return newStats;
    }
    public void AddNextSeriaStats(IReadOnlyList<Stat> stats)
    {
        if (stats != null && stats.Count > 0)
        {
            _stats.AddRange(stats);
            _statsDictionary.AddRange(stats.ToDictionary(x=>x.NameKey));
        }
    }

    private void TryAddToDictionary(string key, Stat stat)
    {
        if (_statsDictionary.ContainsKey(key))
        {
            _statsDictionary[key] = stat;
        }
        else
        {
            _statsDictionary.Add(key, stat);
        }
    }
}