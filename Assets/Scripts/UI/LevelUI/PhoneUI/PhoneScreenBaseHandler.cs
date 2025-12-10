using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PhoneScreenBaseHandler
{
    private const float _scaleValueMax = 1.02f;
    private const int _indexFirstLetter = 0;
    public const float Duration = 1.5f;
    public const int LoopsCount = -1;
    protected const float AlphaMax = 1f;
    protected const float AlphaMin = 0.5f;

    protected readonly GameObject Screen;
    protected readonly TopPanelHandler TopPanelHandler;
    protected readonly Image BackgroundImage;
    protected readonly Color TopPanelColor;
    
    protected CancellationTokenSource CancellationTokenSource;

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

    protected string GetFistLetter(PhoneContact currentContact)
    {
        return $"{currentContact.NameLocalizationString.DefaultText[_indexFirstLetter]}";
    }

    protected void StartScaleAnimation<T>(IReadOnlyList<T> activeContent) where T : MonoBehaviour
    {
        if (activeContent.Count > 0)
        {
            CancellationTokenSource = new CancellationTokenSource();
            for (int i = 0; i < activeContent.Count; i++)
            {
                activeContent[i].transform.DOScale(_scaleValueMax, Duration).SetDelay(GetDelay(i))
                    .SetLoops(LoopsCount, LoopType.Yoyo).WithCancellation(CancellationTokenSource.Token);
            }

            float GetDelay(int i)
            {
                if (i != 0)
                {
                    return i * 0.5f;
                }
                else
                {
                    return 0f;
                }
            }
        }
    }
}