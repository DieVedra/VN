using UniRx;

public class CalculatePriceHandler
{
    private readonly ReactiveProperty<int> _monets;
    private readonly ReactiveProperty<int> _hearts;
    
    private readonly ReactiveProperty<int> _monetsToShow;
    private readonly ReactiveProperty<int> _heartsToShow;

    private SwitchInfo[] _lastSwitchInfo;
    public ReactiveProperty<int> MonetsToShowReactiveProperty => _monetsToShow;
    public ReactiveProperty<int> HeartsToShowReactiveProperty => _heartsToShow;
    public int MonetsToShow => _monetsToShow.Value;
    public int HeartsToShow => _heartsToShow.Value;
    
    public CalculatePriceHandler(ReactiveProperty<int> monets, ReactiveProperty<int> hearts)
    {
        _monets = monets;
        _hearts = hearts;
        
        _monetsToShow = new ReactiveProperty<int>(monets.Value);
        _heartsToShow = new ReactiveProperty<int>(hearts.Value);
        
        monets.Skip(1).Subscribe(_ =>
        {
            PreliminaryBalanceCalculation();
        });
        hearts.Skip(1).Subscribe(_ =>
        {
            PreliminaryBalanceCalculation();
        });
    }
    
    public void PreliminaryBalanceCalculation(params SwitchInfo[] switchInfo)
    {
        _lastSwitchInfo = switchInfo;
        PreliminaryBalanceCalculation();
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

    private void PreliminaryBalanceCalculation()
    {
        int monetsToShow = _monets.Value;
        int heartsToShow = _hearts.Value;
        if (_lastSwitchInfo != null)
        {
            for (int i = 0; i < _lastSwitchInfo.Length; i++)
            {
                monetsToShow -= _lastSwitchInfo[i].Price;
                heartsToShow -= _lastSwitchInfo[i].AdditionalPrice;
            }
        }
        _monetsToShow.Value = monetsToShow;
        _heartsToShow.Value = heartsToShow;
    }
}