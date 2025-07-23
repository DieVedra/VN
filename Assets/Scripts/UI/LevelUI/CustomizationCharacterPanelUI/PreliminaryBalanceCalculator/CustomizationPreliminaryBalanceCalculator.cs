using UniRx;

public class CustomizationPreliminaryBalanceCalculator : PreliminaryBalanceCalculator
{
    private readonly SwitchInfoCustodian _switchInfoCustodian;

    public CustomizationPreliminaryBalanceCalculator(ReactiveProperty<int> monets, ReactiveProperty<int> hearts, SwitchInfoCustodian switchInfoCustodian)
        : base(monets, hearts)
    {
        _switchInfoCustodian = switchInfoCustodian;
        monets.Skip(1).Subscribe(_ =>
        {
            CustomizationPreliminaryBalanceCalculation();
        });
        hearts.Skip(1).Subscribe(_ =>
        {
            CustomizationPreliminaryBalanceCalculation();
        });
    }
    public void CustomizationPreliminaryBalanceCalculation()
    {
        PreliminaryBalanceCalculation(_switchInfoCustodian.GetAllPriceInfo);
    }
}