using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

public class CustomizationCharacterPanelUIHandler
{
    private const float _delay = 0.5f;
    private readonly CustomizationCharacterPanelUI _customizationCharacterPanelUI;
    private readonly StatViewHandler _statViewHandler;
    private readonly ButtonPlayHandler _buttonPlayHandler;
    private readonly PanelResourceHandler _panelResourceHandler;
    private ButtonsCustomizationHandler _buttonsCustomizationHandler;
    private ArrowSwitch _arrowSwitch;
    private ButtonsModeSwitch _buttonsModeSwitch;
    private PriceViewHandler _priceViewHandler;
    private CancellationTokenSource _cancellationTokenSource;
    
    public CustomizationCharacterPanelUIHandler(CustomizationCharacterPanelUI customizationCharacterPanelUI, PanelResourceHandler panelResourceHandler)
    {
        _customizationCharacterPanelUI = customizationCharacterPanelUI;
        _panelResourceHandler = panelResourceHandler;
        _buttonPlayHandler = new ButtonPlayHandler(customizationCharacterPanelUI.PlayButtonCanvasGroup, customizationCharacterPanelUI.DurationButtonPlay);
        _statViewHandler = new StatViewHandler(customizationCharacterPanelUI.StatPanelCanvasGroup, customizationCharacterPanelUI.StatPanelText,
            customizationCharacterPanelUI.DurationAnimStatView);
    }

    public void ShutDown()
    {
        _buttonsCustomizationHandler?.DeactivateButtonsCustomization(_customizationCharacterPanelUI);
        _arrowSwitch?.Dispose();
        _buttonPlayHandler?.Dispose();
        _statViewHandler?.Dispose();
        _buttonsCustomizationHandler = null;
        _arrowSwitch = null;
        _customizationCharacterPanelUI?.gameObject.SetActive(false);
        _customizationCharacterPanelUI?.PriceUIView.gameObject.SetActive(false);
        _buttonsModeSwitch?.Dispose();
        _priceViewHandler?.Dispose();
        _cancellationTokenSource?.Cancel();
    }
    public void SetContentInEditMode()
    {
        _customizationCharacterPanelUI.PriceUIView.gameObject.SetActive(true);
        _customizationCharacterPanelUI.gameObject.SetActive(true);
    }

    public void ShowCustomizationContentInPlayMode(ICharacterCustomizationView characterCustomizationView,
        SelectedCustomizationContentIndexes selectedCustomizationContentIndexes, Wallet wallet,
        CalculateStatsHandler calculateStatsHandler, SetLocalizationChangeEvent setLocalizationChangeEvent,
        CustomizationEndEvent<CustomizationResult> customizationEndEvent)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        var resourcesViewMode = CalculateResourcesViewMode(selectedCustomizationContentIndexes);
        ReactiveProperty<ArrowSwitchMode> arrowSwitchModeReactiveProperty = new ReactiveProperty<ArrowSwitchMode>(selectedCustomizationContentIndexes.StartMode);
        CustomizationSettingsCustodian customizationSettingsCustodian = new CustomizationSettingsCustodian();
        SwitchInfoCustodian switchInfoCustodian = new SwitchInfoCustodian(selectedCustomizationContentIndexes, customizationSettingsCustodian,
            calculateStatsHandler);
        ReactiveProperty<bool> isNuClothesReactiveProperty = new ReactiveProperty<bool>();
        ReactiveCommand<bool> offArrows = new ReactiveCommand<bool>();
        PreliminaryBalanceCalculator preliminaryBalanceCalculator = new PreliminaryBalanceCalculator(wallet);
        
        _panelResourceHandler.Init(resourcesViewMode);

        CustomizationDataProvider customizationDataProvider = new CustomizationDataProvider(
            selectedCustomizationContentIndexes, customizationSettingsCustodian, isNuClothesReactiveProperty);

        _priceViewHandler = new PriceViewHandler(_customizationCharacterPanelUI.PriceUIView,
            resourcesViewMode, _customizationCharacterPanelUI.DurationAnimPriceView);
        
        _buttonsModeSwitch = new ButtonsModeSwitch(characterCustomizationView, selectedCustomizationContentIndexes, arrowSwitchModeReactiveProperty,
            _customizationCharacterPanelUI.TitleText, _statViewHandler, _priceViewHandler, customizationSettingsCustodian, switchInfoCustodian,
            _buttonPlayHandler, calculateStatsHandler, preliminaryBalanceCalculator, customizationDataProvider,
            isNuClothesReactiveProperty, offArrows, setLocalizationChangeEvent);
        
        
        _arrowSwitch = new ArrowSwitch(characterCustomizationView, selectedCustomizationContentIndexes, _statViewHandler, _priceViewHandler,
            _customizationCharacterPanelUI.TitleText, _buttonPlayHandler, arrowSwitchModeReactiveProperty, customizationSettingsCustodian,
            switchInfoCustodian, customizationDataProvider, preliminaryBalanceCalculator,
            isNuClothesReactiveProperty, setLocalizationChangeEvent);
        
        _buttonsCustomizationHandler = new ButtonsCustomizationHandler(
            _customizationCharacterPanelUI, _arrowSwitch, _buttonsModeSwitch,
            calculateStatsHandler, preliminaryBalanceCalculator, switchInfoCustodian, offArrows);
        
        _buttonsCustomizationHandler.ActivateButtonsCustomization(_customizationCharacterPanelUI, selectedCustomizationContentIndexes,
            customizationEndEvent, _arrowSwitch);
        _buttonsModeSwitch.SetMode(arrowSwitchModeReactiveProperty.Value);
        _customizationCharacterPanelUI.gameObject.SetActive(true);
    }

    public async UniTask HideCustomizationContentInPlayMode()
    {
        await _buttonPlayHandler.OffAnim();
        await _priceViewHandler.HideAnim();
        await UniTask.Delay(TimeSpan.FromSeconds(_delay), cancellationToken: _cancellationTokenSource.Token);
        await _panelResourceHandler.TryHidePanel();
        _panelResourceHandler.Dispose();
        _buttonsCustomizationHandler.Dispose();
    }

    private ResourcesViewMode CalculateResourcesViewMode(SelectedCustomizationContentIndexes selectedCustomizationContentIndexes)
    {
        ResourcesViewModeCalculator resourcesViewModeCalculator = new ResourcesViewModeCalculator();
        (int,int) prices = (0,0);
        foreach (var settings in selectedCustomizationContentIndexes.IndexesSpriteIndexes)
        {
            foreach (var t in settings)
            {
                prices.Item1 += t.Price;
                prices.Item2 += t.PriceAdditional;
            }
        }
        return resourcesViewModeCalculator.CalculateResourcesViewMode(prices);
    }
}