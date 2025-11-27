using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "PhoneProvider", menuName = "Phone/PhoneProvider", order = 51)]
public class PhoneProvider : ScriptableObject
{
    [SerializeField] private int _seriaIndex;
    [SerializeField] private List<Phone> _phones;
    public IReadOnlyList<Phone> Phone => _phones;
    public int SeriaIndex => _seriaIndex;
    
    [Button()]
    private void GenerateKeys()
    {
        for (int i = 0; i < _phones.Count; i++)
        {
            _phones[i].NamePhone.TryRegenerateKey();
        }
    }
}