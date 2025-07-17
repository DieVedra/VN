using TMPro;
using UniRx;
using UnityEngine;

public class ButtonsModeSwitch
{
    private readonly ICharacterCustomizationView _characterCustomizationView;
    private readonly SelectedCustomizationContentIndexes _selectedCustomizationContentIndexes;
    private readonly SwitchModeCustodian _switchModeCustodian;
    private readonly TextMeshProUGUI _titleTextComponent;
    private readonly StatViewHandler _statViewHandler;
    private readonly PriceViewHandler _priceViewHandler;
    private readonly CustomizationSettingsCustodian _customizationSettingsCustodian;
    private readonly SwitchInfoCustodian _switchInfoCustodian;
    private readonly ButtonPlayHandler _buttonPlayHandler;
    private readonly CalculateStatsHandler _calculateStatsHandler;
    private readonly CustomizationDataProvider _customizationDataProvider;
    private readonly ReactiveProperty<bool> _isNuClothesReactiveProperty;
    private readonly SetLocalizationChangeEvent _setLocalizationChangeEvent;
    private CompositeDisposable _compositeDisposable;
    private int CurrentSwitchIndex => _switchInfoCustodian.CurrentSwitchInfo.Index;


    public ButtonsModeSwitch(ICharacterCustomizationView characterCustomizationView, SelectedCustomizationContentIndexes selectedCustomizationContentIndexes,
        SwitchModeCustodian switchModeCustodian, TextMeshProUGUI titleTextComponent, StatViewHandler statViewHandler, PriceViewHandler priceViewHandler,
        CustomizationSettingsCustodian customizationSettingsCustodian, SwitchInfoCustodian switchInfoCustodian, ButtonPlayHandler buttonPlayHandler, CalculateStatsHandler calculateStatsHandler,
        CustomizationDataProvider customizationDataProvider, ReactiveProperty<bool> isNuClothesReactiveProperty, SetLocalizationChangeEvent setLocalizationChangeEvent)
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
        _customizationDataProvider = customizationDataProvider;
        _isNuClothesReactiveProperty = isNuClothesReactiveProperty;
        _setLocalizationChangeEvent = setLocalizationChangeEvent;
        _compositeDisposable = _setLocalizationChangeEvent.SubscribeWithCompositeDisposable(SetTitle);
    }

    public void Dispose()
    {
        _compositeDisposable.Dispose();
    }
    public void SetMode(ArrowSwitchMode mode)
    {
        switch (mode)
        {
            case ArrowSwitchMode.SkinColor:
                _switchInfoCustodian.SetToCurrentInfo(_switchInfoCustodian.BodySwitchInfo);
                SetCurrentCustomizationSettingses((int) mode);
                CalculatingStats(_switchInfoCustodian.HairstyleSwitchInfo, _switchInfoCustodian.ClothesSwitchInfo);
                CalculatingPrice(_switchInfoCustodian.HairstyleSwitchInfo, _switchInfoCustodian.ClothesSwitchInfo);
                break;
            case ArrowSwitchMode.Hairstyle:
                _switchInfoCustodian.SetToCurrentInfo(_switchInfoCustodian.HairstyleSwitchInfo);
                SetCurrentCustomizationSettingses((int) mode);
                CalculatingStats(_switchInfoCustodian.BodySwitchInfo, _switchInfoCustodian.ClothesSwitchInfo);
                CalculatingPrice(_switchInfoCustodian.BodySwitchInfo, _switchInfoCustodian.ClothesSwitchInfo);

                break;
            case ArrowSwitchMode.Clothes:
                _switchInfoCustodian.SetToCurrentInfo(_switchInfoCustodian.ClothesSwitchInfo);
                CheckAndSetClothes();
                CalculatingStats(_switchInfoCustodian.BodySwitchInfo, _switchInfoCustodian.HairstyleSwitchInfo);
                CalculatingPrice(_switchInfoCustodian.BodySwitchInfo, _switchInfoCustodian.HairstyleSwitchInfo);
                break;
        }
        _switchModeCustodian.SetMode(mode);
        SetTitle();
        if (_statViewHandler.CheckViewStatToShow(_customizationSettingsCustodian.CurrentCustomizationSettings[CurrentSwitchIndex]))
        {
            Stat stat = _statViewHandler.GetStatInfo(_customizationSettingsCustodian.CurrentCustomizationSettings[CurrentSwitchIndex].GameStats);
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

        
        
        
        if (_priceViewHandler.CalculatePriceHandler.CheckAvailableMoney(price) == true &&
            _priceViewHandler.CalculatePriceHandler.CheckAvailableHearts(priceAdditional) == true)
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
    private void SetCurrentCustomizationSettingses(int modeValue)
    {
        _customizationSettingsCustodian.CurrentCustomizationSettings = _selectedCustomizationContentIndexes.IndexesSpriteIndexes[modeValue];
    }
    private void CalculatingStats(params SwitchInfo[] switchInfo)
    {
        _switchInfoCustodian.SetStatsToCurrentSwitchInfo();
        if (_switchModeCustodian.IsStarted == false)
        {
            _calculateStatsHandler.PreliminaryStatsCalculation(switchInfo);
        }
    }
    private void CalculatingPrice(params SwitchInfo[] switchInfo)
    {
        _switchInfoCustodian.SetPriceToCurrentSwitchInfo();
        if (_switchModeCustodian.IsStarted == false)
        {
            _priceViewHandler.CalculatePriceHandler.PreliminaryBalanceCalculation(switchInfo);
        }
    }
    private void CheckAndSetClothes()
    {
        int clothesIndex = (int) ArrowSwitchMode.Clothes;
        if (_selectedCustomizationContentIndexes.IndexesSpriteIndexes[clothesIndex].Count == 0)
        {
            SetCurrentCustomizationSettingses((int) ArrowSwitchMode.Swimsuits);
            _isNuClothesReactiveProperty.Value = true;
        }
        else
        {
            SetCurrentCustomizationSettingses(clothesIndex);
            _isNuClothesReactiveProperty.Value = false;
        }
    }
}