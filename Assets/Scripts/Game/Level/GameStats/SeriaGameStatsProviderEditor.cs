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

    public List<Stat> GetStatsFromCurrentSeria(int seriaIndex)
    {
        int seriaNumber = ++seriaIndex;
        List<Stat> stats = new List<Stat>();
        List<Stat> statsForm;
        for (int i = 0; i < _seriaStatsProviders.Count; i++)
        {
            if (_seriaStatsProviders[i].SeriaNumber > 0)
            {
                if (_seriaStatsProviders[i].SeriaNumber <= seriaNumber)
                {
                    statsForm = _seriaStatsProviders[i].Stats.ToList();
                    stats.AddRange(statsForm);
                }
                else
                {
                    break;
                }
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