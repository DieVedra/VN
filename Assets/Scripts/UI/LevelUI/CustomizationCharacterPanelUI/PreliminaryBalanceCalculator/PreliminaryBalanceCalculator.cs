using UniRx;

public class PreliminaryBalanceCalculator
{
    private readonly ReactiveProperty<int> _monets;
    private readonly ReactiveProperty<int> _hearts;
    public PreliminaryBalanceCalculator(ReactiveProperty<int> monets, ReactiveProperty<int> hearts)
    {
        _monets = monets;
        _hearts = hearts;
    }

    private bool CheckAvailableMoney(int price)
    {
        int moneyToShow = _monets.Value;
        if (moneyToShow - price >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CheckAvailableHearts(int additionalPrice)
    {
        int moneyToShow = _hearts.Value;
        if (moneyToShow - additionalPrice >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public (int price, int additionalPrice) PricesCalculationToRemove(params (int price, int additionalPrice)[] prices)
    {
        int price = 0;
        int additionalPrice = 0;
        for (int i = 0; i < prices.Length; i++)
        {
            price += prices[i].price;
            additionalPrice += prices[i].additionalPrice;
        }
        return (price, additionalPrice);
    }
    public bool CheckAvailableMoneyAndHearts(params (int price, int additionalPrice)[] prices)
    {
        var pricesToRemove = PricesCalculationToRemove(prices);
        
        if (CheckAvailableMoney(pricesToRemove.price) == true &&
            CheckAvailableHearts(pricesToRemove.additionalPrice) == true)
        {
            return true;
        }
        else return false;
    }
}