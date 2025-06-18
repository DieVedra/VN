
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatProviderSeria", menuName = "StatProviderSeria", order = 51)]
public class SeriaStatProvider : ScriptableObject
{
    [SerializeField] private int _seriaNumber;
    [SerializeField] private List<Stat> _stats;
    // private GameStatsHandler _gameStatsHandler;
    public int SeriaNumber => _seriaNumber;
    public IReadOnlyList<ILocalizationString> StatsLocalizationStrings => _stats;
    public IReadOnlyList<Stat> Stats => _stats;

    // public GameStatsHandler GameStatsHandler
    // {
    //     get
    //     {
    //         if (_gameStatsHandler == null)
    //         {
    //             _gameStatsHandler = new GameStatsHandler(_stats);
    //         }
    //
    //         return _gameStatsHandler;
    //     }
    //     private set{}
    // }

    private void AddNewStat(Stat stat) //вызывает едитор
    {
        if (_stats == null)
        {
            _stats = new List<Stat>();
        }
        _stats.Add(stat);
    }
    private void RemoveStat(int index) //вызывает едитор
    {
        if (_stats != null)
        {
            _stats.Remove(_stats[index]);
        }
    }
}