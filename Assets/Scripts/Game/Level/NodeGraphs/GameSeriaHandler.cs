using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class GameSeriaHandler : MonoBehaviour
{
    [SerializeField, Expandable] private List<SeriaData> _seriaDatas;

    public void Construct()
    {
        
    }
}