using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public abstract class CharacterAnimations
{
    protected readonly Transform CharacterTransform;
    protected readonly SpriteRenderer SpriteRenderer;

    protected CharacterAnimations(Transform characterTransform, SpriteRenderer spriteRenderer)
    {
        CharacterTransform = characterTransform;
        SpriteRenderer = spriteRenderer;
    }

    public abstract UniTask DisappearanceChar(CancellationToken cancellationToken, DirectionType directionType);
    public abstract UniTask EmergenceChar(CancellationToken cancellationToken, DirectionType directionType);
    public void MakeInvisibleSprite()
    {
        MakeAlphaSprite(0f);
    }
    public void MakeVisibleSprite()
    {
        MakeAlphaSprite(1f);
    }

    protected async UniTask AnimChar(CancellationToken cancellationToken, Vector3 endPos, float fadeEndValue, float duration)
    {
        await UniTask.WhenAll(
            CharacterTransform.DOLocalMove(endPos, duration).WithCancellation(cancellationToken),
            SpriteRenderer.DOFade(fadeEndValue, duration).WithCancellation(cancellationToken));
    }

    private void MakeAlphaSprite(float alpha)
    {
        Color color = SpriteRenderer.color;
        color.a = alpha;
        SpriteRenderer.color = color;
    }
}