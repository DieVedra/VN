using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class PhoneScreenBaseHandler
{
    private const int _indexFirstLetter = 0;
    public const int LoopsCount = -1;
    protected const float AlphaMax = 1f;
    protected const float AlphaMin = 0.5f;

    protected readonly GameObject Screen;
    protected readonly Image BackgroundImage;
    
    protected CancellationTokenSource CancellationTokenSource;

    protected PhoneScreenBaseHandler(GameObject screen, Image backgroundImage)
    {
        Screen = screen;
        BackgroundImage = backgroundImage;
    }

    public virtual void Shutdown()
    {
        Screen.SetActive(false);
    }

    protected string GetFistLetter(PhoneContact currentContact)
    {
        return $"{currentContact.NameLocalizationString.DefaultText[_indexFirstLetter]}";
    }
    
}