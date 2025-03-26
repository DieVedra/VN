using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AdditionalImagesToBackground", menuName = "AdditionalImagesToBackground", order = 51)]
public class AdditionalImagesToBackground : ScriptableObject
{
    [SerializeField] private List<Sprite> _sprites;

    public IReadOnlyList<Sprite> Additional => _sprites;
}