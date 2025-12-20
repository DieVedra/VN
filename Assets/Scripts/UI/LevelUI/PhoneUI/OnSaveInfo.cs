using System.Collections.Generic;

public class OnSaveInfo
{
    public string DialogContactKey;
    public int GetPhoneScreenIndex;
    public int PhoneContentNodeIndex;
    public int NotificationsInBlockScreenIndex;
    public int CurrentPhoneMinute;
    public bool NotificationPressed;
    public IReadOnlyList<int> ReadedContactNodeCaseIndexes;
    public IReadOnlyList<string> OnlineContactsKeys;
    public IReadOnlyList<string> NotificationsKeys;
}