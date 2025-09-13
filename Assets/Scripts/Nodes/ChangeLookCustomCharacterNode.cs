using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[NodeWidth(350),NodeTint("#D21B8C")]
public class ChangeLookCustomCharacterNode : BaseNode
{
    [SerializeField] private int _customizationCharacterIndex;
    [SerializeField] private int _hairstyleIndex;
    [SerializeField] private int _clothesIndex;
    [SerializeField] private int _swimsuitIndex;
    
    [SerializeField] private bool _putSwimsuit;
    [SerializeField] private bool _putClothes;
    [SerializeField] private bool _putHairstyle;
    [SerializeField] private bool _skipToWardrobeVariant;

    private Dictionary<int, CustomizableCharacter> _customizableCharactersDictionary;
    public IReadOnlyList<CustomizableCharacter> CustomizableCharacters { get; private set; }
    public void InitMyChangeLookCustomCharacterNode(IReadOnlyList<CustomizableCharacter> customizableCharacters)
    {
        CustomizableCharacters = customizableCharacters;
        InitDictionar(ref _customizableCharactersDictionary, customizableCharacters);
    }

    private void InitDictionar(ref Dictionary<int, CustomizableCharacter> customizableCharactersDictionary, IReadOnlyList<CustomizableCharacter> customizableCharacters)
    {
        customizableCharactersDictionary = new Dictionary<int, CustomizableCharacter>();
        for (int i = 0; i < customizableCharacters.Count; i++)
        {
            customizableCharactersDictionary.Add(i, customizableCharacters[i]);
        }
    }
    public override UniTask Enter(bool isMerged = false)
    {
        return default;
    }

    // public override async UniTask Exit()
    // {
    //     
    // }

    // public void 
}