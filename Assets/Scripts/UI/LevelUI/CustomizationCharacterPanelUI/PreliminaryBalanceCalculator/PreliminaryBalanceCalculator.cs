
public class PreliminaryBalanceCalculator
{
    private readonly Wallet _wallet;
    public PreliminaryBalanceCalculator(Wallet wallet)
    {
        _wallet = wallet;
    }

    private bool CheckAvailableMoney(int price)
    {
        return _wallet.CashAvailable(price);
    }

    private bool CheckAvailableHearts(int additionalPrice)
    {
        return _wallet.HeartsAvailable(additionalPrice);
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