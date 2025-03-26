
using UnityEngine;

public class CalculatePriceHandler
{
    private readonly int _money;
    public int MoneyToShow { get; private set; }
    
    public CalculatePriceHandler(int money)
    {
        _money = money;
        MoneyToShow = money;
    }
    
    public void PreliminaryBalanceCalculation(params SwitchInfo[] switchInfo)
    {
        MoneyToShow = _money;

        for (int i = 0; i < switchInfo.Length; i++)
        {
            MoneyToShow -= switchInfo[i].Price;
        }
    }
    public bool CheckAvailableMoney(int price)
    {
        int moneyToShow = MoneyToShow;
        if (moneyToShow - price >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}