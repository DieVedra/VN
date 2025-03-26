
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ConfirmedPanelUIHandler
{
    private readonly LoadIndicatorUIHandler _loadIndicatorUIHandler;
    private readonly BlackFrameUIHandler _blackFrameUIHandler;
    private readonly Transform _parent;
    private readonly ConfirmedPanelAssetProvider _confirmedPanelAssetProvider;
    private ConfirmedPanelView _confirmedPanelView;
    public bool AssetIsLoaded { get; private set; }
    
    public ConfirmedPanelUIHandler(LoadIndicatorUIHandler loadIndicatorUIHandler, BlackFrameUIHandler blackFrameUIHandler, Transform parent)
    {
        _loadIndicatorUIHandler = loadIndicatorUIHandler;
        _blackFrameUIHandler = blackFrameUIHandler;
        _parent = parent;
        AssetIsLoaded = false;
        _confirmedPanelAssetProvider = new ConfirmedPanelAssetProvider();
    }

    public async UniTask Show(string labelText, string transcriptionText, string buttonText, float heightPanel, int fontSizeValue,
        Action operation, bool blackFrameNotOpen = false)
    {
        _blackFrameUIHandler.CloseTranslucent().Forget();

        if (AssetIsLoaded == false)
        {
            
            await _loadIndicatorUIHandler.Init(_blackFrameUIHandler.Transform);
            _loadIndicatorUIHandler.SetClearIndicateMode();
            _loadIndicatorUIHandler.StartIndicate();
            
            _confirmedPanelView = await _confirmedPanelAssetProvider.LoadAsset(_parent);
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
            await _blackFrameUIHandler.OpenTranslucent();
        }
    }

    private void UnsubscribeButtons()
    {
        _confirmedPanelView.ConfirmedButton.onClick.RemoveAllListeners();
        _confirmedPanelView.ExitButton.onClick.RemoveAllListeners();
    }
}