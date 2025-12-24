using UniRx;

public class CustomizableCharacterIndexesCustodian
{
    public readonly string NameKey;
    public readonly ReactiveProperty<int> BodyIndexRP;
    public readonly ReactiveProperty<int> ClothesIndexRP;
    public readonly ReactiveProperty<int> SwimsuitsIndexRP;
    public readonly ReactiveProperty<int> HairstyleIndexRP;
    
    public readonly ReactiveProperty<bool> BufferCurrentClothesIsActiveRP;
    public readonly ReactiveProperty<int> BufferCurrentClothesIndexRP;
    
    public readonly ReactiveProperty<bool> BufferCurrentSwimsuitsIsActiveRP;
    public readonly ReactiveProperty<int> BufferCurrentSwimsuitsIndexRP;
    
    public readonly ReactiveProperty<bool> BufferCurrentHairstyleIsActiveRP;
    public readonly ReactiveProperty<int> BufferCurrentHairstyleIndexRP;
    public CustomizableCharacterIndexesCustodian(string nameKey)
    {
        BodyIndexRP = new ReactiveProperty<int>();
        ClothesIndexRP = new ReactiveProperty<int>();
        SwimsuitsIndexRP = new ReactiveProperty<int>();
        HairstyleIndexRP = new ReactiveProperty<int>();
        BufferCurrentClothesIsActiveRP = new BoolReactiveProperty(false);
        BufferCurrentSwimsuitsIsActiveRP = new BoolReactiveProperty(false);
        BufferCurrentHairstyleIsActiveRP = new BoolReactiveProperty(false);
        
        BufferCurrentClothesIndexRP = new ReactiveProperty<int>();
        BufferCurrentSwimsuitsIndexRP = new ReactiveProperty<int>();
        BufferCurrentHairstyleIndexRP = new ReactiveProperty<int>();
        NameKey = nameKey;
    }
    
    public CustomizableCharacterIndexesCustodian(WardrobeSaveData wardrobeSaveData, string nameKey)
    {
        BodyIndexRP = new ReactiveProperty<int>(wardrobeSaveData.CurrentBodyIndex);
        ClothesIndexRP = new ReactiveProperty<int>(wardrobeSaveData.CurrentClothesIndex);
        SwimsuitsIndexRP = new ReactiveProperty<int>(wardrobeSaveData.CurrentSwimsuitsIndex);
        HairstyleIndexRP = new ReactiveProperty<int>(wardrobeSaveData.CurrentHairstyleIndex);

        BufferCurrentClothesIsActiveRP = new BoolReactiveProperty(wardrobeSaveData.BuferCurrentClothesIsActive);
        BufferCurrentHairstyleIsActiveRP = new BoolReactiveProperty(wardrobeSaveData.BuferCurrentHairstyleIsActive);
        BufferCurrentSwimsuitsIsActiveRP = new BoolReactiveProperty(wardrobeSaveData.BuferCurrentSwimsuitsIsActive);
        BufferCurrentClothesIndexRP = new ReactiveProperty<int>();
        BufferCurrentSwimsuitsIndexRP = new ReactiveProperty<int>();
        BufferCurrentHairstyleIndexRP = new ReactiveProperty<int>();
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
            
            BuferCurrentClothesIsActive = BufferCurrentClothesIsActiveRP.Value,
            BuferCurrentSwimsuitsIsActive = BufferCurrentSwimsuitsIsActiveRP.Value,
            BuferCurrentHairstyleIsActive = BufferCurrentHairstyleIsActiveRP.Value,
            BuferCurrentClothesIndex = BufferCurrentClothesIndexRP.Value,
            BuferCurrentSwimsuitsIndex = BufferCurrentSwimsuitsIndexRP.Value,
            BuferCurrentHairstyleIndex = BufferCurrentHairstyleIndexRP.Value
        };
    }
}