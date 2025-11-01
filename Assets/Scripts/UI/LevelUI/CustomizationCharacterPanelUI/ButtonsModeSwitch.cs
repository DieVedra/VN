using TMPro;
using UniRx;

public class ButtonsModeSwitch
{
    private readonly ICharacterCustomizationView _characterCustomizationView;
    private readonly SelectedCustomizationContentIndexes _selectedCustomizationContentIndexes;
    private readonly ReactiveProperty<ArrowSwitchMode> _switchModeCustodian;
    private readonly TextMeshProUGUI _titleTextComponent;
    private readonly StatViewHandler _statViewHandler;
    private readonly PriceViewHandler _priceViewHandler;
    private readonly CustomizationSettingsCustodian _customizationSettingsCustodian;
    private readonly SwitchInfoCustodian _switchInfoCustodian;
    private readonly ButtonPlayHandler _buttonPlayHandler;
    private readonly CalculateStatsHandler _calculateStatsHandler;
    private readonly PreliminaryBalanceCalculator _preliminaryBalanceCalculator;
    private readonly CustomizationDataProvider _customizationDataProvider;
    private readonly ReactiveProperty<bool> _isSwimsuitsClothesReactiveProperty;
    private readonly ReactiveCommand<bool> _offArrows;
    private CompositeDisposable _compositeDisposable;
    private int CurrentSwitchIndex => _switchInfoCustodian.CurrentSwitchInfo.Index;
    
    public ButtonsModeSwitch(
        ICharacterCustomizationView characterCustomizationView, SelectedCustomizationContentIndexes selectedCustomizationContentIndexes,
        ReactiveProperty<ArrowSwitchMode> switchModeCustodian, TextMeshProUGUI titleTextComponent, StatViewHandler statViewHandler,
        PriceViewHandler priceViewHandler, CustomizationSettingsCustodian customizationSettingsCustodian, SwitchInfoCustodian switchInfoCustodian,
        ButtonPlayHandler buttonPlayHandler, 
        CalculateStatsHandler calculateStatsHandler, PreliminaryBalanceCalculator preliminaryBalanceCalculator,
        CustomizationDataProvider customizationDataProvider, ReactiveProperty<bool> isSwimsuitsClothesReactiveProperty,
        ReactiveCommand<bool> offArrows, SetLocalizationChangeEvent setLocalizationChangeEvent)
    {
        _characterCustomizationView = characterCustomizationView;
        _selectedCustomizationContentIndexes = selectedCustomizationContentIndexes;
        _switchModeCustodian = switchModeCustodian;
        _titleTextComponent = titleTextComponent;
        _statViewHandler = statViewHandler;
        _priceViewHandler = priceViewHandler;
        _customizationSettingsCustodian = customizationSettingsCustodian;
        _switchInfoCustodian = switchInfoCustodian;
        _buttonPlayHandler = buttonPlayHandler;
        _calculateStatsHandler = calculateStatsHandler;
        _preliminaryBalanceCalculator = preliminaryBalanceCalculator;
        _customizationDataProvider = customizationDataProvider;
        _isSwimsuitsClothesReactiveProperty = isSwimsuitsClothesReactiveProperty;
        _offArrows = offArrows;
        _compositeDisposable = setLocalizationChangeEvent.SubscribeWithCompositeDisposable(SetTitle);
    }

    public void Dispose()
    {
        _compositeDisposable?.Clear();
    }
    public void SetMode(ArrowSwitchMode mode)
    {
        switch (mode)
        {
            case ArrowSwitchMode.SkinColor:
                _switchInfoCustodian.SetToCurrentInfo(_switchInfoCustodian.BodySwitchInfo);
                TryOffArrowsAndSetCurrentCustomizationSettingses((int) mode);
                CalculatingStats(_switchInfoCustodian.HairstyleSwitchInfo, _switchInfoCustodian.ClothesSwitchInfo);
                break;
            case ArrowSwitchMode.Hairstyle:
                _switchInfoCustodian.SetToCurrentInfo(_switchInfoCustodian.HairstyleSwitchInfo);
                TryOffArrowsAndSetCurrentCustomizationSettingses((int) mode);
                CalculatingStats(_switchInfoCustodian.BodySwitchInfo, _switchInfoCustodian.ClothesSwitchInfo);
                break;
            case ArrowSwitchMode.Clothes:
                _switchInfoCustodian.SetToCurrentInfo(_switchInfoCustodian.ClothesSwitchInfo);
                CheckAndSetClothes();
                CalculatingStats(_switchInfoCustodian.BodySwitchInfo, _switchInfoCustodian.HairstyleSwitchInfo);
                break;
        }
        CalculatingPrice();
        _switchModeCustodian.Value = mode;
        SetTitle();
        if (_statViewHandler.CheckViewStatToShow(_customizationSettingsCustodian.CurrentCustomizationSettings[CurrentSwitchIndex]))
        {
            CustomizationStat stat = _statViewHandler.GetStatInfo(_customizationSettingsCustodian.CurrentCustomizationSettings[CurrentSwitchIndex].GameStats);
            _statViewHandler.Show(stat.ColorField, _statViewHandler.CreateLabel(stat));
        }
        else
        {
            _statViewHandler.Hide();
        }

        int price = _customizationSettingsCustodian.CurrentCustomizationSettings[CurrentSwitchIndex].Price;
        int priceAdditional = _customizationSettingsCustodian.CurrentCustomizationSettings[CurrentSwitchIndex].PriceAdditional;
        if (price == 0 && priceAdditional == 0)
        {
            _priceViewHandler.Hide();
        }
        else
        {
            _priceViewHandler.Show(price, priceAdditional);
        }
        if (_preliminaryBalanceCalculator.CheckAvailableMoneyAndHearts(_switchInfoCustodian.GetAllPriceInfo))
        {
            _buttonPlayHandler.On();
        }
        else
        {
            _buttonPlayHandler.Off();
        }

        _characterCustomizationView.SetCharacterCustomization(_customizationDataProvider.CreateCustomizationData(_switchInfoCustodian.CurrentSwitchIndex));
    }
    private void SetTitle()
    {
        _titleTextComponent.text = _customizationSettingsCustodian.CurrentCustomizationSettings[CurrentSwitchIndex].Name;
    }
    private void TryOffArrowsAndSetCurrentCustomizationSettingses(int modeValue)
    {
        var settings = _selectedCustomizationContentIndexes.IndexesSpriteIndexes[modeValue];
        if (settings.Count == 1)
        {
            _offArrows.Execute(true);
        }
        else
        {
            _offArrows.Execute(false);
        }
        _customizationSettingsCustodian.CurrentCustomizationSettings = settings;
    }
    private void CalculatingStats(params SwitchInfo[] switchInfo)
    {
        _switchInfoCustodian.SetStatsToCurrentSwitchInfo();
        _calculateStatsHandler.PreliminaryStatsCalculation(switchInfo);
    }
    private void CalculatingPrice()
    {
        _switchInfoCustodian.SetPriceToCurrentSwitchInfo();
        _switchInfoCustodian.SetAdditionalPriceToCurrentSwitchInfo();
    }
    private void CheckAndSetClothes()
    {
        int clothesIndex = (int) ArrowSwitchMode.Clothes;
        if (_selectedCustomizationContentIndexes.IndexesSpriteIndexes[clothesIndex].Count == 0)
        {
            TryOffArrowsAndSetCurrentCustomizationSettingses((int) ArrowSwitchMode.Swimsuits);
            _isSwimsuitsClothesReactiveProperty.Value = true;
        }
        else
        {
            TryOffArrowsAndSetCurrentCustomizationSettingses(clothesIndex);
            _isSwimsuitsClothesReactiveProperty.Value = false;
        }
    }
}