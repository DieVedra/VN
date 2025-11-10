
using System.Collections.Generic;
using UnityEngine;

public class GameStatsViewer : MonoBehaviour
{
    public IReadOnlyList<Stat> StatsToView { get; private set; }

    public void Construct(IReadOnlyList<Stat> stats)
    {
        StatsToView = stats;
    }
}