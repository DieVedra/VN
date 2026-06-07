using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameEndPanelHandler
{
    public readonly LocalizationString TextLabel;
    public readonly LocalizationString TextDescription;
    public readonly LocalizationString ButtonBackToMenu = "В меню";
    private readonly LoadIndicatorUIHandler _loadIndicatorUIHandler;
    private readonly BlackFrameUIHandler _blackFrameUIHandler;
    private readonly Transform _parent;
    private readonly ButtonTransitionToMainSceneUIHandler _buttonTransitionToMainSceneUIHandler;
    private readonly LevelUISpriteAtlasAssetProvider _levelUISpriteAtlasAssetProvider;
    private readonly GameStatsHandler _gameStatsHandler;
    private readonly StatCasesSpawner _statCasesSpawner;
    private readonly GameEndPanelAssetProvider _gameEndPanelAssetProvider;
    private GameEndPanelView _gameEndPanelView;

    public GameEndPanelHandler(GameStatsHandler gameStatsHandler, LocalizationString textLabel, LocalizationString textDescription,
        LoadIndicatorUIHandler loadIndicatorUIHandler, BlackFrameUIHandler blackFrameUIHandler,
        ButtonTransitionToMainSceneUIHandler buttonTransitionToMainSceneUIHandler,
        LevelUISpriteAtlasAssetProvider levelUISpriteAtlasAssetProvider, Transform parent,
        IconsUISpriteAtlasAssetProvider iconsUISpriteAtlasAssetProvider, SetLocalizationChangeEvent setLocalizationChangeEvent)
    {
        TextLabel = textLabel;
        TextDescription = textDescription;
        _loadIndicatorUIHandler = loadIndicatorUIHandler;
        _blackFrameUIHandler = blackFrameUIHandler;
        _parent = parent;
        _buttonTransitionToMainSceneUIHandler = buttonTransitionToMainSceneUIHandler;
        _levelUISpriteAtlasAssetProvider = levelUISpriteAtlasAssetProvider;
        _gameStatsHandler = gameStatsHandler;
        _gameEndPanelAssetProvider = new GameEndPanelAssetProvider();
        _statCasesSpawner = new StatCasesSpawner(gameStatsHandler, _gameEndPanelAssetProvider, iconsUISpriteAtlasAssetProvider, setLocalizationChangeEvent);
    }

    public async UniTask ShowPanel()
    {
        _blackFrameUIHandler.OpenTranslucent().Forget();
        _loadIndicatorUIHandler.SetClearIndicateMode();
        _loadIndicatorUIHandler.StartIndicate();
        _gameEndPanelView = await _gameEndPanelAssetProvider.LoadGameEndPanelPrefab(_parent);
        _loadIndicatorUIHandler.StopIndicate();
        
        await _statCasesSpawner.SpawnCases(_gameEndPanelView.StatContentRectTransform);

        _gameEndPanelView.TextLabel.text = TextLabel.DefaultText;
        _gameEndPanelView.TextDescription.text = TextDescription.DefaultText;

        _gameEndPanelView.TextButton.text = ButtonBackToMenu.DefaultText;
        
        _gameEndPanelView.ButtonBackToMenu.image.sprite = _levelUISpriteAtlasAssetProvider.GetSprite(LevelUISpriteAtlasAssetProvider.NarrativePanelName);
        _gameEndPanelView.gameObject.SetActive(true);
        _gameEndPanelView.ButtonBackToMenu.onClick.AddListener(() =>
        {
            _gameEndPanelView.ButtonBackToMenu.onClick.RemoveAllListeners();
            _buttonTransitionToMainSceneUIHandler.Press().Forget();
        });
    }

    public void Shutdown()
    {
        if (_gameEndPanelView != null)
        {
            _statCasesSpawner.Shutdown();
            GameObject gameObject;
            (gameObject = _gameEndPanelView.gameObject).SetActive(false);
            Addressables.ReleaseInstance(gameObject);
        }
    }
}