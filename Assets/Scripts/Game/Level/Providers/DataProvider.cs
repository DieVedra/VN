
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class DataProvider<T> : IParticipiteInLoad where T : ScriptableObject 
{
    private readonly ScriptableObjectAssetLoader _scriptableObjectAssetLoader;
    private readonly AssetExistsHandler _assetExistsHandler;
    private readonly MatchNumbersHandler _matchNumbersHandler;
    
    private readonly List<T> _datas;

    private List<string> _names;
    private bool _assetsFinded;
    public bool ParticipiteInLoad { get; private set; }
    public int PercentComplete => _scriptableObjectAssetLoader.GetPercentComplete();

    public IReadOnlyList<T> Datas => _datas;
    public readonly ReactiveCommand<T> OnLoad;

    public DataProvider()
    {
        _datas = new List<T>();
        OnLoad = new ReactiveCommand<T>();
        _assetExistsHandler = new AssetExistsHandler();
        _scriptableObjectAssetLoader = new ScriptableObjectAssetLoader();
        _matchNumbersHandler = new MatchNumbersHandler();
        _assetsFinded = false;
    }
    public void Dispose()
    {
        OnLoad.Dispose();
        _scriptableObjectAssetLoader.UnloadAll();
    }

    public async UniTask CreateNames(string nameAsset)
    {
        _names = await _assetExistsHandler.CheckExistsAssetsNames(nameAsset);
        if (_names.Count > 0)
            _assetsFinded = true;
    }
    public void CheckMatchNumbersSeriaWithNumberAsset(int nextSeriaNumber, int nextSeriaNameAssetIndex)
    {
        if (_assetsFinded == true)
        {
            ParticipiteInLoad = _matchNumbersHandler.CheckMatchNumbersSeriaWithNumberAsset(_names, nextSeriaNumber, nextSeriaNameAssetIndex);
        }
        else
        {
            ParticipiteInLoad = false;
        }
    }
    public async UniTask<bool> TryLoadData(int nextSeriaNameAssetIndex)
    {
        if (ParticipiteInLoad == true)
        {
            var data = await _scriptableObjectAssetLoader.Load<T>(_names[nextSeriaNameAssetIndex]);
            _datas.Add(data);
            OnLoad.Execute(data);
            return true;
        }
        else
        {
            return false;
        }
    }
}