using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ChoicePanelUIHandler
{
    private readonly ChoicePanelUI _choicePanelUI;
    private readonly Wallet _wallet;
    private readonly PanelResourceHandler _panelResourceHandler;
    private readonly ChoiceHeightHandler _choiceHeightHandler;
    private readonly ChoiceNodeTimer _choiceNodeTimer;
    private readonly ChoiceNodePriceHandler _choiceNodePriceHandler;
    private readonly ChoiceNodeButtonsHandler _choiceNodeButtonsHandler;
    private CompositeDisposable _compositeDisposableOnUpdateWallet;
    private ReactiveProperty<bool> _choiceActive;
    public ChoiceNodeButtonsHandler ChoiceNodeButtonsHandler => _choiceNodeButtonsHandler;
    
    public ChoicePanelUIHandler(ChoicePanelUI choicePanelUI, Wallet wallet, PanelResourceHandler panelResourceHandler)
    {
        _choicePanelUI = choicePanelUI;
        _wallet = wallet;
        _panelResourceHandler = panelResourceHandler;
        _choiceNodeTimer = new ChoiceNodeTimer(choicePanelUI.TimerPanelText, choicePanelUI. TimerPanelCanvasGroup, choicePanelUI.TimerImageRectTransform);
        
        _choiceNodePriceHandler = new ChoiceNodePriceHandler( 
            choicePanelUI.PriceRectTransformChoice1, choicePanelUI.PriceRectTransformChoice2, choicePanelUI.PriceRectTransformChoice3,
            choicePanelUI.AdditionaryPriceRectTransformChoice1, choicePanelUI.AdditionaryPriceRectTransformChoice2, choicePanelUI.AdditionaryPriceRectTransformChoice3,
            wallet, choicePanelUI.PriceButton1Text, choicePanelUI.PriceButton2Text, choicePanelUI.PriceButton3Text,
            choicePanelUI.AdditionaryPriceButton1Text, choicePanelUI.AdditionaryPriceButton2Text, choicePanelUI.AdditionaryPriceButton3Text);

        _choiceActive = new ReactiveProperty<bool>(false);
        _choiceNodeButtonsHandler = new ChoiceNodeButtonsHandler(_choiceNodePriceHandler, wallet, choicePanelUI, _choiceActive);

        _choiceHeightHandler = new ChoiceHeightHandler(choicePanelUI);
    }

    public void Dispose()
    {
        _compositeDisposableOnUpdateWallet?.Clear();
        _panelResourceHandler.Dispose();
    }
    public void ShowChoiceVariants(ChoiceData data)
    {
        SetTexts(data);
        _choicePanelUI.gameObject.SetActive(true);
        _choiceNodePriceHandler.TryShowPrices(data);
        _choiceNodeButtonsHandler.CheckChoiceButtonsCanPress(data);
        
        _choiceHeightHandler.UpdateHeights(data);
        
        _choiceNodeTimer.TrySetTimerValue(data.TimerValue);

        SetCanvasGroupStartValue(_choicePanelUI.CanvasGroupChoice1, _choiceNodeButtonsHandler.Choice1ButtonCanPress);
        SetCanvasGroupStartValue(_choicePanelUI.CanvasGroupChoice2, _choiceNodeButtonsHandler.Choice2ButtonCanPress);

        if (data.ShowChoice3 == true)
        {
            SetCanvasGroupStartValue(_choicePanelUI.CanvasGroupChoice3, _choiceNodeButtonsHandler.Choice3ButtonCanPress);
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
        ChoiceResultEvent<int> choiceResultEvent, bool keyButtonChoice3)
    {
        _panelResourceHandler.Init(CalculateResourceViewMode(data));
        _choiceNodeButtonsHandler.Reset();
        _choiceNodeButtonsHandler.CheckChoiceButtonsCanPress(data);
        OnUpdateWallet(data, choiceResultEvent, keyButtonChoice3);
        
        
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

    public async UniTask DisappearanceChoiceVariantsInPlayMode(CancellationToken cancellationToken, bool keyShowChoice3)
    {
        _compositeDisposableOnUpdateWallet.Clear();
        _choiceNodeTimer.TryHideTimerPanelAnim(cancellationToken);
        await _choiceNodeButtonsHandler.HideButtons(cancellationToken, keyShowChoice3);
        if (_choiceNodePriceHandler.PriceExists == true)
        {
            _panelResourceHandler.TryHidePanel(delay:ChoicePanelUIValues.PriceExistsDurationValue).Forget();
        }
        else
        {
            _panelResourceHandler.TryHidePanel().Forget();
        }
        _choicePanelUI.gameObject.SetActive(false);
        _panelResourceHandler.Dispose();
    }

    public void ActivateTimerChoice(ChoiceResultEvent<int> choiceResultEvent, int index, CancellationToken cancellationToken)
    {
        _choiceNodeTimer.TryStartTimer(choiceResultEvent,
            () =>
            {
                _choiceNodeButtonsHandler.DeactivateButtonsChoice();
                _choiceActive.Value = false;
                _choiceNodePriceHandler.Debit(index);
            }, index, cancellationToken).Forget();
    }

    public void SetTexts(ChoiceData data)
    {
        ResetTexts();
        SetTextButton(_choicePanelUI.ButtonChoice1, _choicePanelUI.TextButtonChoice1, data.Text1, true);
        SetTextButton(_choicePanelUI.ButtonChoice2, _choicePanelUI.TextButtonChoice2, data.Text2, true);
        if (data.ShowChoice3 == true)
        {
            SetTextButton(_choicePanelUI.ButtonChoice3, _choicePanelUI.TextButtonChoice3, data.Text3, true);
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
        SetTextButton(_choicePanelUI.ButtonChoice1, _choicePanelUI.TextButtonChoice1);
        SetTextButton(_choicePanelUI.ButtonChoice2, _choicePanelUI.TextButtonChoice2);
        SetTextButton(_choicePanelUI.ButtonChoice3, _choicePanelUI.TextButtonChoice3);
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
        _choicePanelUI.CanvasGroupChoice1.alpha = ChoicePanelUIValues.MinValue;
        _choicePanelUI.CanvasGroupChoice2.alpha = ChoicePanelUIValues.MinValue;
        _choicePanelUI.CanvasGroupChoice3.alpha = ChoicePanelUIValues.MinValue;
    }

    private ResourcesViewMode CalculateResourceViewMode(ChoiceData data)
    {
        return new ResourcesViewModeCalculator().CalculateResourcesViewMode((data.GetAllPrice, data.GetAllAdditionaryPrice));
    }

    private void ShowChoiceVariantsAfterWalletUpdate(ChoiceData data, ChoiceResultEvent<int> choiceResultEvent, bool keyButtonChoice3)
    {
        _choiceNodeButtonsHandler.CheckChoiceButtonsCanPress(data);

        SetCanvasGroupStartValue(_choicePanelUI.CanvasGroupChoice1, _choiceNodeButtonsHandler.Choice1ButtonCanPress);
        SetCanvasGroupStartValue(_choicePanelUI.CanvasGroupChoice2, _choiceNodeButtonsHandler.Choice2ButtonCanPress);

        if (data.ShowChoice3 == true)
        {
            SetCanvasGroupStartValue(_choicePanelUI.CanvasGroupChoice3,
                _choiceNodeButtonsHandler.Choice3ButtonCanPress);
        }

        if (_choiceActive.Value == true)
        {
            _choiceNodeButtonsHandler.TryActivateButtonsChoice(choiceResultEvent, keyButtonChoice3);
        }
    }

    private void OnUpdateWallet(ChoiceData data,
        ChoiceResultEvent<int> choiceResultEvent, bool keyButtonChoice3)
    {
        _compositeDisposableOnUpdateWallet = new CompositeDisposable();
        _wallet.MonetsCountChanged.Subscribe(_ =>
        {
            ShowChoiceVariantsAfterWalletUpdate(data, choiceResultEvent, keyButtonChoice3);
        }).AddTo(_compositeDisposableOnUpdateWallet);
        _wallet.HeartsCountChanged.Subscribe(_ =>
        {
            ShowChoiceVariantsAfterWalletUpdate(data, choiceResultEvent, keyButtonChoice3);
        }).AddTo(_compositeDisposableOnUpdateWallet);
    }
}