using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PhoneData
{
    [SerializeField] private int _seriaIndex;
    [SerializeField] private string _namePhone = "ElisPhone";
    [SerializeField] private List<PhoneContactData> _phoneContactDatas;

    public string NamePhone => _namePhone;
    public int SeriaIndex => _seriaIndex;
    public IReadOnlyList<PhoneContactData> PhoneContactDatas => _phoneContactDatas;

}
