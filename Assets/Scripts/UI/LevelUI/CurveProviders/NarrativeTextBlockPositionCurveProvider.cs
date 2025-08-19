using UnityEngine;

public class NarrativeTextBlockPositionCurveProvider : CurveProvider
{
    private const float _posY0 = -187f;
    private const float _posY1 = -170f;
    private const float _posY2 = -137f;
    private const float _posY3 = -93f;
    private const float _posY4 = -59f;
    private const float _posY5 = -30f;
    private const float _posY6 = 11f;
    private const float _posY7 = 45f;

    public override AnimationCurve GetCurve()
    {
        return new AnimationCurve(
        new Keyframe(_lineBreaks0, _posY0), 
        new Keyframe(_lineBreaks1, _posY1), 
        new Keyframe(_lineBreaks2, _posY2), 
        new Keyframe(_lineBreaks3, _posY3), 
        new Keyframe(_lineBreaks4, _posY4),
        new Keyframe(_lineBreaks5, _posY5),
        new Keyframe(_lineBreaks6, _posY6),
        new Keyframe(_lineBreaks7, _posY7));
    }
}