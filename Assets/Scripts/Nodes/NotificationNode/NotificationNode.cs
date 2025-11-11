using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

[NodeTint("#000D7B")]
public class NotificationNode : BaseNode, ILocalizable
{
    [SerializeField, TextArea] private string _text;
    [SerializeField] private LocalizationString _localizationText;
    [SerializeField] private bool _overrideDataKey;
    [SerializeField] private bool _awaitKey;
    [SerializeField] private NotificationNodeData _notificationNodeData;
    private NotificationPanelUIHandler _notificationPanelUIHandler;
    private CompositeDisposable _compositeDisposable;

    public void ConstructMyNotificationNode(NotificationPanelUIHandler notificationPanelUIHandler)
    {
        _notificationPanelUIHandler = notificationPanelUIHandler;
        ResetNotificationNodeData().Forget();
    }
    public override async UniTask Enter(bool isMerged = false)
    {
        _compositeDisposable = SetLocalizationChangeEvent.SubscribeWithCompositeDisposable(
            () => { _notificationPanelUIHandler.SetText(_localizationText);});
        CancellationTokenSource = new CancellationTokenSource();
        if (_awaitKey)
        {
            await _notificationPanelUIHandler.EmergenceNotificationPanelInPlayMode(_localizationText, CancellationTokenSource.Token, false, _compositeDisposable, _notificationNodeData);
            SwitchToNextNodeEvent.Execute();
        }
        else
        {
            _notificationPanelUIHandler.EmergenceNotificationPanelInPlayMode(_localizationText, CancellationTokenSource.Token, false, _compositeDisposable, _notificationNodeData).Forget();
        }
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[] {_localizationText};
    }

    protected override void SetInfoToView()
    {
        _notificationPanelUIHandler.ShowNotificationInEditMode(_localizationText, _notificationNodeData);
    }

    private async UniTask ResetNotificationNodeData()
    {
        CancellationTokenSource = new CancellationTokenSource();
        await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime), cancellationToken: CancellationTokenSource.Token);
        _notificationNodeData.Reset();
    }
}