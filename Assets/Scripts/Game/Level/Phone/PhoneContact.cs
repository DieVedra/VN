using System;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public class PhoneContact
{
    [field: SerializeField] public LocalizationString NameLocalizationString { get; private set; }
    [field: SerializeField] public string ToPhoneKey{ get; private set; }
    [field: SerializeField] public Sprite Icon  { get; private set; }
    [field: SerializeField] public bool IsEmptyIconKey  { get; private set; }
    [field: SerializeField] public bool AddInPlot  { get; private set; }
    [field: SerializeField] public Color Color  { get; private set; }

    public void Init()
    {
        NameLocalizationString.TryRegenerateKey();
        if (Color == Color.clear)
        {
            Color = Color.white;
        }
    }
}