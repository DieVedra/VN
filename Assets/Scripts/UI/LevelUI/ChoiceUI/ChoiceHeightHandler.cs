
using TMPro;
using UnityEngine;

public class ChoiceHeightHandler : PanelUIHandler
{
    private readonly ChoicePanelUI _choicePanelUI;
    private readonly RectTransform _button1Transform;
    private readonly RectTransform _centralButton2Transform;
    private readonly RectTransform _button3Transform;

    private readonly TextMeshProUGUI _textButtonChoice1;
    private readonly TextMeshProUGUI _textCentralButtonChoice2;
    private readonly TextMeshProUGUI _textButtonChoice3;

    private Vector2 Size;


    public ChoiceHeightHandler(ChoicePanelUI choicePanelUI)
    {
        _choicePanelUI = choicePanelUI;
        _button1Transform = choicePanelUI.RectTransformChoice1;
        _centralButton2Transform = choicePanelUI.RectTransformChoice2;
        _button3Transform = choicePanelUI.RectTransformChoice3;
        
        _textButtonChoice1 = choicePanelUI.TextButtonChoice1;
        _textCentralButtonChoice2 = choicePanelUI.TextButtonChoice2;
        _textButtonChoice3 = choicePanelUI.TextButtonChoice3;
        SetPosPanel(_centralButton2Transform, choicePanelUI.DefaultPosYCentralButtonChoice2);
    }

    public void UpdateHeights(ChoiceData data)
    {
        UpdateHeight(_textCentralButtonChoice2, _centralButton2Transform);
        UpdateHeight(_textButtonChoice1, _button1Transform);
        float y = _button1Transform.sizeDelta.y + _choicePanelUI.OffsetBetweenPanels;
        SetPosPanel(_button1Transform, y);
        if (data.ShowChoice3)
        {
            UpdateHeight(_textButtonChoice3, _button3Transform);
            y = _centralButton2Transform.sizeDelta.y + _choicePanelUI.OffsetBetweenPanels;
            SetPosPanel(_button3Transform, -y);
        }
    }

    private void UpdateHeight(TextMeshProUGUI textButtonChoice, RectTransform buttonTransform)
    {
        textButtonChoice.ForceMeshUpdate();
        Size = textButtonChoice.GetRenderedValues(true);
        Size.x = buttonTransform.sizeDelta.x;
        Size.y = Size.y + _choicePanelUI.HeightOffset * Multiplier;
        if (Size.y <= _choicePanelUI.ImageHeightDefault)
        {
            Size.y = _choicePanelUI.ImageHeightDefault;
        }
        buttonTransform.sizeDelta = Size;
    }

    private void SetPosPanel(RectTransform rectTransform, float y)
    {
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, y);
    }
}