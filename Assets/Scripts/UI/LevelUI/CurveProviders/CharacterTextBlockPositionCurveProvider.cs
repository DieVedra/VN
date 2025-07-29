using UnityEngine;

public class CharacterTextBlockPositionCurveProvider : CurveProvider
{
    private const float _posY0 = -72f;
    private const float _posY1 = -56f;
    private const float _posY2 = -15f;
    private const float _posY3 = 14f;
    private const float _posY4 = 30f;
    
    public override AnimationCurve GetCurve()
    {
        return new AnimationCurve(
            new Keyframe(_lineBreaks0, _posY0),
            new Keyframe(_lineBreaks1, _posY1),
            new Keyframe(_lineBreaks2, _posY2),
            new Keyframe(_lineBreaks3, _posY3),
            new Keyframe(_lineBreaks4, _posY4));
    }
}