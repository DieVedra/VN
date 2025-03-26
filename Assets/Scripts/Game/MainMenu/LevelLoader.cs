
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class LevelLoader
{
    private readonly StoriesProvider _storiesProvider;
    private readonly BlackFrameUIHandler _blackFrameUIHandler;
    private readonly LoadScreenUIHandler _loadScreenUIHandler;
    private readonly LoadIndicatorUIHandler _loadIndicatorUIHandler;
    private readonly Transform _parent;
    private readonly ReactiveCommand _onSceneTransition;
    private readonly SceneAssetLoader _sceneAssetLoader;
    private bool _firstPartLoadComplete;
    public LevelLoader(StoriesProvider storiesProvider, BlackFrameUIHandler blackFrameUIHandler,
        LoadScreenUIHandler loadScreenUIHandler, LoadIndicatorUIHandler loadIndicatorUIHandler,
        Transform parent, ReactiveCommand onSceneTransition)
    {
        _firstPartLoadComplete = false;
        _storiesProvider = storiesProvider;
        _blackFrameUIHandler = blackFrameUIHandler;
        _loadScreenUIHandler = loadScreenUIHandler;
        _loadIndicatorUIHandler = loadIndicatorUIHandler;
        _parent = parent;
        _onSceneTransition = onSceneTransition;
        _sceneAssetLoader = new SceneAssetLoader();
    }

    public async UniTask StartLoadPart1(Story story)
    {
        SaveServiceProvider.CurrentStoryIndex = story.MyIndex;
        await UniTask.WhenAll(
            _blackFrameUIHandler.Close(),
            _loadScreenUIHandler.Init(_parent, story.SpriteStorySkin, story.SpriteLogo),
            _loadIndicatorUIHandler.Init(_parent));
        
        _loadScreenUIHandler.SetAsLastSibling();
        _blackFrameUIHandler.SetAsLastSibling();
        _loadScreenUIHandler.Show();
        _loadIndicatorUIHandler.SetPercentIndicateMode(_sceneAssetLoader.LastPercentLoadValue);
        _sceneAssetLoader.OnLoadPercentUpdate += _loadIndicatorUIHandler.TextPercentIndicate;
        _loadIndicatorUIHandler.StartIndicate(_loadScreenUIHandler.ParentMask);
        _sceneAssetLoader.StartLoad(story.NameSceneAsset).Forget();
        _sceneAssetLoader.OnCompleteLoad += x => { StartLoadPart2(x).Forget();};
        await _blackFrameUIHandler.Open();
        _firstPartLoadComplete = true;
    }
    private async UniTask StartLoadPart2(AsyncOperationHandle<SceneInstance> operationHandle)
    {
        await UniTask.WaitUntil(() => _firstPartLoadComplete == true);
        await  _blackFrameUIHandler.Close();
        _loadIndicatorUIHandler.Dispose();
        _onSceneTransition.Execute();
        _sceneAssetLoader.Activate(operationHandle);
    }
}