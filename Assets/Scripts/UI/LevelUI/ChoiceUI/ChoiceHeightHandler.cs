using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChoiceHeightHandler : PanelUIHandler
{
    private const float _multiplier = 0.5f;
    private const float _defaultPosY = 0f;
    private readonly IReadOnlyList<ChoiceCaseView> _choiseCasesViews;
    private readonly ChoicePanelUI _choicePanelUI;
    public ChoiceHeightHandler(IReadOnlyList<ChoiceCaseView> choiseCasesViews, ChoicePanelUI choicePanelUI)
    {
        _choiseCasesViews = choiseCasesViews;
        _choicePanelUI = choicePanelUI;
    }

    public void UpdateHeights(ChoiceData data)
    {
        _choicePanelUI.ChoicesParent.anchoredPosition = Vector2.zero;
        ChoiceCaseView choiceCaseView = null;
        float allSize = 0f;
        for (int i = 0; i < data.ButtonsCount; i++)
        {            
            choiceCaseView = _choiseCasesViews[i];
            UpdateHeight(choiceCaseView.TextButtonChoice, choiceCaseView.RectTransformChoice);

            if (i > 0)
            {
                choiceCaseView = _choiseCasesViews[i - 1];
                SetPosPanel(_choiseCasesViews[i].RectTransformChoice,
                    -(Mathf.Abs(choiceCaseView.RectTransformChoice.anchoredPosition.y) +
                      choiceCaseView.RectTransformChoice.sizeDelta.y));
            }
            else
            {
                SetPosPanel(choiceCaseView.RectTransformChoice, _defaultPosY);
            }
            allSize += choiceCaseView.RectTransformChoice.sizeDelta.y;

        }
        SetPosPanel(_choicePanelUI.ChoicesParent, allSize * _multiplier);
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