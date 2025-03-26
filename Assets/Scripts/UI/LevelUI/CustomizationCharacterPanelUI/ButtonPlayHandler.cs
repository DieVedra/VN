
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class ButtonPlayHandler
{
    private readonly float _duration;
    private readonly CanvasGroup _buttonCanvasGroup;
    private CancellationTokenSource _cancellationTokenSource;
    public bool IsActive { get; private set; }
    public ButtonPlayHandler(CanvasGroup buttonCanvasGroup, float duration)
    {
        _buttonCanvasGroup = buttonCanvasGroup;
        _duration = duration;
        Off();
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
    }

    public async UniTask TryOnAnim()
    {
        if (IsActive == false)
        {       
            _cancellationTokenSource = new CancellationTokenSource();
            _buttonCanvasGroup.alpha = 0.5f;
            _buttonCanvasGroup.blocksRaycasts = false;
            await _buttonCanvasGroup.DOFade(1f, _duration).WithCancellation(_cancellationTokenSource.Token);
            _buttonCanvasGroup.blocksRaycasts = true;
            IsActive = true;
        }
    }

    public async UniTask OffAnim()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _buttonCanvasGroup.blocksRaycasts = false;
        await _buttonCanvasGroup.DOFade(0.5f, _duration).WithCancellation(_cancellationTokenSource.Token);
        IsActive = false;
    }

    public void On()
    {
        _buttonCanvasGroup.alpha = 1f;
        _buttonCanvasGroup.blocksRaycasts = true;
        IsActive = true;
    }

    public void Off()
    {
        _buttonCanvasGroup.alpha = 0.5f;
        _buttonCanvasGroup.blocksRaycasts = false;
        IsActive = false;
    }
}