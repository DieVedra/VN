
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelLoader
{
    private readonly StoriesProvider _storiesProvider;
    private readonly LoadScreenUIHandler _loadScreenUIHandler;
    private readonly Transform _parent;
    private readonly ReactiveCommand _onSceneTransition;
    private readonly SaveServiceProvider _saveServiceProvider;
    private readonly SceneAssetLoader _sceneAssetLoader;
    public LevelLoader(StoriesProvider storiesProvider, 
        LoadScreenUIHandler loadScreenUIHandler,
        Transform parent, ReactiveCommand onSceneTransition, SaveServiceProvider saveServiceProvider)
    {
        _storiesProvider = storiesProvider;
        _loadScreenUIHandler = loadScreenUIHandler;
        _parent = parent;
        _onSceneTransition = onSceneTransition;
        _saveServiceProvider = saveServiceProvider;
        _sceneAssetLoader = new SceneAssetLoader(_saveServiceProvider);
    }

    public async UniTask StartLoadPart1(Story story)
    {
        _saveServiceProvider.CurrentStoryIndex = story.MyIndex;
        await _loadScreenUIHandler.ShowOnLevelMove(story.SpriteStorySkin, story.SpriteLogo);
        Camera.main.gameObject.SetActive(false);
        EventSystem.current.gameObject.SetActive(false);
        await _sceneAssetLoader.SceneLoad(story.NameSceneAsset, _onSceneTransition);
    }
}