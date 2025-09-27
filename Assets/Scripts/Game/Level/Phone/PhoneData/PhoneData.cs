using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PhoneData
{
    [SerializeField] private int _seriaIndex;
    [SerializeField] private string _namePhone;
    [SerializeField] private string _characterNameKey;
    [SerializeField] private bool _newContentAdded;
    [SerializeField] private Sprite[] _hands;
    [field: SerializeField] public Sprite PhoneFrame { get; private set; }
    [field: SerializeField] public Sprite Background { get; private set; }
    [SerializeField] private List<PhoneContactData> _phoneContactDatas;

    public string NamePhone => _namePhone;
    public string CharacterNameKey => _characterNameKey;
    public int SeriaIndex => _seriaIndex;
    public bool NewContentAdded => _newContentAdded;
    public IReadOnlyList<Sprite> Hands => _hands;
    public IReadOnlyList<PhoneContactData> PhoneContactDatas => _phoneContactDatas;

}
