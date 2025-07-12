using System;
using TMPro;
using UnityEngine;

public class NarrativePanelUIHandler
{
    private const float _offsetValue = 45f;
    private const float _hideValue = 0f;
    private const float _unhideValue = 1f;
    private readonly Vector3 _unfadePosition;
    private readonly Vector3 _fadePosition;
    private readonly NarrativePanelUI _narrativePanelUI;
    private readonly RectTransform _rectTransform;
    private readonly TextMeshProUGUI _textComponent;
    private readonly AnimationPanel _animationPanel;
    private readonly TextConsistentlyViewer _textConsistentlyViewer;
    private readonly PanelHeightHandler _panelHeightHandler;
    public TextConsistentlyViewer TextConsistentlyViewer => _textConsistentlyViewer;
    public AnimationPanel AnimationPanel => _animationPanel;
    public NarrativePanelUIHandler(NarrativePanelUI narrativePanelUI)
    {
        _textConsistentlyViewer = new TextConsistentlyViewer(narrativePanelUI.TextComponent);
        _narrativePanelUI = narrativePanelUI;
        _rectTransform = _narrativePanelUI.RectTransform;
        _textComponent = _narrativePanelUI.TextComponent;
        _unfadePosition = _rectTransform.anchoredPosition;
        _fadePosition = new Vector3(_unfadePosition.x, _unfadePosition.y + _offsetValue, _unfadePosition.z);

        _animationPanel = new AnimationPanel(_rectTransform, _narrativePanelUI.CanvasGroup,
            _fadePosition, _unfadePosition, narrativePanelUI.DurationAnim);
        _panelHeightHandler = new PanelHeightHandler(narrativePanelUI.RectTransform);
    }
    public void NarrativeInEditMode(string text)
    {
        _narrativePanelUI.gameObject.SetActive(true);
        _rectTransform.anchoredPosition = _unfadePosition;
        SetText(text);
        _narrativePanelUI.CanvasGroup.alpha = _unhideValue;
        _panelHeightHandler.UpdateHeight(text);;
    }

    public void EmergenceNarrativePanelInPlayMode(string text)
    {
        _narrativePanelUI.gameObject.SetActive(true);
        _rectTransform.anchoredPosition = _fadePosition;
        _textComponent.text = String.Empty;
        _panelHeightHandler.UpdateHeight(text);;
    }

    public void SetText(string text)
    {
        _textComponent.text = text;
    }

    public void DisappearanceNarrativePanelInPlayMode()
    {
        _narrativePanelUI.gameObject.SetActive(false);
        _narrativePanelUI.CanvasGroup.alpha = _hideValue;
        _rectTransform.anchoredPosition = _fadePosition;
    }
}