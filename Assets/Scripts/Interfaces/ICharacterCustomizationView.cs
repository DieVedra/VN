using Cysharp.Threading.Tasks;

public interface ICharacterCustomizationView
{
    public UniTask SetCharacterCustomizationFromRightArrow(CustomizationData newCustomizationData);
    public UniTask SetCharacterCustomizationFromLeftArrow(CustomizationData newCustomizationData);
    public void SetCharacterCustomization(CustomizationData newCustomizationData);
}