using UnityEngine;
using UnityEngine.UI;

public class PhoneScreenBaseHandler
{
    private const int _indexFirstLetter = 0;
    protected readonly GameObject Screen;
    protected readonly TopPanelHandler TopPanelHandler;
    protected readonly Image BackgroundImage;
    protected readonly Color TopPanelColor;
    
    protected PhoneScreenBaseHandler(GameObject screen, TopPanelHandler topPanelHandler, Image backgroundImage, Color topPanelColor)
    {
        Screen = screen;
        TopPanelHandler = topPanelHandler;
        BackgroundImage = backgroundImage;
        TopPanelColor = topPanelColor;
    }

    public virtual void Disable()
    {
        Screen.SetActive(false);
    }

    protected string GetFistLetter(PhoneContactDataLocalizable currentContact)
    {
        return $"{currentContact.NameContactLocalizationString.DefaultText[_indexFirstLetter]}";
    }

}