using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GameStatsCustodian : MonoBehaviour
{
    [SerializeField] private List<Stat> _stats;
    private ReactiveCommand _statsChangedReactiveCommand;

    public int Count => _stats.Count;
    public ReactiveCommand StatsChangedReactiveCommand => _statsChangedReactiveCommand;
    public IReadOnlyList<Stat> Stats => _stats;
    public IReadOnlyList<ILocalizationString> StatsLocalizationStrings => _stats;
    public void Init(List<Stat> stats)
    {
        AddNextSeriaStats(stats);
        _statsChangedReactiveCommand = new ReactiveCommand();
    }
    public SaveStat[] GetSaveStatsToSave()
    {
        List<SaveStat> baseStats = new List<SaveStat>(_stats.Count);
        for (int i = 0; i < _stats.Count; i++)
        {
            baseStats.Add(new SaveStat(_stats[i].NameText, _stats[i].Value, _stats[i].ShowKey));
        }

        return baseStats.ToArray();
    }

    public List<Stat> GetGameStatsForm()
    {
        List<Stat> stats = new List<Stat>(_stats.Count);
        for (int i = 0; i < _stats.Count; i++)
        {
            stats.Add(new Stat(_stats[i].NameText, 0, _stats[i].ShowKey, _stats[i].ColorField));
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

    public void UpdateStatFromSave(SaveStat[] saveStats)
    {
        for (int i = 0; i < _stats.Count; i++)
        {
            for (int j = 0; j < saveStats.Length; j++)
            {
                if (_stats[i].NameKey == saveStats[j].NameKey)
                {
                    _stats[i] = new Stat(_stats[i].NameText, saveStats[j].Value, saveStats[j].ShowKey, _stats[i].ColorField);
                }
            }
        }
    }

    public void UpdateStat(List<BaseStat> AddStats)
    {
        for (int i = 0; i < _stats.Count; i++)
        {
            _stats[i] = new Stat(_stats[i].NameText, _stats[i].Value + AddStats[i].Value, _stats[i].ShowKey,
                _stats[i].ColorField);
        }
    }

    public void UpdateStat(List<Stat> AddStats)
    {
        for (int i = 0; i < _stats.Count; i++)
        {
            _stats[i] = new Stat(_stats[i].NameText, _stats[i].Value + AddStats[i].Value, AddStats[i].ShowKey,
                _stats[i].ColorField);
        }
    }

    public List<Stat> ReinitStats(List<Stat> oldStats)
    {
        List<Stat> newStats = GetGameStatsForm();
        if (oldStats.Count > newStats.Count)
        {
            Reinit(newStats);
        }
        else if (oldStats.Count < newStats.Count)
        {
            Reinit(oldStats);
        }

        void Reinit(List<Stat> stats)
        {
            for (int i = 0; i < stats.Count; i++)
            {
                if (FindingMatchStat(newStats, stats[i]))
                {              
                    InsertStat(i);
                }
            }
        }
        void InsertStat(int index)
        {
            newStats.Insert(index, new Stat(oldStats[index].NameText, oldStats[index].Value, oldStats[index].ShowKey, oldStats[index].ColorField));
        }
        return newStats;
    }

    public void AddNextSeriaStats(List<Stat> stats)
    {
        if (stats != null && stats.Count > 0)
        {
            _stats.AddRange(stats);
            _statsChangedReactiveCommand?.Execute();
        }
    }

    private bool FindingMatchStat(List<Stat> stats, Stat stat)
    {
        bool result = false;
        for (int i = 0; i < stats.Count; i++)
        {
            if (stat.NameKey == stats[i].NameKey)
            {
                result = true;
                break;
            }
        }
        return result;
    }
}