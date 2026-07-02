using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;

public class test : MonoBehaviour
{
    private const string _locations = "Locations";
    private const string _additionalImages = "AdditionalImages";
    private const string _arts = "Arts";
    private const string _compressed = "Compressed";
    private const string _backgroundDataSeriaNameAsset = "BackgroundDataSeria";
    private const string _wardrobeBackgroundDataNameAsset = "WardrobeBackgroundData";
    
    private readonly string _storyName = "TheWildBeach";
    [SerializeField] private ReactiveProperty<bool> AssetsFinded = new ReactiveProperty<bool>();
    [SerializeField] private ReactiveProperty<bool> ParticipiteInLoad = new ReactiveProperty<bool>();
    [SerializeField] private List<string> _names;
    [SerializeField] private int nextSeriaNumber, nextSeriaNameAssetIndex;
    private readonly MatchNumbersHandler _matchNumbersHandler = new MatchNumbersHandler();
    private readonly AssetExistsHandler _assetExistsHandler = new AssetExistsHandler();
    [SerializeField] private string _name;

    private async void Awake()
    {
        await Addressables.InitializeAsync();
    }

    [Button()]
    private void test3()
    {
        StringBuilder _stringBuilder = new StringBuilder();
        char symbol;
        for (int i = 0; i < _name.Length; i++)
        {
            symbol = _name[i];
            if (symbol >= '0' && symbol <= '9')
            {
                _stringBuilder.Append(symbol);
            }
        }
        Debug.Log($"{_stringBuilder.ToString()}");
    }
    [Button()]
    private void test1()
    {
    }
    
    [Button()]
    private void CreateNamesButton()
    {
        Debug.Log($"GetNameArts {GetNameArts()}");
        CreateNames(GetNameArts()).Forget();
    }
    
    private async UniTask CreateNames(string nameAsset)
    {
        _names = await _assetExistsHandler.CheckExistsAssetsNames(nameAsset);
        if (_names.Count > 0)
            AssetsFinded.Value = true;
    }
    
    
    [Button()]
    public void CheckMatchNumbersSeriaWithNumberAsset()
    {
    }
    private string GetNameArts(bool HDMode = false)
    {
        if (HDMode == false)
        {
            return $"{_storyName}{_arts}{_compressed}{_backgroundDataSeriaNameAsset}";
        }
        else
        {
            return $"{_storyName}{_arts}{_backgroundDataSeriaNameAsset}";
        }
    }
}