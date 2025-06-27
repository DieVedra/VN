
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatProviderSeria", menuName = "StatProviderSeria", order = 51)]
public class SeriaStatProvider : ScriptableObject
{
    [SerializeField] private int _seriaNumber;
    [SerializeField] private List<Stat> _stats;
    public int SeriaNumber => _seriaNumber;
    public IReadOnlyList<ILocalizationString> StatsLocalizationStrings => _stats;
    public IReadOnlyList<Stat> Stats => _stats;

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