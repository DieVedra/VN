
public abstract class Character
{
    private LocalizationString _name;
    public string MyNameText => _name.DefaultText;

    public LocalizationString Name => _name;

    public abstract MySprite GetLookMySprite(int index);
    public abstract MySprite GetEmotionMySprite(int index);

}