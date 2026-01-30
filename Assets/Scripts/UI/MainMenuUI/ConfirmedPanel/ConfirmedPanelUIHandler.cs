using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ConfirmedPanelUIHandler
{
    private readonly LoadIndicatorUIHandler _loadIndicatorUIHandler;
    private readonly BlackFrameUIHandler _darkeningBackgroundFrameUIHandler;
    private readonly Transform _parent;
    private readonly ConfirmedPanelAssetProvider _confirmedPanelAssetProvider;
    private ConfirmedPanelView _confirmedPanelView;
    public bool AssetIsLoaded { get; private set; }
    
    public ConfirmedPanelUIHandler(LoadIndicatorUIHandler loadIndicatorUIHandler, BlackFrameUIHandler darkeningBackgroundFrameUIHandler, Transform parent)
    {
        _loadIndicatorUIHandler = loadIndicatorUIHandler;
        _darkeningBackgroundFrameUIHandler = darkeningBackgroundFrameUIHandler;
        _parent = parent;
        AssetIsLoaded = false;
        _confirmedPanelAssetProvider = new ConfirmedPanelAssetProvider();
    }

    public void Shutdown()
    {
        if (_confirmedPanelView != null)
        {
            Addressables.ReleaseInstance(_confirmedPanelView.gameObject);
        }
    }
    public async UniTask Show(string labelText, string transcriptionText, string buttonText, float heightPanel, int fontSizeValue,
        Action operation, bool blackFrameNotOpen = false)
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
        
        _confirmedPanelView.TextLabel.text = labelText;
        _confirmedPanelView.TextTranscription.text = transcriptionText;
        _confirmedPanelView.TextButton.text = buttonText;
        _confirmedPanelView.TextButton.fontSize = fontSizeValue;

        Vector2 sizeDelta = _confirmedPanelView.RectTransform.sizeDelta;
        sizeDelta.y = heightPanel;
        _confirmedPanelView.RectTransform.sizeDelta = sizeDelta;
        
        _confirmedPanelView.ExitButton.onClick.AddListener(() =>
        {
            Hide(null, false).Forget();
            UnsubscribeButtons();
        });
        
        _confirmedPanelView.ConfirmedButton.onClick.AddListener(() =>
        {
            Hide(operation, blackFrameNotOpen).Forget();
            UnsubscribeButtons();
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