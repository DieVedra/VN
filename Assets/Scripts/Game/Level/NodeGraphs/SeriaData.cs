using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SeriaData", menuName = "NodeGraphs/SeriaData", order = 51)]
public class SeriaData : ScriptableObject
{
    [SerializeField] private List<SeriaPartNodeGraph> _seriaPartNodeGraphs;

    public void Init()
    {
        
    }
}