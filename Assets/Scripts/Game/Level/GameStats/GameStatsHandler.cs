
using System.Collections.Generic;

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
            newStats.Insert(index, new Stat(oldStats[index].Name, oldStats[index].Value, oldStats[index].ShowKey, oldStats[index].ColorField));
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

    private bool FindingMatchStat(List<Stat> stats, Stat stat)
    {
        bool result = false;
        for (int i = 0; i < stats.Count; i++)
        {
            if (stat.Name == stats[i].Name)
            {
                result = true;
                break;
            }
        }
        return result;
    }
}