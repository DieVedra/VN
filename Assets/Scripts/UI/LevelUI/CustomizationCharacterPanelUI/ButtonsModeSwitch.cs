﻿using TMPro;
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
    private readonly CalculatePriceHandler _calculatePriceHandler;
    private readonly CustomizationDataProvider _customizationDataProvider;
    private readonly CustomizationPanelResourceHandler _customizationPanelResourceHandler;
    private readonly CustomizationPanelResourceAndPricePanelBroker _customizationPanelResourceAndPricePanelBroker;
    private readonly ReactiveProperty<bool> _isNuClothesReactiveProperty;
    private CompositeDisposable _compositeDisposable;
    private int CurrentSwitchIndex => _switchInfoCustodian.CurrentSwitchInfo.Index;


    public ButtonsModeSwitch(
        ICharacterCustomizationView characterCustomizationView, SelectedCustomizationContentIndexes selectedCustomizationContentIndexes,
        SwitchModeCustodian switchModeCustodian, TextMeshProUGUI titleTextComponent, StatViewHandler statViewHandler, PriceViewHandler priceViewHandler,
        CustomizationSettingsCustodian customizationSettingsCustodian, SwitchInfoCustodian switchInfoCustodian, ButtonPlayHandler buttonPlayHandler,
        CalculateStatsHandler calculateStatsHandler, CalculatePriceHandler calculatePriceHandler,
        CustomizationDataProvider customizationDataProvider, CustomizationPanelResourceHandler customizationPanelResourceHandler,
        CustomizationPanelResourceAndPricePanelBroker customizationPanelResourceAndPricePanelBroker,
        ReactiveProperty<bool> isNuClothesReactiveProperty, SetLocalizationChangeEvent setLocalizationChangeEvent)
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
        _calculatePriceHandler = calculatePriceHandler;
        _customizationDataProvider = customizationDataProvider;
        _customizationPanelResourceHandler = customizationPanelResourceHandler; //55
        _customizationPanelResourceAndPricePanelBroker = customizationPanelResourceAndPricePanelBroker;
        _isNuClothesReactiveProperty = isNuClothesReactiveProperty;
        _compositeDisposable = setLocalizationChangeEvent.SubscribeWithCompositeDisposable(SetTitle);
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

        _customizationPanelResourceAndPricePanelBroker.CalculateAndSetMode(_switchInfoCustodian.GetAllInfo());
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

        _customizationPanelResourceHandler.TryShowOrHidePanelOnButtonsSwitch(_customizationPanelResourceAndPricePanelBroker.CurrentResourcesViewMode);
        
        if (_calculatePriceHandler.CheckAvailableMoney(price) == true &&
            _calculatePriceHandler.CheckAvailableHearts(priceAdditional) == true)
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
        // if (_switchModeCustodian.IsStarted == false)
        // {
        // }

        _calculateStatsHandler.PreliminaryStatsCalculation(switchInfo);
    }
    private void CalculatingPrice(params SwitchInfo[] switchInfo)
    {
        _switchInfoCustodian.SetPriceToCurrentSwitchInfo();
        _switchInfoCustodian.SetAdditionalPriceToCurrentSwitchInfo();
        // if (_switchModeCustodian.IsStarted == false)
        // {
        //     Debug.Log(111);
        // }

        _calculatePriceHandler.PreliminaryBalanceCalculation(switchInfo);
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