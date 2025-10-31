using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;

public class NotificationPanelUIHandler
{
    public const float ShowTime = 1.5f;
    private const float _multiplier = 2f;
    
    private const string _spaceColorPart1 = "<color=#";
    private const string _spaceColorPart2 = ">";
    private const string _endSpaceColor = "</color>";
    private const char _plus = '+';
    private const char _space = ' ';
    private const char _transfer = '\n';
    // private const int _defaultTimerValue = 0;
    
    public readonly Color ColorText = Color.white;
    private readonly float _offsetValue = 45f;
    private readonly float _heightOffset;
    private readonly RectTransform _imageRectTransform;
    private readonly StringBuilder _stringBuilder;
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
        _stringBuilder = new StringBuilder();
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
    public string GetTextStats(List<BaseStat> stats, IGameStatsProvider gameStatsProvider)
    {
        bool isFirstStat = true;
        _stringBuilder.Clear();
        foreach (var stat in stats)
        {
            if (isFirstStat)
            {
                isFirstStat = false;
            }
            else
            {
                _stringBuilder.Append(_transfer);
            }
            if (stat.NotificationKey)
            {
                var hexColor = GetColor(gameStatsProvider, stat.LocalizationName.Key);
                _stringBuilder.Append(_spaceColorPart1);
                _stringBuilder.Append(hexColor);
                _stringBuilder.Append(_spaceColorPart2);
                if (stat.Value > 0)
                {
                    _stringBuilder.Append(_plus);
                }
                _stringBuilder.Append(stat.Value.ToString());
                _stringBuilder.Append(_space);
                _stringBuilder.Append(stat.LocalizationName);
                _stringBuilder.Append(_endSpaceColor);
            }
            Debug.Log($"stat.NotificationKey {stat.NotificationKey}   {stat.LocalizationName}   {stat.Value}");

        }

        return _stringBuilder.ToString();
    }
    private string GetColor(IGameStatsProvider gameStatsProvider, string key)
    {
        foreach (var stat in gameStatsProvider.GameStatsHandler.Stats)
        {
            if (stat.LocalizationName.Key == key)
            {
                return ColorUtility.ToHtmlStringRGB(stat.ColorField);
            }
        }
        return ColorUtility.ToHtmlStringRGB(Color.white);
    }
}