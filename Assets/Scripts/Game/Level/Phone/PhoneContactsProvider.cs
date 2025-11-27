using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "PhoneContactsProvider", menuName = "Phone/PhoneContactsProvider", order = 51)]
public class PhoneContactsProvider : ScriptableObject
{
    [SerializeField] private int _seriaIndex;

    [SerializeField] private List<PhoneContact> _phoneContacts;
    
    public IReadOnlyList<PhoneContact> PhoneContacts => _phoneContacts;
    public int SeriaIndex => _seriaIndex;
    
    [Button()]
    private void Init()
    {
        for (int i = 0; i < _phoneContacts.Count; i++)
        {
            _phoneContacts[i].Init();
        }
    }
}