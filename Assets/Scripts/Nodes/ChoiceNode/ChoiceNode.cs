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
    [SerializeField] private List<ChoiceCase> _choiceCases;
    
    // [SerializeField] private LocalizationString _localizationChoiceText1;
    // [SerializeField] private LocalizationString _localizationChoiceText2;
    // [SerializeField] private LocalizationString _localizationChoiceText3;
    // [SerializeField] private LocalizationString _localizationChoiceText4;

    [SerializeField] private int _timerValue;
    [SerializeField] private int _timerPortIndex;
    // [SerializeField] private float _choice1Price;
    // [SerializeField] private float _choice2Price;
    // [SerializeField] private float _choice3Price;
    // [SerializeField] private float _choice4Price;
    // [SerializeField] private float _choice1AdditionaryPrice;
    // [SerializeField] private float _choice2AdditionaryPrice;
    // [SerializeField] private float _choice3AdditionaryPrice;
    // [SerializeField] private float _choice4AdditionaryPrice;
    // [SerializeField] private bool _showStatsChoice1Key;
    // [SerializeField] private bool _showStatsChoice2Key;
    // [SerializeField] private bool _showStatsChoice3Key;
    // [SerializeField] private bool _showStatsChoice4Key;
    // [SerializeField] private bool _showChoice3Key;
    // [SerializeField] private bool _showChoice4Key;
    [SerializeField] private bool _showOutput;
    [SerializeField] private bool _addTimer;
    // [SerializeField] private bool _showNotificationChoice1 = false;
    // [SerializeField] private bool _showNotificationChoice2 = false;
    // [SerializeField] private bool _showNotificationChoice3 = false;
    // [SerializeField] private bool _showNotificationChoice4 = false;
    // [SerializeField] private List<BaseStat> _baseStatsChoice1;
    // [SerializeField] private List<BaseStat> _baseStatsChoice2;
    // [SerializeField] private List<BaseStat> _baseStatsChoice3;
    // [SerializeField] private List<BaseStat> _baseStatsChoice4;
    //
    // [SerializeField, HideInInspector, Output] private Empty Choice1Output;
    // [SerializeField, HideInInspector, Output] private Empty Choice2Output;
    // [SerializeField, HideInInspector, Output] private Empty Choice3Output;
    // [SerializeField, HideInInspector, Output] private Empty Choice4Output;
    
    public const int MaxCaseCount = 4;
    public const string PortNamePart1 = "_choice";
    public const string PortNamePart2 = "Output";

    private IGameStatsProvider _gameStatsProvider;
    private ChoiceResultEvent<ChoiceCase> _choiceResultEvent;
    private ChoicePanelUIHandler _choicePanelUIHandler;
    private ChoiceNodeInitializer _choiceNodeInitializer;
    private CancellationTokenSource _timerCancellationTokenSource;
    private CompositeDisposable _compositeDisposable;
    private NotificationPanelUIHandler _notificationPanelUIHandler;
    private ChoiceData _choiceData;
    
    public void ConstructMyChoiceNode(IGameStatsProvider gameStatsProvider, ChoicePanelUIHandler choicePanelUIHandler,
        NotificationPanelUIHandler notificationPanelUIHandler, int seriaIndex)
    {
        _choiceResultEvent = new ChoiceResultEvent<ChoiceCase>();
        _choiceNodeInitializer = new ChoiceNodeInitializer(gameStatsProvider.GetEmptyStatsFromCurrentSeria(seriaIndex));
        _notificationPanelUIHandler = notificationPanelUIHandler;
        _choicePanelUIHandler = choicePanelUIHandler;
        _gameStatsProvider = gameStatsProvider;
        for (int i = 0; i < _choiceCases.Count; i++)
        {
            _choiceCases[i].InitLocalizationString();
        }
        if (IsPlayMode() == false)
        {
            _choiceNodeInitializer.TryInitReInitStatsInCases(_choiceCases);
            DisableNodesContentEvent.Subscribe(() =>
            {
                _choicePanelUIHandler.HideChoiceVariants();
            });
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        _timerCancellationTokenSource?.Cancel();
    }

    public override async UniTask Enter(bool isMerged = false)
    {
        CancellationTokenSource = new CancellationTokenSource();
        _timerCancellationTokenSource = new CancellationTokenSource();
        _choiceData = CreateChoice();
        _compositeDisposable = SetLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
        {
            _choicePanelUIHandler.SetTexts(_choiceData);
        });
        _choiceResultEvent.SubscribeWithCompositeDisposable(SetNextNodeFromResultChoice, _compositeDisposable);

        IsMerged = isMerged;
        if (IsMerged == false)
        {
            ButtonSwitchSlideUIHandler.ActivateSkipTransition(SkipEnterTransition);
        }
        await _choicePanelUIHandler.ShowChoiceVariantsInPlayMode(CancellationTokenSource.Token, _choiceData, _choiceResultEvent);
        ButtonSwitchSlideUIHandler.DeactivatePushOption();
        _choicePanelUIHandler.ChoiceNodeButtonsHandler.TryActivateButtonsChoice(_choiceData, _choiceResultEvent);
        _choicePanelUIHandler.ActivateTimerChoice(_choiceResultEvent, _timerPortIndex, _choiceCases[_timerPortIndex], _timerCancellationTokenSource.Token);
    }

    public override async UniTask Exit()
    {
        if (_timerValue > 0)
        {
            _timerCancellationTokenSource.Cancel();
        }
        CancellationTokenSource = new CancellationTokenSource();
        await _choicePanelUIHandler.DisappearanceChoiceVariantsInPlayMode(CancellationTokenSource.Token);
        _compositeDisposable.Dispose();
        _choiceData = null;
    }
    protected override void SetInfoToView()
    {
        _choicePanelUIHandler.ShowChoiceVariants(CreateChoice());
    }

    public override void SkipEnterTransition()
    {
        CancellationTokenSource.Cancel();
        SetInfoToView();
        _choicePanelUIHandler.ActivateTimerChoice(_choiceResultEvent, _timerPortIndex, _choiceCases[_timerPortIndex], _timerCancellationTokenSource.Token);
        _choicePanelUIHandler.ChoiceNodeButtonsHandler.TryActivateButtonsChoice(_choiceData, _choiceResultEvent);
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

    private ChoiceData CreateChoice()
    {
        return new ChoiceData(_choiceCases, _addTimer == true ? _timerValue : 0);
    }
    
    private void SetNextNodeFromResultChoice(ChoiceCase choiceCaseResult)
    {
        _choiceResultEvent.Dispose();
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
        _gameStatsProvider.GameStatsHandler.UpdateStats(choiceCaseResult.BaseStatsChoiceIReadOnly);
        SwitchToNextNodeEvent.Execute();
    }
    private void TryFindConnectedPorts(NodePort outputPort)
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

    private void ShowNotification(IEnumerable<BaseStat> stats)
    {
        string text = _notificationPanelUIHandler.GetTextStats(stats, _gameStatsProvider);
        if (string.IsNullOrWhiteSpace(text) == false)
        {
            CompositeDisposable compositeDisposable = SetLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
            {
                _notificationPanelUIHandler.SetText(_notificationPanelUIHandler.GetTextStats(stats, _gameStatsProvider));
            });
            _notificationPanelUIHandler.EmergenceNotificationPanelInPlayMode(text, CancellationTokenSource.Token, false, compositeDisposable).Forget();
        }
    }

    private string GetPortName(int index)
    {
        return $"{PortNamePart1}{index}{PortNamePart2}";
    }
    private void AddCase()
    {
        if (DynamicOutputs.Count() < MaxCaseCount)
        {
            ChoiceCase choiceCase = new ChoiceCase(_choiceNodeInitializer.GetBaseStatsChoice());
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