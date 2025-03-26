
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;

public class ChoiceNodePriceHandler
{
    private readonly Wallet _wallet;
    private readonly RectTransform _priceRectTransformChoice1;
    private readonly RectTransform _priceRectTransformChoice2;
    private readonly RectTransform _priceRectTransformChoice3;
    
    private readonly TextMeshProUGUI _priceButton1Text;
    private readonly TextMeshProUGUI _priceButton2Text;
    private readonly TextMeshProUGUI _priceButton3Text;
    private readonly TextMeshProUGUI _moneyPanelText;
    private readonly CanvasGroup _moneyPanelCanvasGroup;

    private List<int> _prices;
    public bool Choice1ButtonCanPress { get; private set; }
    public bool Choice2ButtonCanPress { get; private set; }
    public bool Choice3ButtonCanPress { get; private set; }
    private bool _showMyMoney;

    public ChoiceNodePriceHandler(RectTransform priceRectTransformChoice1, RectTransform priceRectTransformChoice2,
        RectTransform priceRectTransformChoice3, Wallet wallet,
        TextMeshProUGUI priceButton1Text, TextMeshProUGUI priceButton2Text, TextMeshProUGUI priceButton3Text, TextMeshProUGUI moneyPanelText, 
        CanvasGroup moneyPanelCanvasGroup)
    {
        _wallet = wallet;
        _priceRectTransformChoice1 = priceRectTransformChoice1;
        _priceRectTransformChoice2 = priceRectTransformChoice2;
        _priceRectTransformChoice3 = priceRectTransformChoice3;
        _priceButton1Text = priceButton1Text;
        _priceButton2Text = priceButton2Text;
        _priceButton3Text = priceButton3Text;
        _moneyPanelText = moneyPanelText;
        _moneyPanelCanvasGroup = moneyPanelCanvasGroup;
    }

    public void TryShowPrices(ChoiceData data)
    {
        ResetPrices();
        TryShowPrice(_priceRectTransformChoice1, _priceButton1Text, data.Choice1Price);
        Choice1ButtonCanPress = CheckChoiceButtonCanPress(data.Choice1Price);
        TryShowPrice(_priceRectTransformChoice2, _priceButton2Text, data.Choice2Price);
        Choice2ButtonCanPress = CheckChoiceButtonCanPress(data.Choice2Price);

        if (data.ShowChoice3)
        {
            TryShowPrice(_priceRectTransformChoice3, _priceButton3Text, data.Choice3Price);
            Choice3ButtonCanPress = CheckChoiceButtonCanPress(data.Choice3Price);
        }

        if (_showMyMoney)
        {
            _moneyPanelText.text = _wallet.Monets.ToString();
            _wallet.MonetsReactiveProperty.Subscribe(_ =>
            {
                _moneyPanelText.text = _wallet.Monets.ToString();
            });
            _moneyPanelText.transform.parent.gameObject.SetActive(true);
            _moneyPanelCanvasGroup.alpha = 1f;
        }
    }

    public void Debit(int buttonPressIndex)
    {
        _wallet.RemoveCash(_prices[buttonPressIndex]);
        _prices = null;
    }
    public void TryShowMoneyPanel(CancellationToken cancellationToken)
    {
        if (_showMyMoney)
        {
            _moneyPanelCanvasGroup.alpha = 0f;
            _moneyPanelCanvasGroup.DOFade(1f, 0.5f).WithCancellation(cancellationToken);
        }
    }
    public void TryHideMoneyPanel(CancellationToken cancellationToken)
    {
        if (_showMyMoney)
        {
            _moneyPanelCanvasGroup.DOFade(0f, 0.5f).WithCancellation(cancellationToken);
        }
    }
    private void TryShowPrice(RectTransform priceRectTransformChoice, TextMeshProUGUI priceText, int price)
    {
        _prices.Add(price);
        if (price > 0)
        {
            priceRectTransformChoice.gameObject.SetActive(true);
            priceText.text = price.ToString();
            _showMyMoney = true;
        }
    }

    private bool CheckChoiceButtonCanPress(int price)
    {
        bool result = false;
        if (price == 0)
        {
            result = true;
        }
        else if(price > 0)
        {
            result = _wallet.CashAvailable(price);
        }

        return result;
    }
    private void ResetPrices()
    {
        _prices = new List<int>(3);
        _moneyPanelText.transform.parent.gameObject.SetActive(false);
        Choice1ButtonCanPress = true;
        Choice2ButtonCanPress = true;
        Choice3ButtonCanPress = true;
        _priceRectTransformChoice1.gameObject.SetActive(false);
        _priceRectTransformChoice2.gameObject.SetActive(false);
        _priceRectTransformChoice3.gameObject.SetActive(false);
        _showMyMoney = false;
    }
}