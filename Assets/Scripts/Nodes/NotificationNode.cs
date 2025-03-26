using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[NodeTint("#000D7B")]
public class NotificationNode : BaseNode
{
    [SerializeField, TextArea] private string _text;
    [SerializeField] private float _showTime = 1.5f;
    [SerializeField] private float _delayDisplayTime = 0f;
    [SerializeField] private Color _color = Color.white;
    private NotificationPanelUIHandler _notificationPanelUIHandler;

    public void ConstructMyNotificationNode(NotificationPanelUIHandler notificationPanelUIHandler)
    {
        _notificationPanelUIHandler = notificationPanelUIHandler;
    }
    public override async UniTask Enter(bool isMerged = false)
    {
        Show().Forget();
    }

    protected override void SetInfoToView()
    {
        _notificationPanelUIHandler.ShowNotificationInEditMode(_text);
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