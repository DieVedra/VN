
using UnityEngine;

public class CharacterDirectionView
{
    private readonly Vector2 _leftPosition;
    private readonly Vector2 _rightPosition;
    
    private readonly SpriteRenderer _spriteRenderer;
    
    private readonly Transform _transform;
    
    public CharacterDirectionView(SpriteRenderer spriteRenderer, Vector2 leftPosition, Vector2 rightPosition)
    {
        _spriteRenderer = spriteRenderer;
        _transform = spriteRenderer.GetComponent<Transform>();
        _leftPosition = leftPosition;
        _rightPosition = rightPosition;
    }
    public void SetDirection(DirectionType directionType)
    {
        switch (directionType)
        {
            case DirectionType.Right:
                _spriteRenderer.flipX = true;
                _transform.localPosition = _rightPosition;
                break;
            case DirectionType.Left:
                _spriteRenderer.flipX = false;
                _transform.localPosition = _leftPosition;
                break;
        }
    }
}