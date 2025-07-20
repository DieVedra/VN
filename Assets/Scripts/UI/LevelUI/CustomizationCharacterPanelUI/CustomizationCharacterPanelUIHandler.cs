using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;

public class CustomizationCharacterPanelUIHandler
{
    private const float _delay = 0.5f;
    private const string _default = "0";
    private readonly CustomizationCharacterPanelUI _customizationCharacterPanelUI;
    private readonly ResourcePanelWithCanvasGroupView _monetPanel;
    private readonly ResourcePanelWithCanvasGroupView _heartsPanel;
    private readonly StatViewHandler _statViewHandler;
    private CustomizationPanelResourceHandler _customizationPanelResourceHandler;
    private ButtonsCustomizationHandler _buttonsCustomizationHandler;
    private ArrowSwitch _arrowSwitch;
    private ButtonsModeSwitch _buttonsModeSwitch;
    private PriceViewHandler _priceViewHandler;
    private ButtonPlayHandler _buttonPlayHandler;
    private CancellationTokenSource _cancellationTokenSource;
    public ButtonsCustomizationHandler ButtonsCustomizationHandler => _buttonsCustomizationHandler;
    
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
        _buttonsCustomizationHandler.DeactivateButtonsCustomization();
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
        SetNumbersContent(selectedCustomizationContentIndexes);
        _customizationCharacterPanelUI.PriceUIView.gameObject.SetActive(true);
    }

    public void ShowCustomizationContentInPlayMode(ICharacterCustomizationView characterCustomizationView,
        SelectedCustomizationContentIndexes selectedCustomizationContentIndexes, Wallet wallet,
        CalculateStatsHandler calculateStatsHandler, SetLocalizationChangeEvent setLocalizationChangeEvent)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        ReactiveProperty<ArrowSwitchMode> arrowSwitchModeReactiveProperty = new ReactiveProperty<ArrowSwitchMode>(selectedCustomizationContentIndexes.StartMode);
        CustomizationSettingsCustodian customizationSettingsCustodian = new CustomizationSettingsCustodian();
        
        SwitchInfoCustodian switchInfoCustodian = new SwitchInfoCustodian(selectedCustomizationContentIndexes, customizationSettingsCustodian,
            calculateStatsHandler);
        CustomizationPanelResourceAndPricePanelBroker customizationPanelResourceAndPricePanelBroker 
            = new CustomizationPanelResourceAndPricePanelBroker(switchInfoCustodian);
        ReactiveProperty<bool> isNuClothesReactiveProperty = new ReactiveProperty<bool>();
        CalculateBalanceHandler calculateBalanceHandler = new CalculateBalanceHandler(
            wallet.MonetsReactiveProperty, wallet.HeartsReactiveProperty, switchInfoCustodian);
        _customizationPanelResourceHandler =  new CustomizationPanelResourceHandler(
            calculateBalanceHandler, _monetPanel, _heartsPanel, _customizationCharacterPanelUI.DurationAnimResourcePanelView);

        CustomizationDataProvider customizationDataProvider = new CustomizationDataProvider(
            selectedCustomizationContentIndexes, customizationSettingsCustodian, isNuClothesReactiveProperty);

        _priceViewHandler = new PriceViewHandler(_customizationCharacterPanelUI.PriceUIView, calculateBalanceHandler,
            customizationPanelResourceAndPricePanelBroker.ResourcesViewModeReactiveProperty,
             _customizationCharacterPanelUI.DurationAnimPriceView);
        
        _buttonsModeSwitch = new ButtonsModeSwitch(characterCustomizationView, selectedCustomizationContentIndexes, arrowSwitchModeReactiveProperty,
            _customizationCharacterPanelUI.TitleText, _statViewHandler, _priceViewHandler, customizationSettingsCustodian, switchInfoCustodian,
            _buttonPlayHandler, calculateStatsHandler, calculateBalanceHandler, customizationDataProvider, _customizationPanelResourceHandler,
            customizationPanelResourceAndPricePanelBroker, isNuClothesReactiveProperty, setLocalizationChangeEvent);
        
        
        _arrowSwitch = new ArrowSwitch(characterCustomizationView, selectedCustomizationContentIndexes, _statViewHandler, _priceViewHandler,
            _customizationCharacterPanelUI.TitleText, _buttonPlayHandler, arrowSwitchModeReactiveProperty, customizationSettingsCustodian, switchInfoCustodian,
            customizationDataProvider,
            _customizationPanelResourceHandler, customizationPanelResourceAndPricePanelBroker,
            isNuClothesReactiveProperty, setLocalizationChangeEvent);
        
        _buttonsCustomizationHandler = new ButtonsCustomizationHandler(_customizationCharacterPanelUI, _arrowSwitch, _buttonsModeSwitch,
            calculateStatsHandler, _priceViewHandler, switchInfoCustodian);
        
        SetNumbersContent(selectedCustomizationContentIndexes);
        _buttonsModeSwitch.SetMode(arrowSwitchModeReactiveProperty.Value);
    }

    public async UniTask HideCustomizationContentInPlayMode()
    {
        await _buttonPlayHandler.OffAnim();
        await UniTask.WhenAll(_priceViewHandler.HideAnim(), _customizationPanelResourceHandler.TryHidePanel());
        await UniTask.Delay(TimeSpan.FromSeconds(_delay), cancellationToken: _cancellationTokenSource.Token);
    }
    private void SetNumbersContent(SelectedCustomizationContentIndexes selectedCustomizationContentIndexes)
    {
        _customizationCharacterPanelUI.gameObject.SetActive(true);
        SetText(_customizationCharacterPanelUI.SkinColorButtonText,
            selectedCustomizationContentIndexes.IndexesSpriteIndexes[(int)ArrowSwitchMode.SkinColor].Count.ToString());
        SetText(_customizationCharacterPanelUI.ClothesButtonText,
            GetCountClothesOrSwimsuits(selectedCustomizationContentIndexes));
        SetText(_customizationCharacterPanelUI.HairstyleButtonText,
            selectedCustomizationContentIndexes.IndexesSpriteIndexes[(int)ArrowSwitchMode.Hairstyle].Count.ToString());
    }

    private void SetText(TextMeshProUGUI textComponent, string text)
    {
        textComponent.text = text;
    }

    private string GetCountClothesOrSwimsuits(SelectedCustomizationContentIndexes selectedCustomizationContentIndexes)
    {
        if (selectedCustomizationContentIndexes.IndexesSpriteIndexes[(int)ArrowSwitchMode.Clothes].Count > 0)
        {
            return selectedCustomizationContentIndexes.IndexesSpriteIndexes[(int) ArrowSwitchMode.Clothes].Count
                .ToString();
        }else if (selectedCustomizationContentIndexes.IndexesSpriteIndexes[(int)ArrowSwitchMode.Swimsuits].Count > 0)
        {
            return selectedCustomizationContentIndexes.IndexesSpriteIndexes[(int) ArrowSwitchMode.Swimsuits].Count
                .ToString();
        }
        else
        {
            return _default;
        }
    }
}