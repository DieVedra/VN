using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChoiceNodePriceHandler
{
    private const int _pricesCount = 3;
    private readonly Wallet _wallet;
    private readonly RectTransform _priceRectTransformChoice1;
    private readonly RectTransform _priceRectTransformChoice2;
    private readonly RectTransform _priceRectTransformChoice3;
    private readonly RectTransform _additionaryPriceRectTransformChoice1;
    private readonly RectTransform _additionaryPriceRectTransformChoice2;
    private readonly RectTransform _additionaryPriceRectTransformChoice3;

    private readonly TextMeshProUGUI _priceButton1Text;
    private readonly TextMeshProUGUI _priceButton2Text;
    private readonly TextMeshProUGUI _priceButton3Text;

    private readonly TextMeshProUGUI _additionaryPriceButton1Text;
    private readonly TextMeshProUGUI _additionaryPriceButton2Text;
    private readonly TextMeshProUGUI _additionaryPriceButton3Text;

    private List<(int, int)> _prices;
    public bool PriceExists { get; private set; }

    public ChoiceNodePriceHandler(
        RectTransform priceRectTransformChoice1, RectTransform priceRectTransformChoice2, RectTransform priceRectTransformChoice3,
        RectTransform additionaryPriceRectTransformChoice1, RectTransform additionaryPriceRectTransformChoice2, RectTransform additionaryPriceRectTransformChoice3,
        Wallet wallet,
        TextMeshProUGUI priceButton1Text, TextMeshProUGUI priceButton2Text, TextMeshProUGUI priceButton3Text,
        TextMeshProUGUI additionaryPriceButton1Text, TextMeshProUGUI additionaryPriceButton2Text, TextMeshProUGUI additionaryPriceButton3Text)
    {
        _wallet = wallet;
        _priceRectTransformChoice1 = priceRectTransformChoice1;
        _priceRectTransformChoice2 = priceRectTransformChoice2;
        _priceRectTransformChoice3 = priceRectTransformChoice3;
        _additionaryPriceRectTransformChoice1 = additionaryPriceRectTransformChoice1;
        _additionaryPriceRectTransformChoice2 = additionaryPriceRectTransformChoice2;
        _additionaryPriceRectTransformChoice3 = additionaryPriceRectTransformChoice3;
        _priceButton1Text = priceButton1Text;
        _priceButton2Text = priceButton2Text;
        _priceButton3Text = priceButton3Text;
        _additionaryPriceButton1Text = additionaryPriceButton1Text;
        _additionaryPriceButton2Text = additionaryPriceButton2Text;
        _additionaryPriceButton3Text = additionaryPriceButton3Text;
        _prices = new List<(int, int)>(_pricesCount);
    }

    public void TryShowPrices(ChoiceData data)
    {
        ResetPrices();
        _prices.Add((data.Choice1Price, data.Choice1AdditionaryPrice));
        TryShowPrice(_priceRectTransformChoice1, _priceButton1Text, data.Choice1Price);
        TryShowPrice(_additionaryPriceRectTransformChoice1, _additionaryPriceButton1Text, data.Choice1AdditionaryPrice);
        
        _prices.Add((data.Choice2Price, data.Choice2AdditionaryPrice));
        TryShowPrice(_priceRectTransformChoice2, _priceButton2Text, data.Choice2Price);
        TryShowPrice(_additionaryPriceRectTransformChoice2, _additionaryPriceButton2Text, data.Choice2AdditionaryPrice);
        
        if (data.ShowChoice3)
        {
            _prices.Add((data.Choice3Price, data.Choice3AdditionaryPrice));
            TryShowPrice(_priceRectTransformChoice3, _priceButton3Text, data.Choice3Price);
            TryShowPrice(_additionaryPriceRectTransformChoice3, _additionaryPriceButton3Text, data.Choice3AdditionaryPrice);
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
            PriceExists = true;
        }
    }
    private void ResetPrices()
    {
        PriceExists = false;
        _prices.Clear();
        _priceRectTransformChoice1.gameObject.SetActive(false);
        _priceRectTransformChoice2.gameObject.SetActive(false);
        _priceRectTransformChoice3.gameObject.SetActive(false);
        _additionaryPriceRectTransformChoice1.gameObject.SetActive(false);
        _additionaryPriceRectTransformChoice2.gameObject.SetActive(false);
        _additionaryPriceRectTransformChoice3.gameObject.SetActive(false);
    }
}