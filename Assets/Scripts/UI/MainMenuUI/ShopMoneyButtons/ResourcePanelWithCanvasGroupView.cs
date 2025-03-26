using UnityEngine;

public class ResourcePanelWithCanvasGroupView : ResourcePanelView
{
    [SerializeField] private CanvasGroup _canvasGroup;

    public CanvasGroup CanvasGroup => _canvasGroup;
}