
using System.Collections.Generic;
using UnityEngine;

public class GameStatsViewer : MonoBehaviour
{
    private List<Stat> _statsToView;
    public IReadOnlyList<Stat> StatsToView => _statsToView;

    public void Construct(List<Stat> stats)
    {
        _statsToView = stats;
    }
}