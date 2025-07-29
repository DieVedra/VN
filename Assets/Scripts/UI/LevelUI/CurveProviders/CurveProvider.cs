using UnityEngine;

public abstract class CurveProvider
{
    protected const float _lineBreaks0 = 0f;
    protected const float _lineBreaks1 = 1f;
    protected const float _lineBreaks2 = 2f;
    protected const float _lineBreaks3 = 3f;
    protected const float _lineBreaks4 = 4f;
    protected const float _lineBreaks5 = 5f;
    protected const float _lineBreaks6 = 6f;

    public abstract AnimationCurve GetCurve();
}