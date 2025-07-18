

using UnityEngine;

public class LevelResourceHandlerValues
{
    public const float MinValue = 0f;
    public const float MaxValue = 1f;
    
    public const float _monetPanelPosX = 1f;
    public const float _heartsPanelPosX = 1f;
    
    private const float _monetAnchorsMinX = 0.01759259f;
    private const float _monetAnchorsMinY = 0.913f;
    private const float _monetAnchorsMaxX = 0.355f;
    private const float _monetAnchorsMaxY = 0.9725209f;
    
    private const float _monetOffsetLeft = 6f;
    private const float _monetOffsetBottom = 7.524353f;
    private const float _monetOffsetRight = 7f;
    private const float _monetOffsetTop = 6.932068f;
    
    
    private const float _heartsAnchorsMinX = 0.434f;
    private const float _heartsAnchorsMinY = 0.913f;
    private const float _heartsAnchorsMaxX = 0.7690001f;
    private const float _heartsAnchorsMaxY = 0.9725209f;
    
    private const float _heartsOffsetLeft = 5.5f;
    private const float _heartsOffsetBottom = 7.524353f;
    private const float _heartsOffsetRight = 5.5f;
    private const float _heartsOffsetTop = 6.932068f;
    
    
    public Vector2 MonetAnchorsMin => new Vector2(_monetAnchorsMinX, _monetAnchorsMinY);
    public Vector2 MonetAnchorsMax => new Vector2(_monetAnchorsMaxX, _monetAnchorsMaxY);
    public Vector2 MonetOffsetMin => new Vector2(_monetOffsetLeft, _monetOffsetBottom);
    public Vector2 MonetOffsetMax => new Vector2(_monetOffsetRight, _monetOffsetTop);
    
    public Vector2 HeartsAnchorsMin => new Vector2(_heartsAnchorsMinX, _heartsAnchorsMinY);
    public Vector2 HeartsAnchorsMax => new Vector2(_heartsAnchorsMaxX, _heartsAnchorsMaxY);
    public Vector2 HeartsOffsetMin => new Vector2(_heartsOffsetLeft, _heartsOffsetBottom);
    public Vector2 HeartsOffsetMax => new Vector2(_heartsOffsetRight, _heartsOffsetTop);
}