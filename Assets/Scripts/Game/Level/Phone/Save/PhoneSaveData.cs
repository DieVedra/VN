using System;
using System.Collections.Generic;

[Serializable]
public class PhoneSaveData
{
    public string PhoneNameKey;
    public List<string> ContactsKeys;
    public Dictionary<string, List<PhoneMessage>> MessageHistory;
}