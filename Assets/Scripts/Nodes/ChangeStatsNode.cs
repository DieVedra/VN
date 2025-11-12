using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

[NodeWidth(400),NodeTint("#FF5D00")]
public class ChangeStatsNode : BaseNode
{
    [SerializeField] private List<BaseStat> _stats;
    [SerializeField] private bool _showKey;
    private IGameStatsProvider _gameStatsProvider;
    private NotificationPanelUIHandler _notificationPanelUIHandler;
    public IReadOnlyList<ILocalizationString> BaseStatsLocalizations => _stats;

    public void ConstructMyChangeStatsNode(IGameStatsProvider gameStatsProvider, NotificationPanelUIHandler notificationPanelUIHandler,
        int seriaIndex)
    {
        _gameStatsProvider = gameStatsProvider;
        _notificationPanelUIHandler = notificationPanelUIHandler;
        if (IsPlayMode() == false)
        {
            List<BaseStat> newStats = gameStatsProvider.GetEmptyTStat<BaseStat>(seriaIndex);
            Dictionary<string, BaseStat> oldStatsDictionary = _stats.ToDictionary(x => x.NameKey);
            for (int i = 0; i < newStats.Count; i++)
            {
                if (oldStatsDictionary.TryGetValue(newStats[i].NameKey, out BaseStat stat))
                {
                    newStats[i] = stat;
                }
            }
            _stats = newStats;
        }
    }

    public async override UniTask Enter(bool isMerged = false)
    {
        _gameStatsProvider.GameStatsHandler.UpdateStats(_stats);
        ShowNotification(_notificationPanelUIHandler.GetTextStats(_stats, _gameStatsProvider));

        SwitchToNextNodeEvent.Execute();
    }
    private void ShowNotification(string text)
    {
        if (string.IsNullOrWhiteSpace(text) == false)
        {
            CompositeDisposable compositeDisposable = SetLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
            {
                _notificationPanelUIHandler.SetText(_notificationPanelUIHandler.GetTextStats(_stats, _gameStatsProvider));
            });
            _notificationPanelUIHandler.EmergenceNotificationPanelInPlayMode(text, CancellationTokenSource.Token, false, compositeDisposable).Forget();
        }
    }

    private void Awake()
    {
        if (_stats == null)
        {
            _stats = new List<BaseStat>();
        }
    }
}