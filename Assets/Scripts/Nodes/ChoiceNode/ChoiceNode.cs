using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;
using UniRx;
using UnityEditor;
using XNode;

[NodeTint("#7B0800"), NodeWidth(350)]
public class ChoiceNode : BaseNode, ILocalizable
{
    [SerializeField] protected List<ChoiceCase> _choiceCases;
    [SerializeField] protected int _timerValue;
    [SerializeField] protected int _timerPortIndex;
    [SerializeField] protected bool _showOutput;
    [SerializeField] private bool _addTimer;
    
    public const int MaxCaseCount = 4;
    public const string PortNamePart1 = "_choice";
    public const string PortNamePart2 = "Output";

    public ChoiceResultEvent<ChoiceCase> ChoiceResultEvent { get; protected set; }
    protected IGameStatsProvider GameStatsProvider;
    protected ChoicePanelUIHandler ChoicePanelUIHandler;
    protected ChoiceNodeInitializer ChoiceNodeInitializer;
    protected NotificationPanelUIHandler NotificationPanelUIHandler;
    protected CancellationTokenSource TimerCancellationTokenSource;
    protected CompositeDisposable CompositeDisposable;
    protected ChoiceData ChoiceData;
    
    public void ConstructMyChoiceNode(IGameStatsProvider gameStatsProvider, ChoicePanelUIHandler choicePanelUIHandler,
        NotificationPanelUIHandler notificationPanelUIHandler, int seriaIndex)
    {
        ChoiceResultEvent = new ChoiceResultEvent<ChoiceCase>();
        ChoiceNodeInitializer = new ChoiceNodeInitializer(gameStatsProvider.GetEmptyStatsFromCurrentSeria(seriaIndex));
        NotificationPanelUIHandler = notificationPanelUIHandler;
        ChoicePanelUIHandler = choicePanelUIHandler;
        GameStatsProvider = gameStatsProvider;
        if (IsPlayMode() == false)
        {
            ChoiceNodeInitializer.TryInitReInitStatsInCases(_choiceCases);
            DisableNodesContentEvent.Subscribe(() =>
            {
                ChoicePanelUIHandler.HideChoiceVariants();
            });
        }
    }

    public override void Shutdown()
    {
        base.Shutdown();
        TimerCancellationTokenSource?.Cancel();
    }

    public override async UniTask Enter(bool isMerged = false)
    {
        CancellationTokenSource = new CancellationTokenSource();
        TimerCancellationTokenSource = new CancellationTokenSource();
        ChoiceData = CreateChoice();
        CompositeDisposable = SetLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
        {
            ChoicePanelUIHandler.SetTexts(ChoiceData);
        });
        ChoiceResultEvent.SubscribeWithCompositeDisposable(SetNextNodeFromResultChoice, CompositeDisposable);

        IsMerged = isMerged;
        if (IsMerged == false)
        {
            ButtonSwitchSlideUIHandler.ActivateSkipTransition(SkipEnterTransition);
        }
        await ChoicePanelUIHandler.ShowChoiceVariantsInPlayMode(CancellationTokenSource.Token, ChoiceData, ChoiceResultEvent);
        ButtonSwitchSlideUIHandler.DeactivatePushOption();
        ChoicePanelUIHandler.ChoiceNodeButtonsHandler.TryActivateButtonsChoice(ChoiceData, ChoiceResultEvent);
        ChoicePanelUIHandler.ActivateTimerChoice(ChoiceResultEvent, _timerPortIndex, _choiceCases[_timerPortIndex], TimerCancellationTokenSource.Token);
    }

    public override async UniTask Exit()
    {
        if (_timerValue > 0)
        {
            TimerCancellationTokenSource.Cancel();
        }
        CancellationTokenSource = new CancellationTokenSource();
        await ChoicePanelUIHandler.DisappearanceChoiceVariantsInPlayMode(CancellationTokenSource.Token);
        CompositeDisposable.Dispose();
        ChoiceData = null;
    }

    protected override void SetInfoToView()
    {
        ChoicePanelUIHandler.ShowChoiceVariants(CreateChoice());
    }

    public override void SkipEnterTransition()
    {
        CancellationTokenSource.Cancel();
        SetInfoToView();
        ChoicePanelUIHandler.ActivateTimerChoice(ChoiceResultEvent, _timerPortIndex, _choiceCases[_timerPortIndex], TimerCancellationTokenSource.Token);
        ChoicePanelUIHandler.ChoiceNodeButtonsHandler.TryActivateButtonsChoice(ChoiceData, ChoiceResultEvent);
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        List<LocalizationString> strings = new List<LocalizationString>(_choiceCases.Count);
        for (int i = 0; i < _choiceCases.Count; i++)
        {
            strings.Add(_choiceCases[i].GetLocalizationString());
        }
        return strings;
    }

    public IReadOnlyList<ILocalizationString> GetStatsChoiceLocalizations(int index)
    {
        return _choiceCases[index].BaseStatsChoiceLocalizations;
    }

    protected ChoiceData CreateChoice()
    {
        return new ChoiceData(_choiceCases, _addTimer == true ? _timerValue : 0);
    }

    private void SetNextNodeFromResultChoice(ChoiceCase choiceCaseResult)
    {
        ChoiceResultEvent.Shutdown();
        ShowNotification(choiceCaseResult.BaseStatsChoice);
        if (_showOutput == true)
        {
            TryFindConnectedPorts(OutputPortBaseNode);
        }
        else
        {
            int portIndex = _choiceCases.IndexOf(choiceCaseResult);
            TryFindConnectedPorts(GetOutputPort(GetPortName(portIndex)));
        }
        GameStatsProvider.GameStatsHandler.UpdateStats(choiceCaseResult.BaseStatsChoiceIReadOnly);
        SwitchToNextNodeEvent.Execute();
    }

    protected void TryFindConnectedPorts(NodePort outputPort)
    {
        bool notificationNodeFinded = false;
        bool nextNodeFinded = false;
        for (int i = 0; i < outputPort.GetConnections().Count; i++)
        {
            if (notificationNodeFinded == false && outputPort.GetConnection(i).node is NotificationNode notificationNode)
            {
                notificationNode.Enter().Forget();
                notificationNodeFinded = true;
            }
            else if (nextNodeFinded == false)
            {
                SetNextNode(outputPort.GetConnection(i).node as BaseNode);
                nextNodeFinded = true;
            }
        }
    }

    protected void ShowNotification(IEnumerable<BaseStat> stats)
    {
        string text = NotificationPanelUIHandler.GetTextStats(stats, GameStatsProvider);
        if (string.IsNullOrWhiteSpace(text) == false)
        {
            CompositeDisposable compositeDisposable = SetLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
            {
                NotificationPanelUIHandler.SetText(NotificationPanelUIHandler.GetTextStats(stats, GameStatsProvider));
            });
            NotificationPanelUIHandler.EmergenceNotificationPanelInPlayMode(text, CancellationTokenSource.Token, false, compositeDisposable).Forget();
        }
    }

    protected string GetPortName(int index)
    {
        return $"{PortNamePart1}{index}{PortNamePart2}";
    }

    private void InitLocalizationStringInCase(int index, string text)
    {
        _choiceCases[index].InitLocalizationString(text);
    }
    private void AddCase()
    {
        if (DynamicOutputs.Count() < MaxCaseCount)
        {
            ChoiceCase choiceCase = new ChoiceCase(ChoiceNodeInitializer.GetBaseStatsChoice());
            _choiceCases.Add(choiceCase);
            AddDynamicOutput(typeof(Empty), ConnectionType.Override, fieldName: GetPortName(DynamicOutputs.Count()));
            EditorUtility.SetDirty(this);
        }
    }
    private void RemoveCase()
    {
        if (DynamicOutputs.Any())
        {
            _choiceCases.RemoveAt(_choiceCases.Count - 1);
            RemoveDynamicPort(GetPortName(DynamicOutputs.Count() - 1));
            EditorUtility.SetDirty(this);
        }
    }

    private void Awake()
    {
        if (_choiceCases == null)
        {
            _choiceCases = new List<ChoiceCase>(MaxCaseCount);
        }
    }
}