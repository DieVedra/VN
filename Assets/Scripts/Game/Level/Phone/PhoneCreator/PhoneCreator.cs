using System.Collections.Generic;
using System.Linq;

public class PhoneCreator
{
    private readonly IReadOnlyList<PhoneProvider> _phoneProviders;
    private readonly Dictionary<string, Phone> _phonesDictionary;
    private readonly PhoneMessagesCustodian _phoneMessagesCustodian;
    private readonly CheckMathSeriaIndex _checkMathSeriaIndex;

    public PhoneCreator(IReadOnlyList<PhoneProvider> phoneProviders,
        PhoneMessagesCustodian phoneMessagesCustodian, CheckMathSeriaIndex checkMathSeriaIndex)
    {
        _phoneProviders = phoneProviders;
        _phoneMessagesCustodian = phoneMessagesCustodian;
        _checkMathSeriaIndex = checkMathSeriaIndex;
        _phonesDictionary = new Dictionary<string, Phone>();
    }
    
    
    public List<Phone> CreatePhonesOnStart(int currentSeriaIndex, bool isRuntime = true)
    {
        PhoneProvider phoneProvider;
        Phone phone;
        List<Phone> phones = new List<Phone>();
        for (int i = 0; i < _phoneProviders.Count; i++)
        {
            phoneProvider = _phoneProviders[i];
            if (phoneProvider.SeriaIndex <= currentSeriaIndex)
            {
                for (int j = 0; j < phoneProvider.Phone.Count; j++)
                {
                    phone = phoneProvider.Phone[j];
                    if (_phonesDictionary.ContainsKey(phone.NamePhone.Key) == false)
                    {
                        var newPhone = GetNewPhone(phone);
                        _phoneMessagesCustodian.AddPhoneHistory(newPhone.NamePhone.Key);
                        _phonesDictionary.Add(newPhone.NamePhone.Key, newPhone);
                    }
                }
            }
        }
        phones.AddRange(_phonesDictionary.Select(x=>x.Value).ToList());
        if (isRuntime == false)
        {
            _phonesDictionary.Clear();
        }
        return phones;
        
    }

    public void TryAddPhone(List<Phone> phones, int currentSeriaIndex)
    {
        if (_checkMathSeriaIndex.Check(currentSeriaIndex))
        {
            PhoneProvider phoneProvider;
            Phone phone;
            for (int i = 0; i < _phoneProviders.Count; i++)
            {
                phoneProvider = _phoneProviders[i];
                if (phoneProvider.SeriaIndex == currentSeriaIndex)
                {
                    for (int j = 0; j < phoneProvider.Phone.Count; j++)
                    {
                        phone = phoneProvider.Phone[j];
                        if (_phonesDictionary.ContainsKey(phone.NamePhone.Key) == false)
                        {
                            var newPhone = GetNewPhone(phone);
                            phones.Add(newPhone);
                            _phoneMessagesCustodian.AddPhoneHistory(newPhone.NamePhone.Key);
                            _phonesDictionary.Add(newPhone.NamePhone.Key, newPhone);
                        }
                    }
                }
            }
        }
    }

    private Phone GetNewPhone(Phone phone)
    {
        var newPhone = new Phone(phone.BlockScreenTopPanelColor, phone.ContactsScreenTopPanelColor,
            phone.DialogScreenTopPanelColor,
            phone.NamePhone, phone.Hands, phone.PhoneFrame, phone.Background, phone.ToCharacterNameKey);
        return newPhone;
    }
}