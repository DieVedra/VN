using TMPro;
using UniRx;

public class CustomizationCharacterPanelUIHandler
{
    private readonly CustomizationCharacterPanelUI _customizationCharacterPanelUI;
    private readonly LevelResourceHandler _levelResourceHandler;
    private readonly StatViewHandler _statViewHandler;
    private ButtonsCustomizationHandler _buttonsCustomizationHandler;
    private ArrowSwitch _arrowSwitch;
    private ButtonsModeSwitch _buttonsModeSwitch;
    private PriceViewHandler _priceViewHandler;
    
    public ButtonPlayHandler ButtonPlayHandler { get; private set; }
    public ButtonsCustomizationHandler ButtonsCustomizationHandler => _buttonsCustomizationHandler;
    
    public CustomizationCharacterPanelUIHandler(CustomizationCharacterPanelUI customizationCharacterPanelUI, LevelResourceHandler levelResourceHandler)
    {
        _customizationCharacterPanelUI = customizationCharacterPanelUI;
        _levelResourceHandler = levelResourceHandler;
        ButtonPlayHandler = new ButtonPlayHandler(customizationCharacterPanelUI.PlayButtonCanvasGroup, customizationCharacterPanelUI.DurationButtonPlay);
        _statViewHandler = new StatViewHandler(customizationCharacterPanelUI.StatPanelCanvasGroup, customizationCharacterPanelUI.StatPanelText,
            customizationCharacterPanelUI.DurationAnimStatView);
    }

    public void Dispose()
    {
        _buttonsCustomizationHandler.DeactivateButtonsCustomization();
        _arrowSwitch.Dispose();
        ButtonPlayHandler.Dispose();
        _statViewHandler.Dispose();
        _buttonsCustomizationHandler = null;
        _arrowSwitch = null;
        _customizationCharacterPanelUI.gameObject.SetActive(false);
        _customizationCharacterPanelUI.PriceUIView.gameObject.SetActive(false);
        _buttonsModeSwitch.Dispose();
        _priceViewHandler.Dispose();
    }
    public void SetContentInEditMode(SelectedCustomizationContentIndexes selectedCustomizationContentIndexes)
    {
        SetNumbersContent(selectedCustomizationContentIndexes);
        _customizationCharacterPanelUI.PriceUIView.gameObject.SetActive(true);
    }

    public void ShowCustomizationContentInPlayMode(ICharacterCustomizationView characterCustomizationView, SelectedCustomizationContentIndexes selectedCustomizationContentIndexes, 
        CalculatePriceHandler calculatePriceHandler, CalculateStatsHandler calculateStatsHandler, SetLocalizationChangeEvent setLocalizationChangeEvent)
    {
        SwitchModeCustodian switchModeCustodian = new SwitchModeCustodian(selectedCustomizationContentIndexes.StartMode);
        CustomizationSettingsCustodian customizationSettingsCustodian = new CustomizationSettingsCustodian();
        
        SwitchInfoCustodian switchInfoCustodian = new SwitchInfoCustodian(selectedCustomizationContentIndexes, customizationSettingsCustodian,
            calculateStatsHandler);
        
        ReactiveProperty<bool> isNuClothesReactiveProperty = new ReactiveProperty<bool>();
        CustomizationDataProvider customizationDataProvider = new CustomizationDataProvider(selectedCustomizationContentIndexes, customizationSettingsCustodian, isNuClothesReactiveProperty);

        _priceViewHandler = new PriceViewHandler(_customizationCharacterPanelUI.PriceUIView, calculatePriceHandler,
             _customizationCharacterPanelUI.DurationAnimPriceView);
        
        _buttonsModeSwitch = new ButtonsModeSwitch(characterCustomizationView, selectedCustomizationContentIndexes, switchModeCustodian,
            _customizationCharacterPanelUI.TitleText, _statViewHandler, _priceViewHandler, customizationSettingsCustodian, switchInfoCustodian,
            ButtonPlayHandler, calculateStatsHandler, customizationDataProvider, _levelResourceHandler, isNuClothesReactiveProperty, setLocalizationChangeEvent);
        
        
        _arrowSwitch = new ArrowSwitch(characterCustomizationView, selectedCustomizationContentIndexes, _statViewHandler, _priceViewHandler,
            _customizationCharacterPanelUI.TitleText, ButtonPlayHandler, switchModeCustodian, customizationSettingsCustodian, switchInfoCustodian,
            customizationDataProvider, _levelResourceHandler, isNuClothesReactiveProperty, setLocalizationChangeEvent);
        
        _buttonsCustomizationHandler = new ButtonsCustomizationHandler(_customizationCharacterPanelUI, _arrowSwitch, _buttonsModeSwitch,
            calculateStatsHandler, _priceViewHandler, switchInfoCustodian);
        
        SetNumbersContent(selectedCustomizationContentIndexes);
        _buttonsModeSwitch.SetMode(switchModeCustodian.Mode);
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
            return "0";
        }
    }
}