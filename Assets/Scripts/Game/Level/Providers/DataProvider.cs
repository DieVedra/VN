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
    private string _nameToLoad;
    private bool _participiteInLoad;
    public List<string> Names => _names;
    public MatchNumbersHandler MatchNumbersHandler => _matchNumbersHandler;

    public bool AssetsFinded { get; private set; }
    public bool ParticipiteInLoad => _participiteInLoad;
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
        _participiteInLoad = false;
    }

    public async UniTask CreateNames(string nameAsset)
    {
        _names = await _assetExistsHandler.CheckExistsAssetsNames(nameAsset);
        if (_names.Count > 0)
            AssetsFinded = true;
    }
    public bool CheckMatchNumbersSeriaWithNumberAsset(int nextSeriaNumber)
    {
        // Debug.Log($"CheckMatchNumbersSeriaWithNumberAsset AssetsFinded: {AssetsFinded} {this}");

        if (AssetsFinded == true)
        {
            // var ParticipiteInLoad = _matchNumbersHandler.CheckMatchNumbersSeriaWithNumberAsset(_names, nextSeriaNumber, nextSeriaNameAssetIndex);
            _participiteInLoad = _matchNumbersHandler.CheckMatchNumbersSeriaWithNumberAsset1(ref _nameToLoad, _names, nextSeriaNumber);
            // Debug.Log($"1 ParticipiteInLoad = {_participiteInLoad}    nextSeriaNumber: {nextSeriaNumber}    _nameToLoad: ||{_nameToLoad}||");
            return _participiteInLoad;
        }
        else
        {
            // Debug.Log($"2 ParticipiteInLoad = false");

            // return ParticipiteInLoad = false;
            _participiteInLoad = false;
            return _participiteInLoad;
        }
    }
    public async UniTask<bool> TryLoadData()
    {
        // foreach (var VARIABLE in _names)
        // {
        //     Debug.Log($"{VARIABLE}");
        // }

        // Debug.Log($"TryLoadData1 ParticipiteInLoad {_participiteInLoad}  {GetType()}   {this}  _name to load: ||{_nameToLoad}||");
        if (_participiteInLoad == true)
        {
            // var data = await _scriptableObjectAssetLoader.Load<T>(_names[nextSeriaNameAssetIndex]);
            var data = await _scriptableObjectAssetLoader.Load<T>(_nameToLoad);
            _datas.Add(data);
            OnLoad.Execute(data);
            LastLoaded = data;
            // Debug.Log($"Data Loaded  _nameToLoad: ||{_nameToLoad}||");
            return true;
        }
        else
        {
            // Debug.Log($"Data Not Loaded  _nameToLoad: ||{_nameToLoad}||");
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