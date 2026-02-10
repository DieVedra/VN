using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ConfirmedPanelUIHandler
{
    private LoadIndicatorUIHandler _loadIndicatorUIHandler;
    private BlackFrameUIHandler _darkeningBackgroundFrameUIHandler;
    private readonly ConfirmedPanelAssetProvider _confirmedPanelAssetProvider;
    private Transform _parent;
    private GlobalCanvasCloser _globalCanvasCloser;
    private ConfirmedPanelView _confirmedPanelView;
    public bool AssetIsLoaded { get; private set; }
    
    public ConfirmedPanelUIHandler()
    {
        AssetIsLoaded = false;
        _confirmedPanelAssetProvider = new ConfirmedPanelAssetProvider();
    }

    public void Init(Transform parent, GlobalCanvasCloser globalCanvasCloser,
        LoadIndicatorUIHandler loadIndicatorUIHandler, BlackFrameUIHandler darkeningBackgroundFrameUIHandler)
    {
        _parent = parent;
        _globalCanvasCloser = globalCanvasCloser;
        _loadIndicatorUIHandler = loadIndicatorUIHandler;
        _darkeningBackgroundFrameUIHandler = darkeningBackgroundFrameUIHandler;
    }
    public void Shutdown()
    {
        if (_confirmedPanelView != null)
        {
            Addressables.ReleaseInstance(_confirmedPanelView.gameObject);
        }
    }
    public async UniTask Show(string labelText, string transcriptionText, string buttonText, float heightPanel, int fontSizeValue,
        Action operationPutTrue, Action operationPutFalse, bool blackFrameNotOpen = false)
    {
        _darkeningBackgroundFrameUIHandler.CloseTranslucent().Forget();
        if (AssetIsLoaded == false)
        {
            _loadIndicatorUIHandler.SetClearIndicateMode();
            _loadIndicatorUIHandler.StartIndicate();
            _confirmedPanelView = await _confirmedPanelAssetProvider.CreateConfirmedPanel(_parent);
            AssetIsLoaded = true;
        }
        else
        {
            _confirmedPanelView.transform.SetAsLastSibling();
        }
        _globalCanvasCloser.TryEnable();
        _confirmedPanelView.TextLabel.text = labelText;
        _confirmedPanelView.TextTranscription.text = transcriptionText;
        _confirmedPanelView.TextButton.text = buttonText;
        _confirmedPanelView.TextButton.fontSize = fontSizeValue;

        Vector2 sizeDelta = _confirmedPanelView.RectTransform.sizeDelta;
        sizeDelta.y = heightPanel;
        _confirmedPanelView.RectTransform.sizeDelta = sizeDelta;
        
        _confirmedPanelView.ExitButton.onClick.AddListener(() =>
        {
            UnsubscribeButtons();
            Hide(operationPutFalse, false).Forget();
            _globalCanvasCloser.TryDisable();
        });
        
        _confirmedPanelView.ConfirmedButton.onClick.AddListener(() =>
        {
            UnsubscribeButtons();
            Hide(operationPutTrue, blackFrameNotOpen).Forget();
            _globalCanvasCloser.TryDisable();
        });
        _confirmedPanelView.gameObject.SetActive(true);
        _loadIndicatorUIHandler.StopIndicate();
    }

    private async UniTask Hide(Action operation, bool blackFrameNotOpen)
    {
        _confirmedPanelView.gameObject.SetActive(false);
        operation?.Invoke();
        if (blackFrameNotOpen == false)
        {
            await _darkeningBackgroundFrameUIHandler.OpenTranslucent();
        }
    }

    private void UnsubscribeButtons()
    {
        _confirmedPanelView.ConfirmedButton.onClick.RemoveAllListeners();
        _confirmedPanelView.ExitButton.onClick.RemoveAllListeners();
    }
}