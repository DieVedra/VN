
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ChoicePanelUIHandler
{
    private const int _button1PressIndex = 0;
    private const int _button2PressIndex = 1;
    private const int _button3PressIndex = 2;
    private const float _priceAvailableFadeValue = 1f;
    private const float _priceNotAvailableFadeValue = 0.5f;
    private const float _halfValue = 0.5f;
    private readonly Vector2 _offset = new Vector2(0f, 45f);
    private Vector2 _choice1Position;
    private Vector2 _choice2Position;
    private Vector2 _choice3Position;
    
    private readonly ChoicePanelUI _choicePanelUI;
    private readonly ChoiceHeightHandler _choiceHeightHandler;
    private readonly ChoiceNodeTimer _choiceNodeTimer;
    private readonly ChoiceNodePriceHandler _choiceNodePriceHandler;

    private float _duration => _choicePanelUI.DurationAnim;
    
    public ChoicePanelUIHandler(ChoicePanelUI choicePanelUI, Wallet wallet)
    {
        _choicePanelUI = choicePanelUI;
        _choiceNodeTimer = new ChoiceNodeTimer(choicePanelUI.TimerPanelText, choicePanelUI. TimerPanelCanvasGroup, choicePanelUI.TimerImageRectTransform);
        _choiceNodePriceHandler = new ChoiceNodePriceHandler( 
            choicePanelUI.PriceRectTransformChoice1, choicePanelUI.PriceRectTransformChoice2, choicePanelUI.PriceRectTransformChoice3, wallet,
            choicePanelUI.PriceButton1Text, choicePanelUI.PriceButton2Text, choicePanelUI.PriceButton3Text, choicePanelUI.MoneyPanelText, _choicePanelUI.MoneyPanelCanvasGroup);
        _choiceHeightHandler = new ChoiceHeightHandler(
            choicePanelUI.RectTransformChoice1, choicePanelUI.RectTransformChoice2, 
            choicePanelUI.RectTransformChoice3,
            choicePanelUI.TextButtonChoice1, choicePanelUI.TextButtonChoice2, choicePanelUI.TextButtonChoice3);
    }

    public void ShowChoiceVariants(ChoiceData data)
    {
        SetTexts(data);
        _choicePanelUI.gameObject.SetActive(true);
        _choiceNodePriceHandler.TryShowPrices(data);
        _choiceHeightHandler.UpdateHeight(data);
        _choiceNodeTimer.TrySetTimerValue(data.TimerValue);

        SetCanvasGroupStartValue(_choicePanelUI.CanvasGroupChoice1, _choiceNodePriceHandler.Choice1ButtonCanPress);
        SetCanvasGroupStartValue(_choicePanelUI.CanvasGroupChoice2, _choiceNodePriceHandler.Choice2ButtonCanPress);

        if (data.ShowChoice3 == true)
        {
            SetCanvasGroupStartValue(_choicePanelUI.CanvasGroupChoice3, _choiceNodePriceHandler.Choice3ButtonCanPress);
        }
    }

    public void HideChoiceVariants()
    {
        ResetTexts();
        _choicePanelUI.gameObject.SetActive(false);
        SetZeroAlphaToCanvasGroups();
    }

    public async UniTask ShowChoiceVariantsInPlayMode(CancellationToken cancellationToken, ChoiceData data)
    {
        SetTexts(data);

        _choicePanelUI.gameObject.SetActive(true);

        _choiceNodePriceHandler.TryShowPrices(data);

        _choiceHeightHandler.UpdateHeight(data);

        _choiceNodeTimer.TrySetTimerValue(data.TimerValue);

        _choiceNodeTimer.TryShowTimerPanelAnim(cancellationToken).Forget();

        _choiceNodePriceHandler.TryShowMoneyPanel(cancellationToken);

        SetZeroAlphaToCanvasGroups();

        _choice1Position = _choicePanelUI.RectTransformChoice1.anchoredPosition;

        ShowChoiceVariant(cancellationToken, _choicePanelUI.ButtonChoice1, _choicePanelUI.RectTransformChoice1,
            _choicePanelUI.CanvasGroupChoice1, _choice1Position - _offset, _choiceNodePriceHandler.Choice1ButtonCanPress).Forget();

        await UniTask.Delay(TimeSpan.FromSeconds(_duration * _halfValue), cancellationToken: cancellationToken);

        _choice2Position = _choicePanelUI.RectTransformChoice2.anchoredPosition;

        ShowChoiceVariant(cancellationToken, _choicePanelUI.ButtonChoice2, _choicePanelUI.RectTransformChoice2,
            _choicePanelUI.CanvasGroupChoice2, _choice2Position - _offset, _choiceNodePriceHandler.Choice2ButtonCanPress).Forget();

        if (data.ShowChoice3 == true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_duration * _halfValue), cancellationToken: cancellationToken);
            _choice3Position = _choicePanelUI.RectTransformChoice3.anchoredPosition;
            ShowChoiceVariant(cancellationToken, _choicePanelUI.ButtonChoice3, _choicePanelUI.RectTransformChoice3,
                _choicePanelUI.CanvasGroupChoice3, _choice3Position - _offset, _choiceNodePriceHandler.Choice3ButtonCanPress).Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(_duration), cancellationToken: cancellationToken);
        }
        else
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_duration), cancellationToken: cancellationToken);
        }
    }

    public async UniTask DisappearanceChoiceVariantsInPlayMode(CancellationToken cancellationToken, bool keyShowChoice3)
    {
        _choiceNodeTimer.TryHideTimerPanelAnim(cancellationToken);
        _choiceNodePriceHandler.TryHideMoneyPanel(cancellationToken);
        
        if (keyShowChoice3 == true)
        {
            DisappearanceChoiceVariant(cancellationToken, _choicePanelUI.ButtonChoice3, _choicePanelUI.RectTransformChoice3, _choicePanelUI.CanvasGroupChoice3, _choice3Position).Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(_duration * _halfValue), cancellationToken: cancellationToken);
        }
        DisappearanceChoiceVariant(cancellationToken, _choicePanelUI.ButtonChoice2, _choicePanelUI.RectTransformChoice2, _choicePanelUI.CanvasGroupChoice2,_choice2Position).Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(_duration * _halfValue), cancellationToken: cancellationToken);

        DisappearanceChoiceVariant(cancellationToken, _choicePanelUI.ButtonChoice1, _choicePanelUI.RectTransformChoice1, _choicePanelUI.CanvasGroupChoice1,_choice1Position).Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(_duration), cancellationToken: cancellationToken);
        _choicePanelUI.gameObject.SetActive(false);
    }

    public void ActivateTimerChoice(ChoiceResultEvent<int> choiceResultEvent, int index, CancellationToken cancellationToken)
    {
        _choiceNodeTimer.TryStartTimer(choiceResultEvent,
            () =>
            {
                DeactivateButtonsChoice();
                _choiceNodePriceHandler.Debit(index);
            }, index, cancellationToken).Forget();
    }

    public void ActivateButtonsChoice(ChoiceResultEvent<int> choiceResultEvent, bool keyButtonChoice3)
    {
        ActivateButtonChoice(_choicePanelUI.ButtonChoice1, choiceResultEvent, _button1PressIndex, _choiceNodePriceHandler.Choice1ButtonCanPress);

        ActivateButtonChoice(_choicePanelUI.ButtonChoice2, choiceResultEvent, _button2PressIndex, _choiceNodePriceHandler.Choice2ButtonCanPress);
        
        if (keyButtonChoice3 == true)
        {
            ActivateButtonChoice(_choicePanelUI.ButtonChoice3, choiceResultEvent, _button3PressIndex, _choiceNodePriceHandler.Choice3ButtonCanPress);
        }
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

    private void ActivateButtonChoice(Button buttonChoice, ChoiceResultEvent<int> choiceResultEvent, int buttonPressIndex, bool buttonCanPress)
    {
        if (buttonCanPress)
        {
            buttonChoice.onClick.AddListener(()=>
            {
                DeactivateButtonsChoice();
                _choiceNodePriceHandler.Debit(buttonPressIndex);
                choiceResultEvent.Execute(buttonPressIndex);
            });
        }
    }
    private void DeactivateButtonsChoice()
    {
        _choicePanelUI.ButtonChoice1.onClick.RemoveAllListeners();
        _choicePanelUI.ButtonChoice2.onClick.RemoveAllListeners();
        _choicePanelUI.ButtonChoice3.onClick.RemoveAllListeners();
    }
    private async UniTask ShowChoiceVariant(CancellationToken cancellationToken, Button button, RectTransform rectTransform,
        CanvasGroup canvasGroup, Vector2 endPos, bool buttonCanPress)
    {
        canvasGroup.blocksRaycasts = buttonCanPress;
        SetActiveButton(button, true);
        await AnimationPanel(cancellationToken, rectTransform, canvasGroup, endPos,
            buttonCanPress == true ? _priceAvailableFadeValue : _priceNotAvailableFadeValue);
    }
    
    private async UniTask DisappearanceChoiceVariant(CancellationToken cancellationToken, Button button, RectTransform rectTransform, CanvasGroup canvasGroup, Vector3 endPos)
    {
        await AnimationPanel(cancellationToken, rectTransform, canvasGroup, endPos, 0f);
        SetActiveButton(button, false);
    }
    
    private void SetTextButton(Button button, TextMeshProUGUI textComponent, string text = " ", bool key = false)
    {
        SetActiveButton(button, key);
        SetText(textComponent, text);
    }

    private void SetText(TextMeshProUGUI textComponent, string text )
    {
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

    private async UniTask AnimationPanel(CancellationToken cancellationToken, RectTransform panelTransform, CanvasGroup canvasGroup, Vector3 endPos, float fadeEndValue)
    {
        await UniTask.WhenAll(
            panelTransform.DOAnchorPos(endPos, _duration).WithCancellation(cancellationToken),
            canvasGroup.DOFade(fadeEndValue, _duration).WithCancellation(cancellationToken));
    }

    private void SetCanvasGroupStartValue(CanvasGroup canvasGroup, bool choiceButtonCanPress)
    {
        if (choiceButtonCanPress)
        {
            canvasGroup.alpha = _priceAvailableFadeValue;
            canvasGroup.blocksRaycasts = choiceButtonCanPress;
        }
        else
        {
            canvasGroup.alpha = _priceNotAvailableFadeValue;
            canvasGroup.blocksRaycasts = choiceButtonCanPress;
        }
    }

    private void SetZeroAlphaToCanvasGroups()
    {
        _choicePanelUI.CanvasGroupChoice1.alpha = 0f;
        _choicePanelUI.CanvasGroupChoice2.alpha = 0f;
        _choicePanelUI.CanvasGroupChoice3.alpha = 0f;
    }
}