using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ChoicePanelUIHandler
{
    private readonly int _siblingIndex;
    private readonly ChoiceCaseView[] _choiseCasesViews;
    private readonly ChoicePanelUI _choicePanelUI;
    private readonly Wallet _wallet;
    private readonly PanelResourceHandler _panelResourceHandler;
    private readonly ChoiceHeightHandler _choiceHeightHandler;
    private readonly ChoiceNodeTimer _choiceNodeTimer;
    private readonly ChoiceNodePriceHandler _choiceNodePriceHandler;
    private readonly ChoiceNodeButtonsHandler _choiceNodeButtonsHandler;
    private readonly RectTransform _rectTransform; 
    private CompositeDisposable _compositeDisposableOnUpdateWallet;
    private ReactiveProperty<bool> _choiceActive;
    public ChoiceNodeButtonsHandler ChoiceNodeButtonsHandler => _choiceNodeButtonsHandler;
    public RectTransform RectTransform => _rectTransform;

    public ChoicePanelUIHandler(ChoicePanelUI choicePanelUI, Wallet wallet, PanelResourceHandler panelResourceHandler, IChoicePanelInitializer choicePanelInitializer)
    {
        _choicePanelUI = choicePanelUI;
        _wallet = wallet;
        _panelResourceHandler = panelResourceHandler;
        _choiseCasesViews = choicePanelInitializer.GetChoiceCaseViews(choicePanelUI.transform);
        _rectTransform = choicePanelUI.transform as RectTransform;
        _choiceNodeTimer = new ChoiceNodeTimer(choicePanelUI.TimerPanelText, choicePanelUI. TimerPanelCanvasGroup, choicePanelUI.TimerImageRectTransform);
        _choiceNodePriceHandler = new ChoiceNodePriceHandler(_choiseCasesViews, wallet);
        
        _choiceActive = new ReactiveProperty<bool>(false);
        _choiceNodeButtonsHandler = new ChoiceNodeButtonsHandler(_choiseCasesViews, _choiceNodePriceHandler, wallet, choicePanelUI, _choiceActive);

        _choiceHeightHandler = new ChoiceHeightHandler(_choiseCasesViews, choicePanelUI);
        _siblingIndex = _rectTransform.GetSiblingIndex();
    }

    public void Dispose()
    {
        _compositeDisposableOnUpdateWallet?.Clear();
        _panelResourceHandler.Dispose();
    }
    public void SetSibling(int index)
    {
        _rectTransform.SetSiblingIndex(index);
    }
    public void ResetSibling()
    {
        _rectTransform.SetSiblingIndex(_siblingIndex);
    }
    public void ShowChoiceVariants(ChoiceData data)
    {
        SetTexts(data);
        _choicePanelUI.gameObject.SetActive(true);
        _choiceNodePriceHandler.TryShowPrices(data);
        _choiceNodeButtonsHandler.CheckChoiceButtonsCanPress(data);
        _choiceHeightHandler.UpdateHeights(data);
        _choiceNodeTimer.TrySetTimerValue(data.TimerValue);
        for (int i = 0; i < data.ChoiceCases.Count; i++)
        {
            SetCanvasGroupStartValue(_choiseCasesViews[i].CanvasGroupChoice, _choiceNodeButtonsHandler.ChoiseButtonsCanPress[i]);
        }
    }

    public void HideChoiceVariants()
    {
        ResetTexts();
        _choicePanelUI.gameObject.SetActive(false);
        _choiceNodeTimer.TrySetTimerValue();
        SetZeroAlphaToCanvasGroups();
    }

    public async UniTask ShowChoiceVariantsInPlayMode(CancellationToken cancellationToken, ChoiceData data,
        ChoiceResultEvent<ChoiceCase> choiceResultEvent)
    {
        _panelResourceHandler.Init(CalculateResourceViewMode(data));
        _choiceNodeButtonsHandler.Reset();
        _choiceNodeButtonsHandler.CheckChoiceButtonsCanPress(data);
        OnUpdateWallet(data, choiceResultEvent);
        
        SetTexts(data);
        _choicePanelUI.gameObject.SetActive(true);
        _choiceNodePriceHandler.TryShowPrices(data);
        _choiceHeightHandler.UpdateHeights(data);
        _choiceNodeTimer.TrySetTimerValue(data.TimerValue);
        _choiceNodeTimer.TryShowTimerPanelAnim(cancellationToken).Forget();
        _panelResourceHandler.Show().Forget();
        SetZeroAlphaToCanvasGroups();
        await _choiceNodeButtonsHandler.ShowButtons(data, cancellationToken);
    }

    public async UniTask DisappearanceChoiceVariantsInPlayMode(CancellationToken cancellationToken)
    {
        _compositeDisposableOnUpdateWallet.Clear();
        _choiceNodeTimer.TryHideTimerPanelAnim(cancellationToken);
        await _choiceNodeButtonsHandler.HideButtons(cancellationToken);
        if (_choiceNodePriceHandler.PriceExists == true)
        {
            _panelResourceHandler.TryHidePanel(delay:ChoicePanelUIValues.PriceExistsDurationValue).Forget();
        }
        else
        {
            _panelResourceHandler.TryHidePanel().Forget();
        }
        _choicePanelUI.gameObject.SetActive(false);
    }

    public void ActivateTimerChoice(ChoiceResultEvent<ChoiceCase> choiceResultEvent, int index, ChoiceCase choiceCaseResult, CancellationToken cancellationToken)
    {
        _choiceNodeTimer.TryStartTimer(choiceResultEvent,
            () =>
            {
                _choiceNodeButtonsHandler.DeactivateButtonsChoice();
                _choiceActive.Value = false;
                _choiceNodePriceHandler.Debit(index);
            }, choiceCaseResult, cancellationToken).Forget();
    }

    public void SetTexts(ChoiceData data)
    {
        ResetTexts();
        for (int i = 0; i < data.ButtonsCount; i++)
        {
            SetTextButton(_choiseCasesViews[i].ButtonChoice, _choiseCasesViews[i].TextButtonChoice, data.ChoiceCases[i].GetLocalizationString(), true);
        }
    }

    private void SetTextButton(Button button, TextMeshProUGUI textComponent, string text = " ", bool key = false)
    {
        SetActiveButton(button, key);
        textComponent.text = text;
    }
    private void SetActiveButton(Button button, bool key)
    {
        button.gameObject.SetActive(key);
    }
    private void ResetTexts()
    {
        for (int i = 0; i < _choiseCasesViews.Length; i++)
        {
            SetTextButton(_choiseCasesViews[i].ButtonChoice, _choiseCasesViews[i].TextButtonChoice);
        }
    }

    private void SetCanvasGroupStartValue(CanvasGroup canvasGroup, bool choiceButtonCanPress)
    {
        if (choiceButtonCanPress)
        {
            canvasGroup.alpha = ChoicePanelUIValues.PriceAvailableFadeValue;
        }
        else
        {
            canvasGroup.alpha = ChoicePanelUIValues.PriceNotAvailableFadeValue;
        }
        canvasGroup.blocksRaycasts = choiceButtonCanPress;
    }

    private void SetZeroAlphaToCanvasGroups()
    {
        for (int i = 0; i < _choiseCasesViews.Length; i++)
        {
            _choiseCasesViews[i].CanvasGroupChoice.alpha = ChoicePanelUIValues.MinValue;
        }
    }

    private ResourcesViewMode CalculateResourceViewMode(ChoiceData data)
    {
        return new ResourcesViewModeCalculator().CalculateResourcesViewMode((data.AllPrice, data.AllAdditionaryPrice));
    }

    private void ShowChoiceVariantsAfterWalletUpdate(ChoiceData data, ChoiceResultEvent<ChoiceCase> choiceResultEvent)
    {
        _choiceNodeButtonsHandler.CheckChoiceButtonsCanPress(data);
        for (int i = 0; i < data.ChoiceCases.Count; i++)
        {
            SetCanvasGroupStartValue(_choiseCasesViews[i].CanvasGroupChoice, _choiceNodeButtonsHandler.ChoiseButtonsCanPress[i]);
        }
        if (_choiceActive.Value == true)
        {
            _choiceNodeButtonsHandler.TryActivateButtonsChoice(data, choiceResultEvent);
        }
    }

    private void OnUpdateWallet(ChoiceData data,
        ChoiceResultEvent<ChoiceCase> choiceResultEvent)
    {
        _compositeDisposableOnUpdateWallet = new CompositeDisposable();
        _wallet.MonetsCountChanged.Subscribe(_ =>
        {
            ShowChoiceVariantsAfterWalletUpdate(data, choiceResultEvent);
        }).AddTo(_compositeDisposableOnUpdateWallet);
        _wallet.HeartsCountChanged.Subscribe(_ =>
        {
            ShowChoiceVariantsAfterWalletUpdate(data, choiceResultEvent);
        }).AddTo(_compositeDisposableOnUpdateWallet);
    }
}