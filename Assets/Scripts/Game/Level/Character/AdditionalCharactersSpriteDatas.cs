using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "AdditionalCharactersSpriteDatas", menuName = "Character/AdditionalCharactersSpriteDatas", order = 51)]
public class AdditionalCharactersSpriteDatas : ScriptableObject
{
    [SerializeField, Expandable] private List<WardrobeSeriaData> _additionalCustomizationCharactersSpriteDatas;
    [SerializeField, Expandable] private List<SimpleCharacter> _additionalSimpleCharactersSpriteDatas;
    
    public IReadOnlyList<WardrobeSeriaData> GetAdditionalCustomizationCharactersSpriteDatas => _additionalCustomizationCharactersSpriteDatas;
    public IReadOnlyList<SimpleCharacter> GetAdditionalSimpleCharactersSpriteDatas => _additionalSimpleCharactersSpriteDatas;
}