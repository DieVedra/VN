
public abstract class Character
{
    private readonly LocalizationString _name;

    protected Character(LocalizationString name)
    {
        _name = name;
    }

    public string MyNameText => _name.DefaultText;

    public LocalizationString Name => _name;

    public abstract MySprite GetLookMySprite(int index);
    public abstract MySprite GetEmotionMySprite(int index);

}