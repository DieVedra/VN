using UnityEngine;

public class ResourcesPanelsPositionHandlerMainMenu
{
    private RectTransform _monetIndicatorPanel, _heartsIndicatorPanel;
    private AnimationCurve _heartPositionXWithAddButtonAnimationCurve, _heartPositionYWithAddButtonAnimationCurve;
    private ResourcePanelHandler _monetResourcePanelHandler, _heartsResourcePanelHandler;
    public void Init(ResourcePanelHandler monetResourcePanelHandler, ResourcePanelHandler heartsResourcePanelHandler,
        AnimationCurve heartPositionXWithAddButtonAnimationCurve, AnimationCurve heartPositionYWithAddButtonAnimationCurve)
    {
        _monetResourcePanelHandler = monetResourcePanelHandler;
        _heartsResourcePanelHandler = heartsResourcePanelHandler;
        _heartPositionXWithAddButtonAnimationCurve = heartPositionXWithAddButtonAnimationCurve;
        _heartPositionYWithAddButtonAnimationCurve = heartPositionYWithAddButtonAnimationCurve;
        _monetIndicatorPanel = monetResourcePanelHandler.PanelTransform;
        _heartsIndicatorPanel = heartsResourcePanelHandler.PanelTransform;
        _heartsResourcePanelHandler.OnResize += UpdateHeartsPanelPosition;
    }
    public void Shutdown()
    {
        _heartsResourcePanelHandler.OnResize -= UpdateHeartsPanelPosition;
    }

    private void UpdateHeartsPanelPosition(ResourcePanelMode mode, int heartsLineLength)
    {
        if (mode == ResourcePanelMode.WithAddButton)
        {
            _heartsIndicatorPanel.anchoredPosition = new Vector2(
                _heartPositionXWithAddButtonAnimationCurve.Evaluate(heartsLineLength),
                _heartPositionYWithAddButtonAnimationCurve.Evaluate(heartsLineLength));
        }
        else
        {
            _heartsIndicatorPanel.anchoredPosition = Vector2.zero;
        }
    }
}