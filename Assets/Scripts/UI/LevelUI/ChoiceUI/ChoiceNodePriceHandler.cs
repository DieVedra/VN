using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChoiceNodePriceHandler
{
    private readonly Wallet _wallet;
    private readonly IReadOnlyList<ChoiceCaseView> _choiseCasesViews;
    private List<(int, int)> _prices;

    public ChoiceNodePriceHandler(IReadOnlyList<ChoiceCaseView> choiseCasesViews, Wallet wallet)
    {
        _choiseCasesViews = choiseCasesViews;
        _wallet = wallet;
        _prices = new List<(int, int)>(ChoiceNode.MaxCaseCount);
    }

    public void TryShowPrices(ChoiceData data)
    {
        ResetPrices();
        ChoiceCase choiceCase;
        ChoiceCaseView choiceCaseView;
        for (int i = 0; i < data.ChoiceCases.Count; i++)
        {
            choiceCase = data.ChoiceCases[i];
            choiceCaseView = _choiseCasesViews[i];
            _prices.Add((choiceCase.ChoicePrice, choiceCase.ChoiceAdditionaryPrice));
            TryShowPrice(choiceCaseView.PriceRectTransformChoice, choiceCaseView.PriceButtonText, choiceCase.ChoicePrice);
            TryShowPrice(choiceCaseView.AdditionaryPriceRectTransformChoice, choiceCaseView.AdditionaryPriceButtonText, choiceCase.ChoiceAdditionaryPrice);
        }
    }

    public void Debit(int buttonPressIndex)
    {
        _wallet.RemoveCash(_prices[buttonPressIndex].Item1);
        _wallet.RemoveHearts(_prices[buttonPressIndex].Item2);
    }

    private void TryShowPrice(RectTransform priceRectTransformChoice, TextMeshProUGUI priceText, int price)
    {
        if (price > 0)
        {
            priceRectTransformChoice.gameObject.SetActive(true);
            priceText.text = price.ToString();
        }
    }
    private void ResetPrices()
    {
        _prices.Clear();
        foreach (var t in _choiseCasesViews)
        {
            t.PriceRectTransformChoice.gameObject.SetActive(false);
            t.AdditionaryPriceRectTransformChoice.gameObject.SetActive(false);
        }
    }
}