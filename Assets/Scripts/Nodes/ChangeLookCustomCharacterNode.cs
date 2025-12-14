using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
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

    private ReactiveProperty<bool> _buferCurrentClothesIsActiveRP;
    private ReactiveProperty<int> _buferCurrentClothesIndexRP;
    private ReactiveProperty<int> _clothesIndexRP;
    
    private ReactiveProperty<bool> _buferCurrentSwimsuitsIsActiveRP;
    private ReactiveProperty<int> _buferCurrentSwimsuitsIndexRP;
    private ReactiveProperty<int> _swimsuitsIndexRP;
    
    private ReactiveProperty<bool> _buferCurrentHairstyleIsActiveRP;
    private ReactiveProperty<int> _buferCurrentHairstyleIndexRP;
    private ReactiveProperty<int> _hairstyleIndexRP;
    public IReadOnlyList<CustomizableCharacter> CustomizableCharacters { get; private set; }
    public void InitMyChangeLookCustomCharacterNode(IReadOnlyList<CustomizableCharacter> customizableCharacters)
    {
        CustomizableCharacters = customizableCharacters;
        if (IsPlayMode())
        {
            var currentCustomizableCharacterIndexesCustodian =
                CustomizableCharacters[_customizationCharacterIndex].CustomizableCharacterIndexesCustodian;
            
            _buferCurrentClothesIsActiveRP = currentCustomizableCharacterIndexesCustodian.BufferCurrentClothesIsActiveRP;
            _buferCurrentClothesIndexRP = currentCustomizableCharacterIndexesCustodian.BufferCurrentClothesIndexRP;
            _clothesIndexRP = currentCustomizableCharacterIndexesCustodian.ClothesIndexRP;
                
            _buferCurrentSwimsuitsIsActiveRP = currentCustomizableCharacterIndexesCustodian.BufferCurrentSwimsuitsIsActiveRP;
            _buferCurrentSwimsuitsIndexRP = currentCustomizableCharacterIndexesCustodian.BufferCurrentSwimsuitsIndexRP;
            _swimsuitsIndexRP = currentCustomizableCharacterIndexesCustodian.SwimsuitsIndexRP;

            _buferCurrentHairstyleIsActiveRP = currentCustomizableCharacterIndexesCustodian.BufferCurrentHairstyleIsActiveRP;
            _buferCurrentHairstyleIndexRP = currentCustomizableCharacterIndexesCustodian.BufferCurrentHairstyleIndexRP;
            _hairstyleIndexRP = currentCustomizableCharacterIndexesCustodian.HairstyleIndexRP;
        }
    }
    public override UniTask Enter(bool isMerged = false)
    {
        if (_skipToWardrobeVariant == false)
        {
            TryPutLook(_buferCurrentSwimsuitsIsActiveRP,
                _buferCurrentSwimsuitsIndexRP,
                _swimsuitsIndexRP,
                _putSwimsuit, _swimsuitIndex);
            
            TryPutLook(_buferCurrentClothesIsActiveRP,
                _buferCurrentClothesIndexRP,
                _clothesIndexRP,
                _putClothes, _clothesIndex);
            
            TryPutLook(_buferCurrentHairstyleIsActiveRP,
                _buferCurrentHairstyleIndexRP,
                _hairstyleIndexRP,
                _putHairstyle, _hairstyleIndex);
        }
        else
        {
            TryUnPutLook(ref _putSwimsuit, _buferCurrentSwimsuitsIsActiveRP,
                _buferCurrentSwimsuitsIndexRP,
                _swimsuitsIndexRP);
            
            TryUnPutLook(ref _putClothes, _buferCurrentClothesIsActiveRP,
                _buferCurrentClothesIndexRP,
                _clothesIndexRP);
            
            TryUnPutLook(ref _putHairstyle, _buferCurrentHairstyleIsActiveRP,
                _buferCurrentHairstyleIndexRP,
                _hairstyleIndexRP);
        }
        SwitchToNextNodeEvent.Execute();
        return default;
    }

    private void TryPutLook(ReactiveProperty<bool> buferCurrentIsActiveRP, ReactiveProperty<int> buferCurrentIndexRP, ReactiveProperty<int> indexRP,
        bool putKey, int lookIndex)
    {
        if (putKey == true && buferCurrentIsActiveRP.Value == false)
        {
            buferCurrentIsActiveRP.Value = putKey;
            buferCurrentIndexRP.Value = indexRP.Value;
            indexRP.Value = lookIndex;
        }
    }
    private void TryUnPutLook(ref bool putKey, ReactiveProperty<bool> buferCurrentIsActiveRP, 
        ReactiveProperty<int> buferCurrentIndexRP, ReactiveProperty<int> indexRP)
    {
        if (buferCurrentIsActiveRP.Value == true)
        {
            putKey = false;
            buferCurrentIsActiveRP.Value = false;
            indexRP.Value = buferCurrentIndexRP.Value;
        }
    }
}