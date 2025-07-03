using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CharacterAnimationsOnCustomization : CharacterAnimations
{
    private const float _posY = -0.6f;
    private const float _posXOffset = -0.6f;
    private const float _rightPosX = 0.6f;
    private const float _emergenceFadeEndValue = 1f;
    private const float _disappearanceFadeEndValue = 0f;

    private readonly float _emergenceDuration;
    private readonly float _disappearanceDuration;
    private readonly int _sortingOrderDefaultValue;
    private readonly int _sortingOrderAddableValue;

    private readonly Vector3 _centralPosition;
    private readonly Vector3 _rightPosition;
    private readonly Vector3 _leftPosition;

    public CharacterAnimationsOnCustomization(Transform characterTransform, SpriteRenderer spriteRenderer,
        float emergenceDuration, float disappearanceDuration, int sortingOrderDefaultValue, int sortingOrderAddableValue)
        : base(characterTransform, spriteRenderer)
    {
        _emergenceDuration = emergenceDuration;
        _disappearanceDuration = disappearanceDuration;
        _sortingOrderDefaultValue = sortingOrderDefaultValue;
        _sortingOrderAddableValue = sortingOrderAddableValue;
        var localPosition = characterTransform.localPosition;
        _centralPosition = localPosition;
        _leftPosition = new Vector3(localPosition.x + _posXOffset, localPosition.y + _posY, localPosition.z);
        _rightPosition = new Vector3(localPosition.x + _rightPosX, localPosition.y + _posY, localPosition.z);
    }
    public override async UniTask EmergenceChar(CancellationToken cancellationToken, DirectionType type)
    {
        UpSortingOrder();
        if (type == DirectionType.Left)
        {
            // <-
            CharacterTransform.localPosition = _rightPosition;
        }
        else
        {
            //->
            CharacterTransform.localPosition = _leftPosition;
        }
        MakeInvisibleSprite();
        CharacterTransform.gameObject.SetActive(true);
        await AnimChar(cancellationToken, _centralPosition, _emergenceFadeEndValue, _emergenceDuration);
    }

    public override async UniTask DisappearanceChar(CancellationToken cancellationToken, DirectionType type)
    {
        DownSortingOrder();
        CharacterTransform.localPosition = _centralPosition;
        MakeVisibleSprite();
        CharacterTransform.gameObject.SetActive(true);
        if (type == DirectionType.Left)
        {
            // <-
            await AnimChar(cancellationToken, _leftPosition, _disappearanceFadeEndValue, _disappearanceDuration);
        }
        else
        {
            //->
            await AnimChar(cancellationToken, _rightPosition, _disappearanceFadeEndValue, _disappearanceDuration);
        }
    }

    private void UpSortingOrder()
    {
        SpriteRenderer.sortingOrder = _sortingOrderDefaultValue;
        SpriteRenderer.sortingOrder = SpriteRenderer.sortingOrder + _sortingOrderAddableValue;
    }
    private void DownSortingOrder()
    {
        SpriteRenderer.sortingOrder = _sortingOrderDefaultValue;
        SpriteRenderer.sortingOrder = SpriteRenderer.sortingOrder - _sortingOrderAddableValue;
    }
}