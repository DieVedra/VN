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
    private const string _spaceColor = "<color=#00000000>";
    private const string _endSpaceColor = "</color>";
    private readonly Vector3 _unfadePosition;
    private readonly Vector3 _fadePosition;
    private readonly NarrativePanelUI _narrativePanelUI;
    private readonly RectTransform _rectTransform;
    private readonly TextMeshProUGUI _textComponent;
    private readonly AnimationPanel _animationPanel;
    private readonly TextConsistentlyViewer _textConsistentlyViewer;
    private readonly StringBuilder _stringBuilder;
    public TextConsistentlyViewer TextConsistentlyViewer => _textConsistentlyViewer;
    public AnimationPanel AnimationPanel => _animationPanel;
    public NarrativePanelUIHandler(NarrativePanelUI narrativePanelUI)
    {
        _textConsistentlyViewer = new TextConsistentlyViewer(narrativePanelUI.TextComponent);
        _narrativePanelUI = narrativePanelUI;
        _rectTransform = _narrativePanelUI.PanelRectTransform;
        _textComponent = _narrativePanelUI.TextComponent;
        _unfadePosition = _rectTransform.anchoredPosition;
        _fadePosition = new Vector3(_unfadePosition.x, _unfadePosition.y + _offsetValue, _unfadePosition.z);
        _stringBuilder = new StringBuilder();
        _animationPanel = new AnimationPanel(_rectTransform, _narrativePanelUI.CanvasGroup,
            _fadePosition, _unfadePosition, narrativePanelUI.DurationAnim);
    }

    public void Dispose()
    {
        TextConsistentlyViewer.TryStop();
    }
    public void NarrativeInEditMode(string text)
    {
        _narrativePanelUI.gameObject.SetActive(true);
        _rectTransform.anchoredPosition = _unfadePosition;
        _narrativePanelUI.CanvasGroup.alpha = _unhideValue;
        SetText(text);
    }

    public async UniTask EmergenceNarrativePanelInPlayMode(string text, CancellationToken token)
    {
        _narrativePanelUI.gameObject.SetActive(true);
        _rectTransform.anchoredPosition = _fadePosition;
        _textComponent.text = String.Empty;
        _stringBuilder.Clear();
        _stringBuilder.Append(_spaceColor);
        _stringBuilder.Append(text);
        _stringBuilder.Append(_endSpaceColor);
        SetText(_stringBuilder.ToString());
        await AnimationPanel.UnfadePanel(token);
        await TextConsistentlyViewer.SetTextConsistently(text, _narrativePanelUI.FirstLineOffset);
    }

    public void SetText(string text)
    {
        if (TextConsistentlyViewer.IsRun)
        {
            TextConsistentlyViewer.TryStop();
        }
        _textComponent.text = text;
        ResizePanel();
    }

    public async UniTask DisappearanceNarrativePanelInPlayMode(CancellationToken token)
    {
        await AnimationPanel.FadePanel(token);
        _narrativePanelUI.gameObject.SetActive(false);
        _narrativePanelUI.CanvasGroup.alpha = _hideValue;
        _rectTransform.anchoredPosition = _fadePosition;
    }
    private void ResizePanel()
    {
        _textComponent.text = TextConsistentlyViewer.AddStringOffset(_textComponent.text, _narrativePanelUI.FirstLineOffset);
        _textComponent.ForceMeshUpdate();
        Size = _textComponent.GetRenderedValues(true);
        Size.x = _narrativePanelUI.ImageRectTransform.sizeDelta.x;
        Size.y = Size.y + _narrativePanelUI.HeightOffset * _multiplier;
        _narrativePanelUI.ImageRectTransform.sizeDelta = Size;
    }
}