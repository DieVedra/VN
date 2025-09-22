using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PhoneDataProvider", menuName = "Phone/PhoneDataProvider", order = 51)]
public class PhoneDataProvider : ScriptableObject
{
    [SerializeField] private int _seriaIndex;
    [SerializeField] private List<PhoneData> _phoneDatas;
    
    public IReadOnlyList<PhoneData> PhoneDatas => _phoneDatas;
    public int SeriaIndex => _seriaIndex;
}