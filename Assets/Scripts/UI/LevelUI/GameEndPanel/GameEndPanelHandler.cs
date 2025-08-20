using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameEndPanelHandler
{
    public readonly LocalizationString _textLabel = "Продолжение следует...";
    public readonly LocalizationString _textDescription = "Кринж?";
    public readonly LocalizationString _buttonBackToMenu = "В меню";

    private readonly LoadIndicatorUIHandler _loadIndicatorUIHandler;
    private readonly BlackFrameUIHandler _blackFrameUIHandler;
    private readonly Transform _parent;
    private readonly ButtonTransitionToMainSceneUIHandler _buttonTransitionToMainSceneUIHandler;

    public GameEndPanelHandler(LoadIndicatorUIHandler loadIndicatorUIHandler, BlackFrameUIHandler blackFrameUIHandler,
        ButtonTransitionToMainSceneUIHandler buttonTransitionToMainSceneUIHandler, Transform parent)
    {
        _loadIndicatorUIHandler = loadIndicatorUIHandler;
        _blackFrameUIHandler = blackFrameUIHandler;
        _parent = parent;
        _buttonTransitionToMainSceneUIHandler = buttonTransitionToMainSceneUIHandler;
    }

    public async UniTask ShowPanel()
    {
        _blackFrameUIHandler.OpenTranslucent().Forget();
        _loadIndicatorUIHandler.SetClearIndicateMode();
        _loadIndicatorUIHandler.StartIndicate();
        
        GameEndPanelView gameEndPanelView = await new GameEndPanelAssetProvider().LoadGameEndPanelPrefab(_parent);
        _loadIndicatorUIHandler.StopIndicate();
        gameEndPanelView.TextLabel.text = _textLabel.DefaultText;
        gameEndPanelView.TextDescription.text = _textDescription.DefaultText;
        gameEndPanelView.TextButton.text = _buttonBackToMenu.DefaultText;
        gameEndPanelView.gameObject.SetActive(true);
        
        gameEndPanelView.ButtonBackToMenu.onClick.AddListener(() =>
        {
            _buttonTransitionToMainSceneUIHandler.Press().Forget();
            gameEndPanelView.ButtonBackToMenu.onClick.RemoveAllListeners();
        });
    }
}