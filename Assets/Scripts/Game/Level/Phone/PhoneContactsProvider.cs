using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PhoneContactsProvider", menuName = "Phone/PhoneContactsProvider", order = 51)]
public class PhoneContactsProvider : ScriptableObject
{
    [SerializeField] private List<PhoneContactData> _phoneContactDatas;
    
    public IReadOnlyList<PhoneContactData> PhoneContactDatas => _phoneContactDatas;
}