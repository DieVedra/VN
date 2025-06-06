﻿
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LoadScreenUIHandler
{
    private readonly LocalizationString _disclaimerText = "Все персонажи и описываемые события вымышлены. Все совпадения случайны.";
    private readonly LoadScreenAssetProvider _loadScreenAssetProvider;
    private readonly LoadWordsHandler _loadWordsHandler;
    private BlackFrameUIHandler _blackFrameUIHandler;
    private LoadIndicatorUIHandler _indicatorUIHandler;
    private LoadScreenUIView _loadScreenUIView;
    private CancellationTokenSource _cancellationTokenSource;
    private Sprite _backgrountSpriteDefault;
    private Sprite _logoSpriteDefault;
    public Transform ParentMask => _loadScreenUIView.LoadScreenMaskImage.transform;
    public BlackFrameUIHandler BlackFrameUIHandler => _blackFrameUIHandler;
    public bool IsStarted { get; private set; }

    public LoadScreenUIHandler()
    {
        _loadScreenAssetProvider = new LoadScreenAssetProvider();
        _loadWordsHandler = new LoadWordsHandler();
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
        _indicatorUIHandler.Dispose();
        _blackFrameUIHandler.Dispose();
        _loadWordsHandler.StopSubstitutingWords();
    }

    public async UniTask Init(Transform parent, LoadIndicatorUIHandler loadIndicatorUIHandler, BlackFrameUIHandler blackFrameUIHandler)
    {
        _loadScreenUIView = await _loadScreenAssetProvider.LoadAsset(parent);
        _loadScreenUIView.RectMask2D.padding = Vector4.zero;
        _backgrountSpriteDefault = _loadScreenUIView.LoadScreenImage.sprite;
        _logoSpriteDefault = _loadScreenUIView.LogoImage.sprite;
        _indicatorUIHandler = loadIndicatorUIHandler;
        _blackFrameUIHandler = blackFrameUIHandler;
        _loadScreenUIView.LoadScreenImage.raycastTarget = true;
        _loadScreenUIView.LoadScreenMaskImage.raycastTarget = true;
    }
    private async UniTask Show()
    {
        _loadScreenUIView.gameObject.SetActive(true);
        _indicatorUIHandler.SetTextIndicateMode();
        _indicatorUIHandler.StartIndicate();
        await _blackFrameUIHandler.Open();
    }

    public async UniTask ShowToMainMenuMove()
    {
        _loadScreenUIView.LoadScreenImage.sprite = _backgrountSpriteDefault;
        _loadScreenUIView.LogoImage.sprite = _logoSpriteDefault;
        SetDisclaimerText();
        _loadScreenUIView.transform.parent.gameObject.SetActive(true);
        await Show();
    }
    public async UniTask ShowToLevelMove(Sprite loadScreenSprite, Sprite logoImage)
    {
        _loadScreenUIView.LoadScreenImage.sprite = loadScreenSprite;
        _loadScreenUIView.LogoImage.sprite = logoImage;
        _loadScreenUIView.DisclaimerText.text = string.Empty;
        _loadScreenUIView.gameObject.SetActive(false);
        _loadScreenUIView.transform.parent.gameObject.SetActive(true);
        await _blackFrameUIHandler.Close();
        await Show();
        _loadWordsHandler.StartSubstitutingWords(_loadScreenUIView.DisclaimerText).Forget();

    }
    public void ShowOnStart()
    {
        SetDisclaimerText();
        _loadScreenUIView.transform.parent.gameObject.SetActive(true);
        _blackFrameUIHandler.On();
        _loadScreenUIView.gameObject.SetActive(true);
        _indicatorUIHandler.StartIndicate();
        _blackFrameUIHandler.Open().Forget();
        IsStarted = true;
    }

    public async UniTask HideOnLevelMove()
    {
        await _blackFrameUIHandler.Close();
        _loadScreenUIView.gameObject.SetActive(false);
        _indicatorUIHandler.StopIndicate();
        _loadWordsHandler.StopSubstitutingWords();
        _blackFrameUIHandler.Open().Forget();
    }
    
    public async UniTaskVoid HideOnMainMenuMove()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        Vector4 padding = Vector4.zero;
        _loadScreenUIView.LoadScreenImage.raycastTarget = false;
        _loadScreenUIView.LoadScreenMaskImage.raycastTarget = false;

        _indicatorUIHandler.StopIndicate();
        
        float paddingValue = Screen.width;
        await DOTween.To(() => padding.z, x => padding.z = x, paddingValue, _loadScreenUIView.MaskHideDuration)
            .OnUpdate(() => _loadScreenUIView.RectMask2D.padding = padding).WithCancellation(_cancellationTokenSource.Token);
        _loadScreenUIView.gameObject.SetActive(false);
        _loadScreenUIView.RectMask2D.padding = Vector4.zero;
        _loadScreenUIView.transform.parent.gameObject.SetActive(false);
    }

    private void SetDisclaimerText()
    {
        _loadScreenUIView.DisclaimerText.text = _disclaimerText;
    }
}