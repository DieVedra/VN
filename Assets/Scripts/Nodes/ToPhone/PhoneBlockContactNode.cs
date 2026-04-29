
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[NodeWidth(350),NodeTint("#2620F6")]
public class PhoneBlockContactNode : BaseNode
{
    [SerializeField] private int _phoneIndex;
    [SerializeField] private string _contactKey;
    [SerializeField] private string _contactName;
    [SerializeField] private bool _blockContact;
    [SerializeField] private bool _unblockContact;
    
    
    
    private List<PhoneContact> _allContacts;

    public IReadOnlyList<Phone> Phones;
    public Phone CurrentPhone => Phones[_phoneIndex];
    public IReadOnlyList<PhoneContact> AllContacts => _allContacts;

    private IReadOnlyList<PhoneContact> _contactsToAddInPlot;


    public void ConstructMyPhoneBlockContactNode(IReadOnlyList<Phone> phones, IReadOnlyList<PhoneContact> contactsToAddInPlot)
    {
        Phones = phones;
        _contactsToAddInPlot = contactsToAddInPlot;
        InitAllContacts();
    }

    public override UniTask Enter(bool isMerged = false)
    {

        if (CurrentPhone.PhoneContactDictionary.TryGetValue(_contactKey, out var value))
        {
            if (_blockContact == true)
            {
                value.ContactBlockedMe = true;
            }
            else if (_unblockContact == true)
            {
                value.ContactBlockedMe = false;
            }
        }
        
        return base.Enter(isMerged);
    }

    private void InitAllContacts()
    {
        if (_allContacts == null)
        {
            _allContacts = new List<PhoneContact>();
        } 
        _allContacts.Clear();
        foreach (var contact in CurrentPhone.PhoneContactDictionary)
        {
            _allContacts.Add(contact.Value);
        }
        _allContacts.AddRange(_contactsToAddInPlot);
    }
}