using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class NarrativePanelUIHandler : PanelUIHandler
{
    private const float _multiplier = 2f;
    private const float _offsetValue = 45f;
    private const float _hideValue = 0f;
    private const float _unhideValue = 1f;
    public readonly RectTransform RectTransform;
    private readonly int _siblingIndex;
    private readonly Vector3 _unfadePosition;
    private readonly Vector3 _fadePosition;
    private readonly NarrativePanelUI _narrativePanelUI;
    private readonly TextMeshProUGUI _textComponent;
    private readonly AnimationPanel _animationPanel;
    private readonly TextConsistentlyViewer _textConsistentlyViewer;
    private readonly FirstLineCustomizer _firstLineCustomizer;
    public TextConsistentlyViewer TextConsistentlyViewer => _textConsistentlyViewer;
    public AnimationPanel AnimationPanel => _animationPanel;
    public NarrativePanelUIHandler(NarrativePanelUI narrativePanelUI)
    {
        _firstLineCustomizer = new FirstLineCustomizer(narrativePanelUI);
        _textConsistentlyViewer = new TextConsistentlyViewer(narrativePanelUI.TextComponent);
        _narrativePanelUI = narrativePanelUI;
        RectTransform = _narrativePanelUI.PanelRectTransform;
        _siblingIndex = RectTransform.GetSiblingIndex();
        _textComponent = _narrativePanelUI.TextComponent;
        _unfadePosition = RectTransform.anchoredPosition;
        _fadePosition = new Vector3(_unfadePosition.x, _unfadePosition.y + _offsetValue, _unfadePosition.z);
        _animationPanel = new AnimationPanel(RectTransform, _narrativePanelUI.CanvasGroup,
            _fadePosition, _unfadePosition, narrativePanelUI.DurationAnim);
    }

    public void Dispose()
    {
        TextConsistentlyViewer.TryStop();
    }

    public void SetSibling(int index)
    {
        RectTransform.SetSiblingIndex(index);
    }
    public void ResetSibling()
    {
        RectTransform.SetSiblingIndex(_siblingIndex);
    }
    public void NarrativeInEditMode(string text)
    {
        _narrativePanelUI.gameObject.SetActive(true);
        RectTransform.anchoredPosition = _unfadePosition;
        _narrativePanelUI.CanvasGroup.alpha = _unhideValue;
        SetText(text);
        if (_firstLineCustomizer.CheckChangeFirstLineLength(out string result, _textComponent, text))
        {
            SetText(result);
        }
        ResizePanel();
    }

    public async UniTask EmergenceNarrativePanelInPlayMode(string text, CancellationToken token)
    {
        _narrativePanelUI.gameObject.SetActive(true);
        RectTransform.anchoredPosition = _fadePosition;
        _textComponent.text = String.Empty;
        SetText(_firstLineCustomizer.GetGhostLine(text));
        string result;
        if (_firstLineCustomizer.CheckChangeFirstLineLength(out result, _textComponent, text))
        {
            SetText(_firstLineCustomizer.GetGhostLine(result));
        }
        else
        {
            result = text;
        }
        ResizePanel();
        await AnimationPanel.UnfadePanel(token);
        await TextConsistentlyViewer.SetTextConsistently(result);
    }

    public void SetText(string text)
    {
        if (TextConsistentlyViewer.IsRun)
        {
            TextConsistentlyViewer.TryStop();
        }
        _textComponent.text = text;
    }

    public async UniTask DisappearanceNarrativePanelInPlayMode(CancellationToken token)
    {
        await AnimationPanel.FadePanel(token);
        _narrativePanelUI.gameObject.SetActive(false);
        _narrativePanelUI.CanvasGroup.alpha = _hideValue;
        RectTransform.anchoredPosition = _fadePosition;
    }
    private void ResizePanel()
    {
        _textComponent.ForceMeshUpdate();
        Size = _textComponent.GetRenderedValues(true);
        Size.x = _narrativePanelUI.ImageRectTransform.sizeDelta.x;
        Size.y = Size.y + _narrativePanelUI.HeightOffset * _multiplier;
        _narrativePanelUI.ImageRectTransform.sizeDelta = Size;
    }
}