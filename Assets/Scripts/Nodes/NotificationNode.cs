﻿using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[NodeTint("#000D7B")]
public class NotificationNode : BaseNode, ILocalizable
{
    [SerializeField, TextArea] private string _text;
    [SerializeField] private LocalizationString _localizationText;
    [SerializeField] private float _showTime = 1.5f;
    [SerializeField] private float _delayDisplayTime = 0f;
    [SerializeField] private Color _color = Color.white;
    private NotificationPanelUIHandler _notificationPanelUIHandler;

    public void ConstructMyNotificationNode(NotificationPanelUIHandler notificationPanelUIHandler)
    {
        _notificationPanelUIHandler = notificationPanelUIHandler;
        _localizationText.SetText(_text);
    }
    public override async UniTask Enter(bool isMerged = false)
    {
        Show().Forget();
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[] {_localizationText};
    }

    protected override void SetInfoToView()
    {
        _notificationPanelUIHandler.ShowNotificationInEditMode(_localizationText);
        _notificationPanelUIHandler.SetColorText(_color);
    }

    private async UniTaskVoid Show()
    {
        CancellationTokenSource = new CancellationTokenSource();
        if (_delayDisplayTime > 0f)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_delayDisplayTime), cancellationToken: CancellationTokenSource.Token);
        }

        _notificationPanelUIHandler.EmergenceNotificationPanelInPlayMode();
        SetInfoToView();
        await _notificationPanelUIHandler.AnimationPanel.UnfadePanel(CancellationTokenSource.Token);
        await UniTask.Delay(TimeSpan.FromSeconds(_showTime), cancellationToken: CancellationTokenSource.Token);
        await _notificationPanelUIHandler.AnimationPanel.FadePanel(CancellationTokenSource.Token);
        _notificationPanelUIHandler.DisappearanceNotificationPanelInPlayMode();
    }
}