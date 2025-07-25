using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceNodeButtonsHandler
{
    private readonly float _duration;
    private readonly ChoiceNodePriceHandler _choiceNodePriceHandler;
    private readonly ReactiveProperty<bool> _choiceActive;
    private readonly Wallet _wallet;
    private readonly Button _buttonChoice1;
    private readonly Button _buttonChoice2;
    private readonly Button _buttonChoice3;
    private readonly RectTransform _rectTransformChoice1;
    private readonly RectTransform _rectTransformChoice2;
    private readonly RectTransform _rectTransformChoice3;
    private readonly CanvasGroup _canvasGroupChoice1;
    private readonly CanvasGroup _canvasGroupChoice2;
    private readonly CanvasGroup _canvasGroupChoice3;
    private Vector2 _offset = new Vector2(ChoicePanelUIValues.OffsetX, ChoicePanelUIValues.OffsetY);
    private Vector2 _choice1Position;
    private Vector2 _choice2Position;
    private Vector2 _choice3Position;
    public bool Choice1ButtonCanPress { get; private set; }
    public bool Choice2ButtonCanPress { get; private set; }
    public bool Choice3ButtonCanPress { get; private set; }

    public ChoiceNodeButtonsHandler(ChoiceNodePriceHandler choiceNodePriceHandler, Wallet wallet, ChoicePanelUI choicePanelUI, ReactiveProperty<bool> choiceActive)
    {
        _choiceNodePriceHandler = choiceNodePriceHandler;
        _wallet = wallet;
        _duration = choicePanelUI.DurationAnim;
        _buttonChoice1 = choicePanelUI.ButtonChoice1;
        _buttonChoice2 = choicePanelUI.ButtonChoice2;
        _buttonChoice3 = choicePanelUI.ButtonChoice3;
        _rectTransformChoice1 = choicePanelUI.RectTransformChoice1;
        _rectTransformChoice2 = choicePanelUI.RectTransformChoice2;
        _rectTransformChoice3 = choicePanelUI.RectTransformChoice3;
        _canvasGroupChoice1 = choicePanelUI.CanvasGroupChoice1;
        _canvasGroupChoice2 = choicePanelUI.CanvasGroupChoice2;
        _canvasGroupChoice3 = choicePanelUI.CanvasGroupChoice3;
        _choiceActive = choiceActive;
    }

    public async UniTask ShowButtons(ChoiceData data, CancellationToken cancellationToken)
    {
        _choice1Position = _rectTransformChoice1.anchoredPosition;
         
         ShowChoiceVariant(cancellationToken, _buttonChoice1, _rectTransformChoice1,
            _canvasGroupChoice1, _choice1Position - _offset, Choice1ButtonCanPress).Forget();

        await UniTask.Delay(TimeSpan.FromSeconds(_duration * ChoicePanelUIValues.HalfValue), cancellationToken: cancellationToken);

        _choice2Position = _rectTransformChoice2.anchoredPosition;

        ShowChoiceVariant(cancellationToken, _buttonChoice2, _rectTransformChoice2,
            _canvasGroupChoice2, _choice2Position - _offset, Choice2ButtonCanPress).Forget();

        if (data.ShowChoice3 == true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_duration * ChoicePanelUIValues.HalfValue), cancellationToken: cancellationToken);
            _choice3Position = _rectTransformChoice3.anchoredPosition;

            ShowChoiceVariant(cancellationToken, _buttonChoice3, _rectTransformChoice3,
                _canvasGroupChoice3, _choice3Position - _offset, Choice3ButtonCanPress).Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(_duration), cancellationToken: cancellationToken);
        }
        else
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_duration), cancellationToken: cancellationToken);
        }
    }

    public async UniTask HideButtons(CancellationToken cancellationToken, bool keyShowChoice3)
    {
        if (keyShowChoice3 == true)
        {
            DisappearanceChoiceVariant(cancellationToken, _buttonChoice3, _rectTransformChoice3, _canvasGroupChoice3, _choice3Position).Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(_duration * ChoicePanelUIValues.HalfValue), cancellationToken: cancellationToken);
        }
        DisappearanceChoiceVariant(cancellationToken, _buttonChoice2, _rectTransformChoice2, _canvasGroupChoice2,_choice2Position).Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(_duration * ChoicePanelUIValues.HalfValue), cancellationToken: cancellationToken);

        DisappearanceChoiceVariant(cancellationToken, _buttonChoice1, _rectTransformChoice1, _canvasGroupChoice1,_choice1Position).Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(_duration), cancellationToken: cancellationToken);
    }
    public void TryActivateButtonsChoice(ChoiceResultEvent<int> choiceResultEvent, bool keyButtonChoice3)
    {
        ActivateButtonChoice(_buttonChoice1, choiceResultEvent, ChoicePanelUIValues.Button1PressIndex, Choice1ButtonCanPress);

        ActivateButtonChoice(_buttonChoice2, choiceResultEvent, ChoicePanelUIValues.Button2PressIndex, Choice2ButtonCanPress);
        
        if (keyButtonChoice3 == true)
        {
            ActivateButtonChoice(_buttonChoice3, choiceResultEvent, ChoicePanelUIValues.Button3PressIndex, Choice3ButtonCanPress);
        }
    }
    private void ActivateButtonChoice(Button buttonChoice, ChoiceResultEvent<int> choiceResultEvent, int buttonPressIndex, bool buttonCanPress)
    {
        if (buttonCanPress)
        {
            buttonChoice.onClick.AddListener(()=>
            {
                DeactivateButtonsChoice();
                _choiceActive.Value = false;
                _choiceNodePriceHandler.Debit(buttonPressIndex);
                choiceResultEvent.Execute(buttonPressIndex);
            });
        }
    }
    public void DeactivateButtonsChoice()
    {
        _buttonChoice1.onClick.RemoveAllListeners();
        _buttonChoice2.onClick.RemoveAllListeners();
        _buttonChoice3.onClick.RemoveAllListeners();
    }
    public void Reset()
    {
        Choice1ButtonCanPress = true;
        Choice2ButtonCanPress = true;
        Choice3ButtonCanPress = true;
    }
    public void CheckChoiceButtonsCanPress(ChoiceData data)
    {
        Choice1ButtonCanPress = CheckChoiceButtonCanPress(data.Choice1Price, data.Choice1AdditionaryPrice);
        Choice2ButtonCanPress = CheckChoiceButtonCanPress(data.Choice2Price, data.Choice2AdditionaryPrice);
        if (data.ShowChoice3)
        {
            Choice3ButtonCanPress = CheckChoiceButtonCanPress(data.Choice3Price, data.Choice3AdditionaryPrice);
        }
    }
    
    private bool CheckChoiceButtonCanPress(int price, int additionaryPrice)
    {
        bool result1 = _wallet.CashAvailable(price);
        bool result2 = _wallet.HeartsAvailable(additionaryPrice);
        if (result1 == true && result2 == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private async UniTask ShowChoiceVariant(CancellationToken cancellationToken, Button button, RectTransform rectTransform,
        CanvasGroup canvasGroup, Vector2 endPos, bool buttonCanPress)
    {
        canvasGroup.blocksRaycasts = buttonCanPress;
        button.gameObject.SetActive(true);

        await AnimationPanel(cancellationToken, rectTransform, canvasGroup, endPos,
            buttonCanPress == true ? ChoicePanelUIValues.PriceAvailableFadeValue : ChoicePanelUIValues.PriceNotAvailableFadeValue);
    }
    private async UniTask AnimationPanel(CancellationToken cancellationToken, RectTransform panelTransform, CanvasGroup canvasGroup, Vector3 endPos, float fadeEndValue)
    {
        await UniTask.WhenAll(
            panelTransform.DOAnchorPos(endPos, _duration).WithCancellation(cancellationToken),
            canvasGroup.DOFade(fadeEndValue, _duration).WithCancellation(cancellationToken));
    }
    private async UniTask DisappearanceChoiceVariant(CancellationToken cancellationToken, Button button, RectTransform rectTransform, CanvasGroup canvasGroup, Vector3 endPos)
    {
        await AnimationPanel(cancellationToken, rectTransform, canvasGroup, endPos, ChoicePanelUIValues.MinValue);
        button.gameObject.SetActive(false);
    }
}