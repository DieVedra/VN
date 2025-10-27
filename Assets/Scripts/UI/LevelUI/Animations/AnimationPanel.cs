using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class AnimationPanel
{
    private const float _fadeEndValue = 0f;
    private const float _unfadeEndValue = 1f;
    private readonly float _duration;
    private readonly RectTransform _panelTransform;
    private readonly CanvasGroup _canvasGroup;
    private readonly Vector3 _fadeEndPos;
    private readonly Vector3 _unfadeEndPos;

    public AnimationPanel(RectTransform panelTransform, CanvasGroup canvasGroup, Vector3 fadeEndPos, Vector3 unfadeEndPos, float duration)
    {
        _panelTransform = panelTransform;
        _canvasGroup = canvasGroup;
        _fadeEndPos = fadeEndPos;
        _unfadeEndPos = unfadeEndPos;
        _duration = duration;
    }
    public async UniTask FadePanel(CancellationToken cancellationToken)
    {
        await DoAnimation(cancellationToken, _fadeEndPos, _fadeEndValue);
        _panelTransform.anchoredPosition = _unfadeEndPos;
    }
    public async UniTask UnfadePanel(CancellationToken cancellationToken)
    {
        _canvasGroup.alpha = 0f;
        _panelTransform.anchoredPosition = _fadeEndPos;
        await DoAnimation(cancellationToken, _unfadeEndPos, _unfadeEndValue);
    }

    private async UniTask DoAnimation(CancellationToken cancellationToken, Vector3 endPos, float fadeEndValue)
    {
        await UniTask.WhenAll(
            _panelTransform.DOAnchorPos(endPos, _duration).WithCancellation(cancellationToken),
            _canvasGroup.DOFade(fadeEndValue, _duration).WithCancellation(cancellationToken));
    }
}