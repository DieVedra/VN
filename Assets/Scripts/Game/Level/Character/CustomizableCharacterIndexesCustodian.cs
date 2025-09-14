using UniRx;

public class CustomizableCharacterIndexesCustodian
{
    public readonly string NameKey;
    public readonly ReactiveProperty<int> BodyIndexRP;
    public readonly ReactiveProperty<int> ClothesIndexRP;
    public readonly ReactiveProperty<int> SwimsuitsIndexRP;
    public readonly ReactiveProperty<int> HairstyleIndexRP;
    
    public readonly ReactiveProperty<bool> BuferCurrentClothesIsActiveRP;
    public readonly ReactiveProperty<int> BuferCurrentClothesIndexRP;
    
    public readonly ReactiveProperty<bool> BuferCurrentSwimsuitsIsActiveRP;
    public readonly ReactiveProperty<int> BuferCurrentSwimsuitsIndexRP;
    
    public readonly ReactiveProperty<bool> BuferCurrentHairstyleIsActiveRP;
    public readonly ReactiveProperty<int> BuferCurrentHairstyleIndexRP;
    public CustomizableCharacterIndexesCustodian(string nameKey)
    {
        BodyIndexRP = new ReactiveProperty<int>();
        ClothesIndexRP = new ReactiveProperty<int>();
        SwimsuitsIndexRP = new ReactiveProperty<int>();
        HairstyleIndexRP = new ReactiveProperty<int>();
        BuferCurrentClothesIsActiveRP = new BoolReactiveProperty(false);
        BuferCurrentSwimsuitsIsActiveRP = new BoolReactiveProperty(false);
        BuferCurrentHairstyleIsActiveRP = new BoolReactiveProperty(false);
        
        BuferCurrentClothesIndexRP = new ReactiveProperty<int>();
        BuferCurrentSwimsuitsIndexRP = new ReactiveProperty<int>();
        BuferCurrentHairstyleIndexRP = new ReactiveProperty<int>();
        NameKey = nameKey;
    }
    
    public CustomizableCharacterIndexesCustodian(WardrobeSaveData wardrobeSaveData, string nameKey)
    {
        BodyIndexRP = new ReactiveProperty<int>(wardrobeSaveData.CurrentBodyIndex);
        ClothesIndexRP = new ReactiveProperty<int>(wardrobeSaveData.CurrentClothesIndex);
        SwimsuitsIndexRP = new ReactiveProperty<int>(wardrobeSaveData.CurrentSwimsuitsIndex);
        HairstyleIndexRP = new ReactiveProperty<int>(wardrobeSaveData.CurrentHairstyleIndex);

        BuferCurrentClothesIsActiveRP = new BoolReactiveProperty(wardrobeSaveData.BuferCurrentClothesIsActive);
        BuferCurrentHairstyleIsActiveRP = new BoolReactiveProperty(wardrobeSaveData.BuferCurrentHairstyleIsActive);
        BuferCurrentSwimsuitsIsActiveRP = new BoolReactiveProperty(wardrobeSaveData.BuferCurrentSwimsuitsIsActive);
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
            CurrentSwimsuitsIndex = SwimsuitsIndexRP.Value,
            
            BuferCurrentClothesIsActive = BuferCurrentClothesIsActiveRP.Value,
            BuferCurrentSwimsuitsIsActive = BuferCurrentSwimsuitsIsActiveRP.Value,
            BuferCurrentHairstyleIsActive = BuferCurrentHairstyleIsActiveRP.Value,
            BuferCurrentClothesIndex = BuferCurrentClothesIndexRP.Value,
            BuferCurrentSwimsuitsIndex = BuferCurrentSwimsuitsIndexRP.Value,
            BuferCurrentHairstyleIndex = BuferCurrentHairstyleIndexRP.Value
        };
    }
}