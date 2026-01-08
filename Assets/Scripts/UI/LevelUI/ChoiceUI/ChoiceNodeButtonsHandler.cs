using System;
using System.Collections.Generic;
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
    private readonly IReadOnlyList<ChoiceCaseView> _choiseCasesViews;
    private readonly bool[] _choiseButtonsCanPress;
    private readonly Vector2[] _choicePositions;
    private ChoiceData _data;
    private Vector2 _offset = new Vector2(ChoicePanelUIValues.OffsetX, ChoicePanelUIValues.OffsetY);
    public IReadOnlyList<bool> ChoiseButtonsCanPress => _choiseButtonsCanPress;

    public ChoiceNodeButtonsHandler(IReadOnlyList<ChoiceCaseView> choiseCasesViews, ChoiceNodePriceHandler choiceNodePriceHandler, Wallet wallet, ChoicePanelUI choicePanelUI,
        ReactiveProperty<bool> choiceActive)
    {
        _choiceNodePriceHandler = choiceNodePriceHandler;
        _wallet = wallet;
        _duration = choicePanelUI.DurationAnim;
        _choiseCasesViews = choiseCasesViews;
        _choiceActive = choiceActive;
        _choiseButtonsCanPress = new[] {true, true, true, true};
        _choicePositions = new[] {new Vector2(), new Vector2(), new Vector2(), new Vector2()};
    }

    public async UniTask ShowButtons(ChoiceData data, CancellationToken cancellationToken)
    {
        _data = data;
        for (int i = 0; i < data.ButtonsCount; i++)
        {
            _choicePositions[i] = _choiseCasesViews[i].RectTransformChoice.anchoredPosition;
            ShowChoiceVariant(cancellationToken, _choiseCasesViews[i].ButtonChoice, _choiseCasesViews[i].RectTransformChoice,
                _choiseCasesViews[i].CanvasGroupChoice, _choicePositions[i] - _offset, _choiseButtonsCanPress[i]).Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(_duration * ChoicePanelUIValues.HalfValue), cancellationToken: cancellationToken);
        }
    }

    public async UniTask HideButtons(CancellationToken cancellationToken)
    {
        ChoiceCaseView choiceCaseView;
        for (int i = _data.ChoiceCases.Count - 1; i >= 0; i--)
        {
            choiceCaseView = _choiseCasesViews[i];
            DisappearanceChoiceVariant(cancellationToken, choiceCaseView.ButtonChoice, choiceCaseView.RectTransformChoice, choiceCaseView.CanvasGroupChoice, _choicePositions[i]).Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(_duration * ChoicePanelUIValues.HalfValue), cancellationToken: cancellationToken);
        }
    }

    public void TryActivateButtonsChoice(ChoiceData data, ChoiceResultEvent<ChoiceCase> choiceResultEvent)
    {
        for (int i = 0; i < data.ChoiceCases.Count; i++)
        {
            ActivateButtonChoice(_choiseCasesViews[i].ButtonChoice, choiceResultEvent, data.ChoiceCases[i], i, _choiseButtonsCanPress[i]);
        }
    }

    private void ActivateButtonChoice(Button buttonChoice, ChoiceResultEvent<ChoiceCase> choiceResultEvent, ChoiceCase choiceCaseResult, int caseIndex, bool buttonCanPress)
    {
        if (buttonCanPress)
        {
            buttonChoice.onClick.AddListener(()=>
            {
                DeactivateButtonsChoice();
                _choiceActive.Value = false;
                _choiceNodePriceHandler.Debit(caseIndex);
                choiceResultEvent.Execute(choiceCaseResult);
            });
        }
    }

    public void DeactivateButtonsChoice()
    {
        foreach (var view in _choiseCasesViews)
        {
            view.ButtonChoice.onClick.RemoveAllListeners();
        }
    }
    public void Reset()
    {
        for (int i = 0; i < _choiseButtonsCanPress.Length; i++)
        {
            _choiseButtonsCanPress[i] = true;
        }
    }
    public void CheckChoiceButtonsCanPress(ChoiceData data)
    {
        for (int i = 0; i < data.ChoiceCases.Count; i++)
        {
            _choiseButtonsCanPress[i] = CheckChoiceButtonCanPress(data.ChoiceCases[i].ChoicePrice, data.ChoiceCases[i].ChoiceAdditionaryPrice);
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