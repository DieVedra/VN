
using TMPro;
using UnityEngine;

public class ChoiceHeightHandler
{
    private readonly float _defaultPosY = 260f;
    private readonly RectTransform _button1Transform;
    private readonly RectTransform _centralButton2Transform;
    private readonly RectTransform _button3Transform;

    private readonly TextMeshProUGUI _textButtonChoice1;
    private readonly TextMeshProUGUI _textCentralButtonChoice2;
    private readonly TextMeshProUGUI _textButtonChoice3;
    
    private readonly AnimationCurve _animationCurveHeight;
    private readonly AnimationCurve _animationCurveRange;

    public ChoiceHeightHandler(RectTransform button1Transform, RectTransform centralButton2Transform, RectTransform button3Transform,
        TextMeshProUGUI textButtonChoice1, TextMeshProUGUI textCentralButtonChoice2, TextMeshProUGUI textButtonChoice3)
    {
        _button1Transform = button1Transform;
        _centralButton2Transform = centralButton2Transform;
        _button3Transform = button3Transform;
        _textButtonChoice1 = textButtonChoice1;
        _textCentralButtonChoice2 = textCentralButtonChoice2;
        _textButtonChoice3 = textButtonChoice3;
        _animationCurveHeight = new AnimationCurve(new Keyframe(1f, 200f), new Keyframe(2f, 230f),
            new Keyframe(3f, 300f), new Keyframe(4f, 350f), new Keyframe(5f, 420f), new Keyframe(6f, 480f));
        _animationCurveRange = new AnimationCurve(new Keyframe(3f, 40f),
            new Keyframe(4f, 80f), new Keyframe(5f, 100f), new Keyframe(6f, 140f));
    }

    public void UpdateHeight(ChoiceData data)
    {
        _textButtonChoice1.ForceMeshUpdate();
        CalculateHeightPanels(_button1Transform, _textButtonChoice1.textInfo.lineCount);
        _textCentralButtonChoice2.ForceMeshUpdate();
        CalculateHeightPanels(_centralButton2Transform, _textCentralButtonChoice2.textInfo.lineCount);

        if (data.ShowChoice3)
        {
            _textButtonChoice3.ForceMeshUpdate();
            CalculateHeightPanels(_button3Transform, _textButtonChoice3.textInfo.lineCount);
        }
        CalculatePosPanels(data.ShowChoice3);
    }

    private void CalculateHeightPanels(RectTransform rectTransform, int linesCount)
    {
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, _animationCurveHeight.Evaluate(linesCount));
    }

    private void CalculatePosPanels(bool showChoice3)
    {
        SetPosPanel(_button1Transform, _defaultPosY);
        SetPosPanel(_button3Transform, -_defaultPosY);
        float addValue = GetAddValue(_textCentralButtonChoice2.textInfo.lineCount);
        SetPosPanel(_button1Transform, _button1Transform.anchoredPosition.y + addValue);
        if (showChoice3)
        {
            SetPosPanel(_button3Transform, _button3Transform.anchoredPosition.y - addValue);
        }

        addValue = GetAddValue(_textButtonChoice1.textInfo.lineCount);
        SetPosPanel(_button1Transform, _button1Transform.anchoredPosition.y + addValue);

        if (showChoice3)
        {
            addValue = GetAddValue(_textButtonChoice3.textInfo.lineCount);
            SetPosPanel(_button3Transform, _button3Transform.anchoredPosition.y - addValue);
        }
    }

    private float GetAddValue(int lineCount)
    {
        if (lineCount > 2 && lineCount < 7)
        {
            return _animationCurveRange.Evaluate(lineCount);
        }
        else
        {
            return 0;
        }
    }

    private void SetPosPanel(RectTransform rectTransform, float y)
    {
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, y);
    }
}