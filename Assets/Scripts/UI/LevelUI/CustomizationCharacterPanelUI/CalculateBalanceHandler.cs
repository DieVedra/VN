using UniRx;

public class CalculateBalanceHandler
{
    private readonly ReactiveProperty<int> _monets;
    private readonly ReactiveProperty<int> _hearts;
    private readonly SwitchInfoCustodian _switchInfoCustodian;

    private readonly ReactiveProperty<int> _monetsToShow;
    private readonly ReactiveProperty<int> _heartsToShow;
    public ReactiveProperty<int> MonetsToShowReactiveProperty => _monetsToShow;
    public ReactiveProperty<int> HeartsToShowReactiveProperty => _heartsToShow;
    public int MonetsToShow => _monetsToShow.Value;
    public int HeartsToShow => _heartsToShow.Value;
    
    public CalculateBalanceHandler(ReactiveProperty<int> monets, ReactiveProperty<int> hearts, SwitchInfoCustodian switchInfoCustodian)
    {
        _monets = monets;
        _hearts = hearts;
        _switchInfoCustodian = switchInfoCustodian;

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

    public void PreliminaryBalanceCalculation()
    {
        int monetsToShow = _monets.Value;
        int heartsToShow = _hearts.Value;

        for (int i = 0; i < _switchInfoCustodian.GetAllInfo.Count; i++)
        {
            monetsToShow -= _switchInfoCustodian.GetAllInfo[i].Price;
            heartsToShow -= _switchInfoCustodian.GetAllInfo[i].AdditionalPrice;
        }

        _monetsToShow.Value = monetsToShow;
        _heartsToShow.Value = heartsToShow;
    }
}