

using UnityEngine;

public class NotificationPanelSizeCurveProvider : CurveProvider
{
    private const float _height0 = 600f;
    private const float _height1 = 603f;
    private const float _height2 = 663f;
    private const float _height3 = 749f;
    private const float _height4 = 809f;
    private const float _height5 = 890f;
    private const float _height6 = 963f;
    private const float _height7 = 1043f;
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