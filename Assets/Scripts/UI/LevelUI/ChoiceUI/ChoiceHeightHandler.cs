using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChoiceHeightHandler : PanelUIHandler
{
    private const int _centralButtonIndex = 1;
    private readonly IReadOnlyList<ChoiceCaseView> _choiseCasesViews;
    private readonly ChoicePanelUI _choicePanelUI;
    private Vector2 Size;
    public ChoiceHeightHandler(IReadOnlyList<ChoiceCaseView> choiseCasesViews, ChoicePanelUI choicePanelUI)
    {
        _choiseCasesViews = choiseCasesViews;
        _choicePanelUI = choicePanelUI;
    }

    public void UpdateHeights(ChoiceData data)
    {
        ChoiceCaseView choiceCaseView = _choiseCasesViews[_centralButtonIndex];
        UpdateHeight(choiceCaseView.TextButtonChoice, choiceCaseView.RectTransformChoice);
        SetPosPanel(choiceCaseView.RectTransformChoice, _choicePanelUI.DefaultPosYCentralButtonChoice2);
        float y = 0f;
        for (int i = 0; i < data.ButtonsCount; i++)
        {
            if (i == _centralButtonIndex)
            {
                continue;
            }
            choiceCaseView = _choiseCasesViews[i];
            UpdateHeight(choiceCaseView.TextButtonChoice, choiceCaseView.RectTransformChoice);
            if (i < _centralButtonIndex)
            {
                y = _choiseCasesViews[_centralButtonIndex].RectTransformChoice.anchoredPosition.y + 
                    _choiseCasesViews[i].RectTransformChoice.sizeDelta.y +
                    _choicePanelUI.OffsetBetweenPanels;
            }
            else if (i > _centralButtonIndex)
            {
                y = -(Mathf.Abs(_choiseCasesViews[i - 1].RectTransformChoice.anchoredPosition.y) +
                      _choiseCasesViews[i - 1].RectTransformChoice.sizeDelta.y + 
                      _choicePanelUI.OffsetBetweenPanels);
            }
            SetPosPanel(_choiseCasesViews[i].RectTransformChoice, y);
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