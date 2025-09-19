using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[CreateAssetMenu(fileName = "PhoneData", menuName = "Phone/PhoneData", order = 51)]
public class PhoneData : ScriptableObject
{
    public const string NamePhone = "ElisPhone";
    [SerializeField] private SpriteAtlas _spriteAtlas1;
    [SerializeField] private SpriteAtlas _spriteAtlas2;
    [SerializeField] private SpriteAtlas _spriteAtlas3;
    [SerializeField] private List<BackgroundContentValues> _backgroundContentValues;
    [SerializeField] private List<PhoneContactData> _phoneContacts;
    
    public Sprite GetSprite(string name) => _spriteAtlas1.GetSprite(name);

}
[Serializable]
public class PhoneContactData
{
    [field:SerializeField] public string Name { get; private set; }
    [SerializeField] private Sprite _icon;
    [SerializeField] private bool _withIcon;
    [SerializeField] private LocalizationString _nameContact;
    [SerializeField] private List<PhoneMessage> PhoneMessages;
}

[Serializable]
public class PhoneMessage
{
    public string Text;
    public string Data;
}
