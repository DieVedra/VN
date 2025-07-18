using UnityEngine;

public class PriceViewHandlerValues
{
    public const float MinValue = 0f;
    public const float MaxValue = 1f;
    private const float _posYMonetBlock = -156;
    private const float _posYHeartsBlock = -258;
    
    private const float _posY1PricePanel = -251;
    private const float _posX1PricePanel = -179;
    
    private const float _posY2PricePanel = -382;
    private const float _posX2PricePanel = -169;

    private const float _posY1ImageBackground = -115;
    private const float _height1ImageBackground = 243;
    
    private const float _posY2ImageBackground = -163;
    private const float _height2ImageBackground = 340;
    private const float _widhtImageBackground = 272;
    
    public Vector2 Mode1PosPricePanel => new Vector2(_posX1PricePanel, _posY1PricePanel);
    public Vector2 Mode2PosPricePanel => new Vector2(_posX2PricePanel, _posY2PricePanel);
    public Vector2 Mode1PosImageBackground => new Vector2(MinValue, _posY1ImageBackground);
    public Vector2 Mode2PosImageBackground => new Vector2(MinValue, _posY2ImageBackground);
    public Vector2 Mode1SizeImageBackground => new Vector2(_widhtImageBackground, _height1ImageBackground);
    public Vector2 Mode2SizeImageBackground => new Vector2(_widhtImageBackground, _height2ImageBackground);
    public Vector2 PosMonetBlock => new Vector2(MinValue, _posYMonetBlock);
    public Vector2 PosHeartsBlock => new Vector2(MinValue, _posYHeartsBlock);
}