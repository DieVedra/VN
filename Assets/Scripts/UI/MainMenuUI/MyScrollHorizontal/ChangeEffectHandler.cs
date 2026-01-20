
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ChangeEffectHandler
{
    private readonly ReactiveProperty<int> _currentIndex;
    private readonly IReadOnlyList<StoryPanel> _contentChilds;
    private CancellationTokenSource _cancellationTokenSource;

    public ChangeEffectHandler(IReadOnlyList<StoryPanel> contentChilds, ReactiveProperty<int> currentIndex)
    {
        _contentChilds = contentChilds;
        _currentIndex = currentIndex;
    }

    public void Shutdown()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
    }

    public async UniTaskVoid PlayEffect()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        Image image = _contentChilds[_currentIndex.Value].ImageEffectChange;
        image.gameObject.SetActive(true);
        await image.DOFade(0.15f, 0.02f).WithCancellation(_cancellationTokenSource.Token);
        await image.DOFade(0f, 0.05f).WithCancellation(_cancellationTokenSource.Token);
        image.gameObject.SetActive(false);
    }
}