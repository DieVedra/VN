
using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class NotificationPanelUIHandler
{
    private readonly float _offsetValue = 45f;
    private readonly AnimationPanel _panelAnimation;
    private readonly NotificationPanelUI _notificationPanelUI;
    private readonly TextMeshProUGUI _textComponent;
    private readonly Vector3 _unfadePosition;
    private readonly Vector3 _fadePosition;
    private readonly Color _defaultColorText;
    public AnimationPanel AnimationPanel => _panelAnimation;

    public NotificationPanelUIHandler(NotificationPanelUI notificationPanelUI)
    {
        _notificationPanelUI = notificationPanelUI;
        _textComponent = _notificationPanelUI.Text;
        _unfadePosition = _notificationPanelUI.RectTransform.anchoredPosition;
        _fadePosition = new Vector3(_unfadePosition.x, _unfadePosition.y + _offsetValue, _unfadePosition.z);
        _panelAnimation = new AnimationPanel(_notificationPanelUI.RectTransform, _notificationPanelUI.CanvasGroup,
            _fadePosition, _unfadePosition, notificationPanelUI.DurationAnim);
        _defaultColorText = _textComponent.color;
    }
    public void ShowNotificationInEditMode(string text)
    {
        _notificationPanelUI.gameObject.SetActive(true);
        _notificationPanelUI.RectTransform.anchoredPosition = _unfadePosition;
        _textComponent.text = text;
        _notificationPanelUI.CanvasGroup.alpha = 1f;
    } 
    public void EmergenceNotificationPanelInPlayMode()
    {
        _notificationPanelUI.gameObject.SetActive(true);
        _notificationPanelUI.CanvasGroup.alpha = 0f;
        _notificationPanelUI.RectTransform.anchoredPosition = _fadePosition;
        _textComponent.text = String.Empty;
    }

    public void DisappearanceNotificationPanelInPlayMode()
    {
        _notificationPanelUI.gameObject.SetActive(false);
        _notificationPanelUI.CanvasGroup.alpha = 0f;
        _notificationPanelUI.RectTransform.anchoredPosition = _fadePosition;
        SetColorText(_defaultColorText);
    }
    public void SetColorText(Color color)
    {
        _textComponent.color = color;
    }
}