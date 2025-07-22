using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

public class CustomizationCharacterPanelUIHandler
{
    private const float _delay = 0.5f;
    private readonly CustomizationCharacterPanelUI _customizationCharacterPanelUI;
    private readonly ResourcePanelWithCanvasGroupView _monetPanel;
    private readonly ResourcePanelWithCanvasGroupView _heartsPanel;
    private readonly StatViewHandler _statViewHandler;
    private readonly ButtonPlayHandler _buttonPlayHandler;
    private CustomizationPanelResourceHandler _customizationPanelResourceHandler;
    private ButtonsCustomizationHandler _buttonsCustomizationHandler;
    private ArrowSwitch _arrowSwitch;
    private ButtonsModeSwitch _buttonsModeSwitch;
    private PriceViewHandler _priceViewHandler;
    private CancellationTokenSource _cancellationTokenSource;
    
    public CustomizationCharacterPanelUIHandler(CustomizationCharacterPanelUI customizationCharacterPanelUI, LevelUIView levelUIView)
    {
        _customizationCharacterPanelUI = customizationCharacterPanelUI;
        _monetPanel = levelUIView.MonetPanel;
        _heartsPanel = levelUIView.HeartsPanel;
        _buttonPlayHandler = new ButtonPlayHandler(customizationCharacterPanelUI.PlayButtonCanvasGroup, customizationCharacterPanelUI.DurationButtonPlay);
        _statViewHandler = new StatViewHandler(customizationCharacterPanelUI.StatPanelCanvasGroup, customizationCharacterPanelUI.StatPanelText,
            customizationCharacterPanelUI.DurationAnimStatView);
    }

    public void Dispose()
    {
        _buttonsCustomizationHandler.DeactivateButtonsCustomization(_customizationCharacterPanelUI);
        _arrowSwitch.Dispose();
        _buttonPlayHandler.Dispose();
        _statViewHandler.Dispose();
        _buttonsCustomizationHandler = null;
        _arrowSwitch = null;
        _customizationCharacterPanelUI.gameObject.SetActive(false);
        _customizationCharacterPanelUI.PriceUIView.gameObject.SetActive(false);
        _buttonsModeSwitch.Dispose();
        _priceViewHandler.Dispose();
        _customizationPanelResourceHandler?.Dispose();
        _cancellationTokenSource?.Cancel();
    }
    public void SetContentInEditMode(SelectedCustomizationContentIndexes selectedCustomizationContentIndexes)
    {
        _buttonsCustomizationHandler.ActivateButtonsCustomization(_customizationCharacterPanelUI, selectedCustomizationContentIndexes);
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
        CalculateBalanceHandler calculateBalanceHandler = new CalculateBalanceHandler(
            wallet.MonetsReactiveProperty, wallet.HeartsReactiveProperty, switchInfoCustodian);
        _customizationPanelResourceHandler =  new CustomizationPanelResourceHandler(
            calculateBalanceHandler, _monetPanel, _heartsPanel, resourcesViewMode, _customizationCharacterPanelUI.DurationAnimResourcePanelView);

        CustomizationDataProvider customizationDataProvider = new CustomizationDataProvider(
            selectedCustomizationContentIndexes, customizationSettingsCustodian, isNuClothesReactiveProperty);

        _priceViewHandler = new PriceViewHandler(_customizationCharacterPanelUI.PriceUIView, calculateBalanceHandler,
            resourcesViewMode, _customizationCharacterPanelUI.DurationAnimPriceView);
        
        _buttonsModeSwitch = new ButtonsModeSwitch(characterCustomizationView, selectedCustomizationContentIndexes, arrowSwitchModeReactiveProperty,
            _customizationCharacterPanelUI.TitleText, _statViewHandler, _priceViewHandler, customizationSettingsCustodian, switchInfoCustodian,
            _buttonPlayHandler, calculateStatsHandler, calculateBalanceHandler, customizationDataProvider,
            isNuClothesReactiveProperty, offArrows, setLocalizationChangeEvent);
        
        
        _arrowSwitch = new ArrowSwitch(characterCustomizationView, selectedCustomizationContentIndexes, _statViewHandler, _priceViewHandler,
            _customizationCharacterPanelUI.TitleText, _buttonPlayHandler, arrowSwitchModeReactiveProperty, customizationSettingsCustodian, switchInfoCustodian,
            customizationDataProvider,
            _customizationPanelResourceHandler, 
            isNuClothesReactiveProperty, setLocalizationChangeEvent);
        
        _buttonsCustomizationHandler = new ButtonsCustomizationHandler(_customizationCharacterPanelUI, _arrowSwitch, _buttonsModeSwitch,
            calculateStatsHandler, _priceViewHandler, switchInfoCustodian, offArrows);
        
        _buttonsCustomizationHandler.ActivateButtonsCustomization(_customizationCharacterPanelUI, selectedCustomizationContentIndexes,
            customizationEndEvent, _arrowSwitch);
        _buttonsModeSwitch.SetMode(arrowSwitchModeReactiveProperty.Value);
        _customizationCharacterPanelUI.gameObject.SetActive(true);
    }

    public async UniTask HideCustomizationContentInPlayMode()
    {
        await _buttonPlayHandler.OffAnim();
        await UniTask.WhenAll(_priceViewHandler.HideAnim(), _customizationPanelResourceHandler.TryHidePanel());
        await UniTask.Delay(TimeSpan.FromSeconds(_delay), cancellationToken: _cancellationTokenSource.Token);
    }

    private ResourcesViewMode CalculateResourcesViewMode(SelectedCustomizationContentIndexes selectedCustomizationContentIndexes)
    {
        ResourcesViewMode resourcesViewMode1 = ResourcesViewMode.Hide;
        ResourcesViewMode resourcesViewMode2 = ResourcesViewMode.Hide;

        //compared сравнивамое
        //sample образец
        foreach (var settings in selectedCustomizationContentIndexes.IndexesSpriteIndexes)
        {
            for (int i = 0; i < settings.Count; i++)
            {
                if (settings[i].Price > 0)
                {
                    resourcesViewMode1 = ResourcesViewMode.MonetMode;
                }
                if (settings[i].PriceAdditional > 0)
                {
                    resourcesViewMode2 = ResourcesViewMode.HeartsMode;
                }
            }
        }

        if (resourcesViewMode1 == resourcesViewMode2)
        {
            return resourcesViewMode1;
        }
        if (resourcesViewMode1 == ResourcesViewMode.Hide && resourcesViewMode2 == ResourcesViewMode.HeartsMode)
        {
            resourcesViewMode1 = ResourcesViewMode.HeartsMode;
        }
        if (resourcesViewMode1 == ResourcesViewMode.Hide && resourcesViewMode2 == ResourcesViewMode.MonetMode)
        {
            resourcesViewMode1 = ResourcesViewMode.MonetMode;
        }
        if (resourcesViewMode1 == ResourcesViewMode.HeartsMode && resourcesViewMode2 == ResourcesViewMode.Hide)
        {
            resourcesViewMode1 = ResourcesViewMode.HeartsMode;
        }
        if (resourcesViewMode1 == ResourcesViewMode.HeartsMode && resourcesViewMode2 == ResourcesViewMode.MonetMode)
        {
            resourcesViewMode1 = ResourcesViewMode.MonetsAndHeartsMode;
        }
        if (resourcesViewMode1 == ResourcesViewMode.MonetMode && resourcesViewMode2 == ResourcesViewMode.Hide)
        {
            resourcesViewMode1 = ResourcesViewMode.MonetMode;
        }
        if (resourcesViewMode1 == ResourcesViewMode.MonetMode && resourcesViewMode2 == ResourcesViewMode.HeartsMode)
        {
            resourcesViewMode1 = ResourcesViewMode.MonetsAndHeartsMode;
        }
        return resourcesViewMode1;
    }
}