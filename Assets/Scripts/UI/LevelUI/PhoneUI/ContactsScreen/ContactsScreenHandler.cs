using TMPro;
using UnityEngine;

public class ContactsScreenHandler : PhoneScreenBaseHandler
{
    private readonly TextMeshProUGUI _textFind;
    private readonly TextMeshProUGUI _textCalls;
    private readonly TextMeshProUGUI _textExit;
    private readonly TextMeshProUGUI _textContacts;
    private readonly TextMeshProUGUI _textAddContact;
    private readonly RectTransform _contactsTransform;
    public ContactsScreenHandler(ContactsScreenView contactsScreenViewBackground, TopPanelHandler topPanelHandler)
        :base(contactsScreenViewBackground.gameObject, topPanelHandler, contactsScreenViewBackground.ImageBackground, contactsScreenViewBackground.ColorTopPanel)
    {
        _textFind = contactsScreenViewBackground.TextFind;
        _textCalls = contactsScreenViewBackground.TextCalls;
        _textExit = contactsScreenViewBackground.TextExit;
        _textContacts = contactsScreenViewBackground.TextContacts;
        _textAddContact = contactsScreenViewBackground.TextAddContact;
        _contactsTransform = contactsScreenViewBackground.ContactsTransform;
    }
}