using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;

public class NotificationPanelUIHandler
{
    public const float ShowTime = 1.5f;
    private const float _multiplier = 2f;
    public readonly Color ColorText = Color.white;
    private readonly float _offsetValue = 45f;
    private readonly float _heightOffset;
    private readonly RectTransform _imageRectTransform;

    private readonly AnimationPanel _panelAnimation;
    private readonly NotificationPanelUI _notificationPanelUI;
    private readonly TextMeshProUGUI _textComponent;

    private readonly Vector3 _unfadePosition;
    private readonly Vector3 _fadePosition;
    private readonly Color _defaultColorText;
    public AnimationPanel AnimationPanel => _panelAnimation;

    public NotificationPanelUIHandler(NotificationPanelUI notificationPanelUI)
    {
        _imageRectTransform = notificationPanelUI.ImageRectTransform;
        _heightOffset = notificationPanelUI.HeightOffset;
        _notificationPanelUI = notificationPanelUI;
        _textComponent = _notificationPanelUI.Text;
        _unfadePosition = _notificationPanelUI.RectTransform.anchoredPosition;
        _fadePosition = new Vector3(_unfadePosition.x, _unfadePosition.y + _offsetValue, _unfadePosition.z);
        _panelAnimation = new AnimationPanel(_notificationPanelUI.RectTransform, _notificationPanelUI.CanvasGroup,
            _fadePosition, _unfadePosition, notificationPanelUI.DurationAnim);
        _defaultColorText = _textComponent.color;
    }
    public void ShowNotificationInEditMode(string text, NotificationNodeData notificationNodeData = null)
    {
        _notificationPanelUI.gameObject.SetActive(true);
        _notificationPanelUI.RectTransform.anchoredPosition = _unfadePosition;
        SetText(text);
        _notificationPanelUI.CanvasGroup.alpha = 1f;
        if (notificationNodeData != null)
        {
            _textComponent.color = notificationNodeData.Color;
        }
        else
        {
            _textComponent.color = ColorText;
        }
    } 
    public async UniTask EmergenceNotificationPanelInPlayMode(string text, CancellationToken token, CompositeDisposable compositeDisposable = null, NotificationNodeData notificationNodeData = null)
    {
        if (notificationNodeData != null)
        {
            await Delay(token, notificationNodeData.DelayDisplayTime);
        }
        _notificationPanelUI.gameObject.SetActive(true);
        _notificationPanelUI.CanvasGroup.alpha = 0f;
        _notificationPanelUI.RectTransform.anchoredPosition = _fadePosition;
        _textComponent.text = String.Empty;
        ShowNotificationInEditMode(text, notificationNodeData);
        await AnimationPanel.UnfadePanel(token);
        if (notificationNodeData == null)
        {
            await Delay(token, ShowTime);
        }
        else
        {
            await Delay(token, notificationNodeData.ShowTime);
        }
        await AnimationPanel.FadePanel(token);
        DisappearanceNotificationPanelInPlayMode();
        compositeDisposable?.Clear();
    }

    private async UniTask Delay(CancellationToken token, float delayTime)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delayTime), cancellationToken: token);
    }

    private void DisappearanceNotificationPanelInPlayMode()
    {
        _notificationPanelUI.gameObject.SetActive(false);
        _notificationPanelUI.CanvasGroup.alpha = 0f;
        _notificationPanelUI.RectTransform.anchoredPosition = _fadePosition;
        _textComponent.color = _defaultColorText;
    }

    public void SetText(string text)
    {
        _textComponent.text = text;
        ResizePanel();
    }
    private void ResizePanel()
    {
        _textComponent.ForceMeshUpdate();
        Vector2 size = _textComponent.GetRenderedValues(true);
        size.x = _imageRectTransform.sizeDelta.x;
        size.y = size.y + _heightOffset * _multiplier;
        _imageRectTransform.sizeDelta = size;
    }
}