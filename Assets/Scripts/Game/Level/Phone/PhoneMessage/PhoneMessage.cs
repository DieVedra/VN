using System;
using UnityEngine;

[Serializable]
public class PhoneMessage
{
    [field: SerializeField] public string Text { get; private set; }
    public bool IsReaded;
}