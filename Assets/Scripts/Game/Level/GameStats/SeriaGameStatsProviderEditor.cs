using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class SeriaGameStatsProviderEditor : MonoBehaviour, IGameStatsProvider
{
    [SerializeField, Expandable] private List<SeriaStatProvider> _seriaStatsProviders;
    private GameStatsHandler _gameStatsHandler;

    public GameStatsHandler GameStatsHandler => _gameStatsHandler;

    public void Init()
    {
        List<Stat> stats = GetStats();
        _gameStatsHandler = new GameStatsHandler(stats);
    }
    public List<T> GetEmptyTStat<T>(int seriaIndex) where T : BaseStat
    {
        int seriaNumber = ++seriaIndex;
        List<T> stats = new List<T>();
        Stat stat;
        for (int i = 0; i < _seriaStatsProviders.Count; i++)
        {
            if (_seriaStatsProviders[i].SeriaNumber > 0 && _seriaStatsProviders[i].SeriaNumber <= seriaNumber)
            {
                for (int j = 0; j < _seriaStatsProviders[i].Stats.Count; j++)
                {
                    stat = _seriaStatsProviders[i].Stats[j];
                    var newStat = new Stat(stat.NameText, stat.NameKey, stat.Value, stat.ColorField);
                    T type = (T)(object)newStat;
                    stats.Add(type);
                }
            }
            else
            {
                break;
            }
        }
        return stats;
    }
    public List<Stat> GetEmptyStatsFromCurrentSeria(int seriaIndex)
    {
        int seriaNumber = ++seriaIndex;
        List<Stat> stats = new List<Stat>();
        for (int i = 0; i < _seriaStatsProviders.Count; i++)
        {
            if (_seriaStatsProviders[i].SeriaNumber > 0 && _seriaStatsProviders[i].SeriaNumber <= seriaNumber)
            {
                stats.AddRange(_seriaStatsProviders[i].Stats.ToList());
            }
            else
            {
                break;
            }
        }
        return stats;
    }
    public IReadOnlyList<SaveStat> GetAllStatsToSave()
    {
        return GameStatsHandler.GetStatsToSave();
    }
    public void UpdateAllStatsFromSave(IReadOnlyList<SaveStat> saveStats)
    {
        GameStatsHandler.UpdateStatFromSave(saveStats);
    }
    private List<Stat> GetStats()
    {
        List<Stat> newStats = new List<Stat>();
        for (int i = 0; i < _seriaStatsProviders.Count; i++)
        {
            newStats.AddRange(_seriaStatsProviders[i].Stats);
        }
        return newStats.GroupBy(p => p.NameText)
            .Select(g => g.First())
            .ToList();
    }
}