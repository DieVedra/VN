using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class GameStatsCustodian : MonoBehaviour
{
    [SerializeField] private List<Stat> _stats;
    private ReactiveCommand _statsChangedReactiveCommand;

    public int Count => _stats.Count;
    public ReactiveCommand StatsChangedReactiveCommand => _statsChangedReactiveCommand;
    public IReadOnlyList<Stat> Stats => _stats;

    public void Init(SaveStat[] initStats = null)
    {
        if (initStats != null && initStats.Length == _stats.Count)
        {
            UpdateStat(initStats.ToList());
        }

        _statsChangedReactiveCommand = new ReactiveCommand();
    }

    public SaveStat[] GetSaveStatsToSave()
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

    private void UpdateStat(List<SaveStat> saveStats)
    {
        for (int i = 0; i < _stats.Count; i++)
        {
            _stats[i] = new Stat(_stats[i].Name, saveStats[i].Value, saveStats[i].ShowKey, _stats[i].ColorField);
        }
    }
    public void UpdateStat(List<BaseStat> AddStats)
    {
        for (int i = 0; i < _stats.Count; i++)
        {
            _stats[i] = new Stat(_stats[i].Name, _stats[i].Value + AddStats[i].Value, _stats[i].ShowKey, _stats[i].ColorField);
        }
    }
    public void UpdateStat(List<Stat> AddStats)
    {
        for (int i = 0; i < _stats.Count; i++)
        {
            _stats[i] = new Stat(_stats[i].Name, _stats[i].Value + AddStats[i].Value, AddStats[i].ShowKey, _stats[i].ColorField);
        }
    }
    private void AddNewStat(Stat stat) //вызывает едитор
    {
        if (_stats == null)
        {
            _stats = new List<Stat>();
        }
        _stats.Add(stat);
        if (_statsChangedReactiveCommand != null)
        {
            _statsChangedReactiveCommand.Execute();
        }
    }
    private void RemoveStat(int index) //вызывает едитор
    {
        if (_stats != null)
        {
            _stats.Remove(_stats[index]);
        }
        _statsChangedReactiveCommand.Execute();
    }
}