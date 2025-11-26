using System;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public class PhoneContactData
{
    [field: SerializeField] public string NikName { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Icon  { get; private set; }
    [field: SerializeField] public bool IsEmptyIconKey  { get; private set; }
    [field: SerializeField] public Color Color  { get; private set; } = Color.white;

    [field:SerializeField, Expandable] public PhoneMessagesGraph PhoneMessagesGraph { get; private set; }
    
}