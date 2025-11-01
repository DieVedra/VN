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
        SaveStat[] saveStats = new SaveStat[_stats.Count];
        for (int i = 0; i < _stats.Count; i++)
        {
            saveStats[i] = new SaveStat(_stats[i].NameText, _stats[i].Value);
        }
        return saveStats;
    }

    public List<Stat> GetGameStatsForm()
    {
        List<Stat> stats = new List<Stat>(_stats.Count);
        for (int i = 0; i < _stats.Count; i++)
        {
            stats.Add(new Stat(_stats[i].NameText, 0, _stats[i].ColorField));
        }

        return stats;
    }

    public void AddNextSeriaStats(List<Stat> stats)
    {
        if (stats != null && stats.Count > 0)
        {
            _stats.AddRange(stats);
            _statsChangedReactiveCommand?.Execute();
        }
    }
}