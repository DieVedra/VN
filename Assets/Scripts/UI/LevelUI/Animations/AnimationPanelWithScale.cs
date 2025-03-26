
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class AnimationPanelWithScale
{
    private readonly float _fadeEndValue = 0f;
    private readonly float _unfadeEndValue = 1f;
    private readonly float _duration;
    private readonly float _delay = 0.2f;
    private readonly Vector3 _defaultPosition;
    private readonly Vector3 _leftPosition;
    private readonly Vector3 _rightPosition;
    private readonly RectTransform _panelTransform;
    private readonly CanvasGroup _canvasGroup;
    
    public AnimationPanelWithScale(RectTransform panelTransform, CanvasGroup canvasGroup, 
        Vector3 rightPosition, Vector3 leftPosition, Vector3 defaultPosition, float durationAnim)
    {
        _panelTransform = panelTransform;
        _canvasGroup = canvasGroup;
        _defaultPosition = defaultPosition;
        _rightPosition = rightPosition;
        _leftPosition = leftPosition;
        _duration = durationAnim;
    }
    public async UniTask FadePanelWithScale(CancellationToken cancellationToken, DirectionType directionType)
    {
        await UniTask.WhenAll(
            _panelTransform.DOAnchorPos(directionType == DirectionType.Right ? _rightPosition : _leftPosition, _duration).WithCancellation(cancellationToken).SuppressCancellationThrow(),
            _panelTransform.DOScale(Vector3.one, _duration).WithCancellation(cancellationToken).SuppressCancellationThrow(),
            _canvasGroup.DOFade(_fadeEndValue, _duration).WithCancellation(cancellationToken).SuppressCancellationThrow());
    }
    
    public async UniTask UnfadePanelWithScale(bool toggleShowPanel, CancellationToken cancellationToken)
    {
        _canvasGroup.alpha = 0f;
        await UniTask.Delay(TimeSpan.FromSeconds(_delay), cancellationToken: cancellationToken);
        if (toggleShowPanel)
        {
            await UniTask.WhenAll(
                _panelTransform.DOAnchorPos(_defaultPosition, _duration).WithCancellation(cancellationToken).SuppressCancellationThrow(),
                _panelTransform.DOScale(Vector3.one, _duration).WithCancellation(cancellationToken).SuppressCancellationThrow(),
                _canvasGroup.DOFade(_unfadeEndValue, _duration).WithCancellation(cancellationToken).SuppressCancellationThrow());
        }
    }
}