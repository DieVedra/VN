using UniRx;

public class CustomizableCharacterIndexesCustodian
{
    public  readonly ReactiveProperty<int> BodyIndexRP;
    public  readonly ReactiveProperty<int> ClothesIndexRP;
    public  readonly ReactiveProperty<int> SwimsuitsIndexRP;
    public  readonly ReactiveProperty<int> HairstyleIndexRP;
    
    public WardrobeSaveData WardrobeSaveData { get; private set; }

    public CustomizableCharacterIndexesCustodian()
    {
        BodyIndexRP = new ReactiveProperty<int>();
        ClothesIndexRP = new ReactiveProperty<int>();
        SwimsuitsIndexRP = new ReactiveProperty<int>();
        HairstyleIndexRP = new ReactiveProperty<int>();
    }
    
    
    public CustomizableCharacterIndexesCustodian(ReactiveProperty<int> bodyIndexRP, ReactiveProperty<int> clothesIndexRP,
        ReactiveProperty<int> swimsuitsIndexRP, ReactiveProperty<int> hairstyleIndexRP)
    {
        BodyIndexRP = bodyIndexRP;
        ClothesIndexRP = clothesIndexRP;
        SwimsuitsIndexRP = swimsuitsIndexRP;
        HairstyleIndexRP = hairstyleIndexRP;
    }
}