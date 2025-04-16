using System;
using UnityEngine;

[Serializable]
public class BaseStat
{
    [SerializeField] private string _name;
    [SerializeField] private int _value;
    
    public string Name => _name;
    public int Value => _value;

    public BaseStat(string name, int value)
    {
        _name = name;
        _value = value;
    }
}