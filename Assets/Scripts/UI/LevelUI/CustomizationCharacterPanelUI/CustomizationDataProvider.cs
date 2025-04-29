using UniRx;

public class CustomizationDataProvider
{
    private readonly SelectedCustomizationContentIndexes _selectedCustomizationContentIndexes;
    private readonly CustomizationSettingsCustodian _customizationSettingsCustodian;
    private readonly ReactiveProperty<bool> _isNuClothesReactiveProperty;

    public CustomizationDataProvider(SelectedCustomizationContentIndexes selectedCustomizationContentIndexes, CustomizationSettingsCustodian customizationSettingsCustodian,
        ReactiveProperty<bool> isNuClothesReactiveProperty)
    {
        _selectedCustomizationContentIndexes = selectedCustomizationContentIndexes;
        _customizationSettingsCustodian = customizationSettingsCustodian;
        _isNuClothesReactiveProperty = isNuClothesReactiveProperty;
    }
    public CustomizationData CreateCustomizationData(int currentSwitchIndex)
    {
        return new CustomizationData(
            _selectedCustomizationContentIndexes.CustomizableCharacter.GetLookMySprite(),
            _selectedCustomizationContentIndexes.CustomizableCharacter.GetEmotionMySprite(),
            _selectedCustomizationContentIndexes.CustomizableCharacter.GetHairstyleSprite(),
            _isNuClothesReactiveProperty.Value == true ? _selectedCustomizationContentIndexes.CustomizableCharacter.GetSwimsuitSprite()
                : _selectedCustomizationContentIndexes.CustomizableCharacter.GetClothesSprite(),
            DirectionType.Left, _customizationSettingsCustodian.CurrentCustomizationSettings[currentSwitchIndex].Name);
    }
}