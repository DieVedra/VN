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
    public List<string> Names => _names;
    public MatchNumbersHandler MatchNumbersHandler => _matchNumbersHandler;

    public bool AssetsFinded { get; private set; }
    public bool ParticipiteInLoad { get; private set; }
    public int PercentComplete => _scriptableObjectAssetLoader.GetPercentComplete();
    public int NamesCount => _names.Count;
    public IReadOnlyList<T> GetDatas => _datas;
    public T LastLoaded { get; private set; }

    public readonly ReactiveCommand<T> OnLoad;
    protected CompositeDisposable BaseCompositeDisposable;
    private CompositeDisposable _compositeDisposableOnLoad;

    public DataProvider()
    {
        _datas = new List<T>();
        _compositeDisposableOnLoad = new CompositeDisposable();
        OnLoad = new ReactiveCommand<T>().AddTo(_compositeDisposableOnLoad);
        _assetExistsHandler = new AssetExistsHandler();
        _scriptableObjectAssetLoader = new ScriptableObjectAssetLoader();
        _matchNumbersHandler = new MatchNumbersHandler();
        AssetsFinded = false;
    }

    public void SetDefault()
    {
        _scriptableObjectAssetLoader.SetDefault();
    }

    public void Shutdown()
    {
        _compositeDisposableOnLoad?.Clear();
        BaseCompositeDisposable?.Clear();
        _scriptableObjectAssetLoader.TryAbortLoad();
        ParticipiteInLoad = false;
    }

    public async UniTask CreateNames(string nameAsset)
    {
        _names = await _assetExistsHandler.CheckExistsAssetsNames(nameAsset);
        if (_names.Count > 0)
            AssetsFinded = true;
    }
    public bool CheckMatchNumbersSeriaWithNumberAsset(int nextSeriaNumber, int nextSeriaNameAssetIndex)
    {
        if (AssetsFinded == true)
        {
            return ParticipiteInLoad = _matchNumbersHandler.CheckMatchNumbersSeriaWithNumberAsset(_names, nextSeriaNumber, nextSeriaNameAssetIndex);
        }
        else
        {
            return ParticipiteInLoad = false;
        }
    }
    public async UniTask<bool> TryLoadData(int nextSeriaNameAssetIndex)
    {
        if (ParticipiteInLoad == true)
        {
            var data = await _scriptableObjectAssetLoader.Load<T>(_names[nextSeriaNameAssetIndex]);
            _datas.Add(data);
            OnLoad.Execute(data);
            LastLoaded = data;
            return true;
        }
        else
        {
            return false;
        }
    }
    public async UniTask<T> TryLoadDataAndGet(int nextSeriaNameAssetIndex)
    {
        if (ParticipiteInLoad == true)
        {
            T data = await _scriptableObjectAssetLoader.Load<T>(_names[nextSeriaNameAssetIndex]);
            _datas.Add(data);
            OnLoad.Execute(data);
            LastLoaded = data;
            return data;
        }
        else
        {
            return default;
        }
    }
}