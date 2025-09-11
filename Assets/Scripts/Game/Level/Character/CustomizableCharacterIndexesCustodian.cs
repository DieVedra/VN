using UniRx;

public class CustomizableCharacterIndexesCustodian
{
    public readonly string NameKey;
    public readonly ReactiveProperty<int> BodyIndexRP;
    public readonly ReactiveProperty<int> ClothesIndexRP;
    public readonly ReactiveProperty<int> SwimsuitsIndexRP;
    public readonly ReactiveProperty<int> HairstyleIndexRP;

    public CustomizableCharacterIndexesCustodian(string nameKey)
    {
        BodyIndexRP = new ReactiveProperty<int>();
        ClothesIndexRP = new ReactiveProperty<int>();
        SwimsuitsIndexRP = new ReactiveProperty<int>();
        HairstyleIndexRP = new ReactiveProperty<int>();
        NameKey = nameKey;
    }
    
    public CustomizableCharacterIndexesCustodian(WardrobeSaveData wardrobeSaveData, string nameKey)
    {
        BodyIndexRP = new ReactiveProperty<int>(wardrobeSaveData.CurrentBodyIndex);
        ClothesIndexRP = new ReactiveProperty<int>(wardrobeSaveData.CurrentClothesIndex);
        SwimsuitsIndexRP = new ReactiveProperty<int>(wardrobeSaveData.CurrentSwimsuitsIndex);
        HairstyleIndexRP = new ReactiveProperty<int>(wardrobeSaveData.CurrentHairstyleIndex);
        NameKey = nameKey;
    }

    public WardrobeSaveData GetWardrobeSaveData()
    {
        return new WardrobeSaveData
        {
            NameKey = NameKey,
            CurrentBodyIndex = BodyIndexRP.Value,
            CurrentClothesIndex = ClothesIndexRP.Value,
            CurrentHairstyleIndex = HairstyleIndexRP.Value,
            CurrentSwimsuitsIndex = SwimsuitsIndexRP.Value
        };
    }
}