using System;
using System.Collections.Generic;
using System.ComponentModel;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    [SerializeField] private string nameasset;
    // [SerializeField, Expandable] private CharactersData _wardrobeSeriaData;
    [SerializeField, Expandable] private ScriptableObject _scriptable;
    [SerializeField] private List<string> _names;
    
    
    private ScriptableObjectAssetLoader _scriptableObjectAssetLoader;
    private AssetExistsHandler _assetExistsHandler;
    [Button()]
    private void Load()
    {
        load().Forget();

    }
    [Button()]
    private void Unload()
    {
        _scriptableObjectAssetLoader.UnloadAll();
        _scriptable = null;
        _names = null;
    }
    private async UniTaskVoid load()
    {
        await Addressables.InitializeAsync();
        _scriptableObjectAssetLoader = new ScriptableObjectAssetLoader();
        _assetExistsHandler = new AssetExistsHandler();

        _names = await _assetExistsHandler.CheckExistsAssetsNames(nameasset);
        _scriptable = await _scriptableObjectAssetLoader.Load<ScriptableObject>(_names[0]);

        if (_scriptable == null)
        {
            Debug.Log(11111);
        }
    }
}