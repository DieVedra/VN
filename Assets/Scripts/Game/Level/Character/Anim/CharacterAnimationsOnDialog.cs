using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CharacterAnimationsOnDialog : CharacterAnimations
{
    private readonly Vector2 _rightStartPosition;
    private readonly Transform _characterTransform;
    private readonly Vector2 _rightEndPosition;
    private readonly Vector2 _leftStartPosition;
    private readonly Vector2 _leftEndPosition;
    private readonly float _offsetValue = 0.6f;
    private readonly float _emergenceFadeEndValue = 1f;
    private readonly float _disappearanceFadeEndValue = 0f;
    private readonly float _emergenceDuration;
    private readonly float _disappearanceDuration;
    
    public CharacterAnimationsOnDialog(Transform characterTransform, SpriteRenderer spriteRenderer, Vector2 rightEndPosition, Vector2 leftEndPosition,
        float emergenceDuration, float disappearanceDuration)
        : base(characterTransform, spriteRenderer)
    {
        _characterTransform = characterTransform;
        _rightEndPosition = rightEndPosition;
        _leftEndPosition = leftEndPosition;
        _rightStartPosition = new Vector2(rightEndPosition.x + _offsetValue, rightEndPosition.y);
        _leftStartPosition = new Vector2(leftEndPosition.x - _offsetValue, rightEndPosition.y);
        _emergenceDuration = emergenceDuration;
        _disappearanceDuration = disappearanceDuration;
    }
    public override async UniTask DisappearanceChar(CancellationToken cancellationToken, DirectionType directionType)
    {
        if (directionType == DirectionType.Left)
        {
            CharacterTransform.localPosition = _leftEndPosition;
            await AnimChar(cancellationToken, _leftStartPosition, _disappearanceFadeEndValue, _disappearanceDuration);
        }
        else
        {
            CharacterTransform.localPosition = _rightEndPosition;
            await AnimChar(cancellationToken, _rightStartPosition, _disappearanceFadeEndValue, _disappearanceDuration);
        }
        _characterTransform.gameObject.SetActive(false);
    } 
    
    public override async UniTask EmergenceChar(CancellationToken cancellationToken, DirectionType directionType)
    {
        _characterTransform.gameObject.SetActive(true);
        MakeInvisibleSprite();
        if (directionType == DirectionType.Left)
        {
            CharacterTransform.localPosition = _leftStartPosition;
            await AnimChar(cancellationToken, _leftEndPosition, _emergenceFadeEndValue, _emergenceDuration);
        }
        else
        {
            CharacterTransform.localPosition = _rightStartPosition;
            await AnimChar(cancellationToken, _rightEndPosition, _emergenceFadeEndValue, _emergenceDuration);
        }
    }
}