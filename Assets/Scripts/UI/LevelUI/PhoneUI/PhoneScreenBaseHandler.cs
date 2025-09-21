using UnityEngine;
using UnityEngine.UI;

public class PhoneScreenBaseHandler
{
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
    public void BaseEnable(PhoneTime phoneTime, int butteryPercent)
    {
        Screen.SetActive(true);
        TopPanelHandler.Init(TopPanelColor, phoneTime, butteryPercent);
    }

    public virtual void Disable()
    {
        Screen.SetActive(false);
        TopPanelHandler.Dispose();
    }
}