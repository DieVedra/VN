using UnityEngine;

public class LevelResourceHandlerValues
{
    public const float MinValue = 0f;
    public const float MaxValue = 1f;
    
    public readonly Vector2 MonetAnchorsMin;

    public readonly Vector2 MonetAnchorsMax;

    public readonly Vector2 MonetOffsetMin;

    public readonly Vector2 MonetOffsetMax;

    public readonly Vector2 HeartsAnchorsMin;

    public readonly Vector2 HeartsAnchorsMax;

    public readonly Vector2 HeartsOffsetMin;

    public readonly Vector2 HeartsOffsetMax;

    public LevelResourceHandlerValues(RectTransform monetResourcePanelRectTransform, RectTransform heartsResourcePanelRectTransform)
    {
        MonetAnchorsMin = monetResourcePanelRectTransform.anchorMin;
        MonetAnchorsMax = monetResourcePanelRectTransform.anchorMax;
        MonetOffsetMin = monetResourcePanelRectTransform.offsetMin;
        MonetOffsetMax = monetResourcePanelRectTransform.offsetMax;
        HeartsAnchorsMin = heartsResourcePanelRectTransform.anchorMin;
        HeartsAnchorsMax = heartsResourcePanelRectTransform.anchorMax;
        HeartsOffsetMin = heartsResourcePanelRectTransform.offsetMin;
        HeartsOffsetMax = heartsResourcePanelRectTransform.offsetMax;
    }
}