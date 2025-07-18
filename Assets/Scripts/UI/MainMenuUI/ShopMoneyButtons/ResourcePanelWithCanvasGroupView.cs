using UnityEngine;

public class ResourcePanelWithCanvasGroupView : ResourcePanelView
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _rectTransform;

    public CanvasGroup CanvasGroup => _canvasGroup;
    public RectTransform RectTransform => _rectTransform;
}