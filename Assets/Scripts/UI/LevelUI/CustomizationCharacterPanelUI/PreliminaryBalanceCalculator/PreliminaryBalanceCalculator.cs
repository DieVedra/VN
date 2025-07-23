using UniRx;

public class PreliminaryBalanceCalculator
{
    private readonly ReactiveProperty<int> _monets;
    private readonly ReactiveProperty<int> _hearts;

    private readonly ReactiveProperty<int> _monetsToShow;
    private readonly ReactiveProperty<int> _heartsToShow;
    public ReactiveProperty<int> MonetsToShowReactiveProperty => _monetsToShow;
    public ReactiveProperty<int> HeartsToShowReactiveProperty => _heartsToShow;
    public int MonetsToShow => _monetsToShow.Value;
    public int HeartsToShow => _heartsToShow.Value;
    
    public PreliminaryBalanceCalculator(ReactiveProperty<int> monets, ReactiveProperty<int> hearts)
    {
        _monets = monets;
        _hearts = hearts;
        _monetsToShow = new ReactiveProperty<int>(monets.Value);
        _heartsToShow = new ReactiveProperty<int>(hearts.Value);
    }
    public bool CheckAvailableMoney(int price)
    {
        int moneyToShow = MonetsToShow;
        if (moneyToShow - price >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool CheckAvailableHearts(int additionalPrice)
    {
        int moneyToShow = HeartsToShow;
        if (moneyToShow - additionalPrice >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void PreliminaryBalanceCalculation(params (int price, int additionalPrice)[] prices)
    {
        int monetsToShow = _monets.Value;
        int heartsToShow = _hearts.Value;

        for (int i = 0; i < prices.Length; i++)
        {
            monetsToShow -= prices[i].price;
            heartsToShow -= prices[i].additionalPrice;
        }

        _monetsToShow.Value = monetsToShow;
        _heartsToShow.Value = heartsToShow;
    }
}