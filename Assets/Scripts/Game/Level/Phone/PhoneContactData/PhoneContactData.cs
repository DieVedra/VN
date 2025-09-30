using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PhoneContactData
{
    [field: SerializeField] public string Name { get; private set; }

    [field: SerializeField] public Sprite Icon  { get; private set; }
    [field: SerializeField] public bool IsEmptyIconKey  { get; private set; }


    [SerializeField] private List<PhoneMessage> _phoneMessages;
    
    public IReadOnlyList<PhoneMessage> PhoneMessages => _phoneMessages;
}