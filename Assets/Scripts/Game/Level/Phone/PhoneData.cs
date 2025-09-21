using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PhoneData", menuName = "Phone/PhoneData", order = 51)]
public class PhoneData : ScriptableObject
{
    [SerializeField] private string _namePhone = "ElisPhone";
    [SerializeField] private string _keyCharacterName;
    [SerializeField] private List<PhoneContactData> _phoneContactDatas;
    
    // public Sprite GetSprite(string name) => _spriteAtlas1.GetSprite(name);
    public string NamePhone => _namePhone;
    public string KeyCharacterName => _keyCharacterName;
}
