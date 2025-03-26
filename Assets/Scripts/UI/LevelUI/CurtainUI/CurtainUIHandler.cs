
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CurtainUIHandler
{
    protected readonly CurtainUI CurtainUI;
    protected readonly Image CurtainImage;
    public CurtainUIHandler(CurtainUI curtainUI)
    {
        CurtainUI = curtainUI;
        CurtainImage = curtainUI.Image;
        if (Application.isPlaying == true)
        {
            CurtainUI.gameObject.SetActive(true);
            SkipAtCloses();
        }
        else
        {
            CurtainUI.gameObject.SetActive(false);
        }
    }
    public virtual async UniTask CurtainOpens(CancellationToken cancellationToken)
    {
        CurtainUI.gameObject.SetActive(true);
        CurtainUI.Image.color = Color.black;
        await UniTask.WhenAny(CurtainImage.DOFade(0f, CurtainUI.DurationAnim).WithCancellation(cancellationToken),
            UniTask.Delay(TimeSpan.FromSeconds(CurtainUI.DurationAnim - CurtainUI.UnfadeSkipValue), cancellationToken: cancellationToken));
        CurtainUI.gameObject.SetActive(false);
    }

    public virtual async UniTask CurtainCloses(CancellationToken cancellationToken)
    {
        CurtainUI.gameObject.SetActive(true);
        CurtainUI.Image.color = Color.clear;
        await CurtainImage.DOFade(1f, CurtainUI.DurationAnim).WithCancellation(cancellationToken);
    }
    public void SkipAtOpens()
    {
        CurtainUI.Image.color = Color.clear;
    }
    public void SkipAtCloses()
    {
        CurtainUI.Image.color = Color.black;
    }
}