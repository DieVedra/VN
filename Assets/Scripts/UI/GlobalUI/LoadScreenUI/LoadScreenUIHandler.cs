using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class LoadScreenUIHandler : ILocalizable
{
    private const float _awaitLoadContentFontSize = 70f;
    private const float _defaultFontSize = 50f;
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

    public LoadScreenUIHandler(LoadWordsHandler loadWordsHandler)
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

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        List<LocalizationString> content = new List<LocalizationString>();
        content.AddRange(_loadWordsHandler.GetLocalizableContent().ToList());
        content.Add(_disclaimerText);
        return content;
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
        _loadScreenUIView.LoadScreenImage.gameObject.SetActive(true);
        _loadScreenUIView.LogoImage.sprite = _logoSpriteDefault;
        _loadScreenUIView.LogoImage.gameObject.SetActive(true);
        SetDisclaimerText();
        _loadScreenUIView.transform.parent.gameObject.SetActive(true);
        await Show();
    }

    public async UniTask ShowToLevelMove(Sprite loadScreenSprite, Sprite logoImage)
    {
        _loadScreenUIView.LoadScreenImage.sprite = loadScreenSprite;
        _loadScreenUIView.LoadScreenImage.gameObject.SetActive(true);
        _loadScreenUIView.LogoImage.sprite = logoImage;
        _loadScreenUIView.LogoImage.gameObject.SetActive(true);
        _loadScreenUIView.DisclaimerText.text = string.Empty;
        _loadScreenUIView.gameObject.SetActive(false);
        _loadScreenUIView.transform.parent.gameObject.SetActive(true);
        await _blackFrameUIHandler.Close();
        await Show();
        _loadWordsHandler.StartSubstitutingWords(_loadScreenUIView.DisclaimerText).Forget();

    }

    public void ShowToAwaitLoadContent(LoadAssetsPercentHandler loadAssetsPercentHandler, string awaitText)
    {
        _loadScreenUIView.LoadScreenImage.sprite = null;
        _loadScreenUIView.LoadScreenImage.gameObject.SetActive(false);
        _loadScreenUIView.LogoImage.sprite = null;
        _loadScreenUIView.LogoImage.gameObject.SetActive(false);
        _loadScreenUIView.DisclaimerText.text = awaitText;
        _loadScreenUIView.DisclaimerText.fontSize = _awaitLoadContentFontSize;
        _loadScreenUIView.gameObject.SetActive(true);
        loadAssetsPercentHandler.StartCalculatePercent();
        _indicatorUIHandler.SetPercentIndicateMode(loadAssetsPercentHandler.CurrentLoadPercentReactiveProperty);
        _indicatorUIHandler.StartIndicate();
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

    public void HideToAwaitLoadContent(LoadAssetsPercentHandler loadAssetsPercentHandler)
    {
        loadAssetsPercentHandler.StopCalculatePercent();
        _loadScreenUIView.gameObject.SetActive(false);
        _indicatorUIHandler.StopIndicate();
        _loadScreenUIView.DisclaimerText.fontSize = _defaultFontSize;
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