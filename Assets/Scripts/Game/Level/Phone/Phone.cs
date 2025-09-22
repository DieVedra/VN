using System;

[Serializable]
public class Phone
{
    private PhoneData _phoneData;
    public string NamePhone { get; private set; }
    public Phone(string namePhone)
    {
        NamePhone = namePhone;
    }

    public void AddPhoneData(PhoneData phoneData)
    {
        _phoneData = phoneData;
    }
}