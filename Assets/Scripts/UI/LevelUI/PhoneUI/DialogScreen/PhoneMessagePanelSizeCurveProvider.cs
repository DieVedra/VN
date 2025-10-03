
using UnityEngine;

public class PhoneMessagePanelSizeCurveProvider : CurveProvider
{
    private const float _height0 = 93.5f;
    private const float _height1 = 120f;
    private const float _height2 = 180f;
    private const float _height3 = 220f;
    private const float _height4 = 260f;
    private const float _height5 = 320f;
    private const float _height6 = 360f;
    private const float _height7 = 410f;

    public override AnimationCurve GetCurve()
    {
        return new AnimationCurve(
            new Keyframe(_lineBreaks0,_height0),
            new Keyframe(_lineBreaks1,_height1),
            new Keyframe(_lineBreaks2,_height2),
            new Keyframe(_lineBreaks3,_height3),
            new Keyframe(_lineBreaks4,_height4),
            new Keyframe(_lineBreaks5,_height5),
            new Keyframe(_lineBreaks6,_height6),
            new Keyframe(_lineBreaks7,_height7));
    }
}