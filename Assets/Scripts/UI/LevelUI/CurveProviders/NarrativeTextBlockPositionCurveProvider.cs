using UnityEngine;

public class NarrativeTextBlockPositionCurveProvider : CurveProvider
{
    private const float _posY1 = -105f;
    private const float _posY2 = 23f;
    private const float _posY3 = 67f;
    private const float _posY4 = 93f;
    
    public override AnimationCurve GetCurve()
    {
        return new AnimationCurve(
        new Keyframe(_lineBreaks0, _posY1), 
        new Keyframe(_lineBreaks4, _posY2),
        new Keyframe(_lineBreaks5, _posY3),
        new Keyframe(_lineBreaks6, _posY4));
    }
}