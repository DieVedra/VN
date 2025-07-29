using UnityEngine;

public class NarrativePanelSizeCurveProvider : CurveProvider
{
    private const float _height1 = 600f;
    private const float _height2 = 777.1f;
    private const float _height3 = 850f;
    private const float _height4 = 930f;
    private const float _height5 = 990f;

    public override AnimationCurve GetCurve()
    {
        return new AnimationCurve(
            new Keyframe(_lineBreaks0,_height1),
            new Keyframe(_lineBreaks3,_height2),
            new Keyframe(_lineBreaks4,_height3),
            new Keyframe(_lineBreaks5,_height4),
            new Keyframe(_lineBreaks6,_height5));
    }
}