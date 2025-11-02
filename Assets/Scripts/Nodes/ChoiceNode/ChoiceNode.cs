using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;
using UniRx;
using XNode;

[NodeTint("#7B0800"), NodeWidth(350)]
public class ChoiceNode : BaseNode, ILocalizable
{
    [SerializeField] private LocalizationString _localizationChoiceText1;
    [SerializeField] private LocalizationString _localizationChoiceText2;
    [SerializeField] private LocalizationString _localizationChoiceText3;

    [SerializeField, HideInInspector] private int _timerValue;
    [SerializeField, HideInInspector] private int _timerPortIndex;
    [SerializeField, HideInInspector] private float _choice1Price;
    [SerializeField, HideInInspector] private float _choice2Price;
    [SerializeField, HideInInspector] private float _choice3Price;
    [SerializeField, HideInInspector] private float _choice1AdditionaryPrice;
    [SerializeField, HideInInspector] private float _choice2AdditionaryPrice;
    [SerializeField, HideInInspector] private float _choice3AdditionaryPrice;
    [SerializeField, HideInInspector] private bool _showStatsChoice1Key;
    [SerializeField, HideInInspector] private bool _showStatsChoice2Key;
    [SerializeField, HideInInspector] private bool _showStatsChoice3Key;
    [SerializeField, HideInInspector] private bool _showChoice3Key;
    [SerializeField, HideInInspector] private bool _showOutput;
    [SerializeField, HideInInspector] private bool _addTimer;
    [SerializeField, HideInInspector] private bool _showNotificationChoice1 = false;
    [SerializeField, HideInInspector] private bool _showNotificationChoice2 = false;
    [SerializeField, HideInInspector] private bool _showNotificationChoice3 = false;
    [SerializeField, HideInInspector] private List<BaseStat> _baseStatsChoice1;
    [SerializeField, HideInInspector] private List<BaseStat> _baseStatsChoice2;
    [SerializeField, HideInInspector] private List<BaseStat> _baseStatsChoice3;

    [SerializeField, HideInInspector, Output] private Empty Choice1Output;
    [SerializeField, HideInInspector, Output] private Empty Choice2Output;
    [SerializeField, HideInInspector, Output] private Empty Choice3Output;

    private const string _port1 = "Choice1Output";
    private const string _port2 = "Choice2Output";
    private const string _port3 = "Choice3Output";
    // private const char _plus = '+';
    // private const char _space = ' ';
    // private const string _spaceColorPart1 = "<color=#";
    // private const string _spaceColorPart2 = ">";
    // private const string _endSpaceColor = "</color>";
    private const int _defaultTimerValue = 0;
    private IGameStatsProvider _gameStatsProvider;
    private ChoiceResultEvent<int> _choiceResultEvent;
    private ChoicePanelUIHandler _choicePanelUIHandler;
    private ChoiceNodeInitializer _choiceNodeInitializer;
    private SendCurrentNodeEvent<BaseNode> _sendCurrentNodeEvent;
    private CancellationTokenSource _timerCancellationTokenSource;
    private CompositeDisposable _compositeDisposable;
    private NotificationPanelUIHandler _notificationPanelUIHandler;
    private string[] _namesPortsPorts;
    private List<List<BaseStat>> _allStatsChoice;
    public IReadOnlyList<string> NamesPorts => _namesPortsPorts;
    public IReadOnlyList<ILocalizationString> BaseStatsChoice1Localizations => _baseStatsChoice1;
    public IReadOnlyList<ILocalizationString> BaseStatsChoice2Localizations => _baseStatsChoice2;
    public IReadOnlyList<ILocalizationString> BaseStatsChoice3Localizations => _baseStatsChoice3;
    
    public void ConstructMyChoiceNode(IGameStatsProvider gameStatsProvider, ChoicePanelUIHandler choicePanelUIHandler,
        SendCurrentNodeEvent<BaseNode> sendCurrentNodeEvent, NotificationPanelUIHandler notificationPanelUIHandler, int seriaIndex)
    {
        _namesPortsPorts = new[] {_port1, _port2, _port3};
        _allStatsChoice = new List<List<BaseStat>>(){_baseStatsChoice1, _baseStatsChoice2, _baseStatsChoice3};
        _choiceResultEvent = new ChoiceResultEvent<int>();
        _choiceNodeInitializer = new ChoiceNodeInitializer(gameStatsProvider.GetStatsFromCurrentSeria(seriaIndex));
        _notificationPanelUIHandler = notificationPanelUIHandler;
        _choicePanelUIHandler = choicePanelUIHandler;
        _sendCurrentNodeEvent = sendCurrentNodeEvent;
        _gameStatsProvider = gameStatsProvider;
        TryInitAllStats();
        if (IsPlayMode() == false)
        {
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
        var data = CreateChoiceTexts();
        _compositeDisposable = SetLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
        {
            _choicePanelUIHandler.SetTexts(data);
        });
        _choiceResultEvent.SubscribeWithCompositeDisposable(SetNextNodeFromResultChoice, _compositeDisposable);

        IsMerged = isMerged;
        if (IsMerged == false)
        {
            ButtonSwitchSlideUIHandler.ActivateSkipTransition(SkipEnterTransition);
        }
        await _choicePanelUIHandler.ShowChoiceVariantsInPlayMode(CancellationTokenSource.Token, data, _choiceResultEvent, _showChoice3Key);
        ButtonSwitchSlideUIHandler.DeactivatePushOption();
        _choicePanelUIHandler.ChoiceNodeButtonsHandler.TryActivateButtonsChoice(_choiceResultEvent, _showChoice3Key);
        _choicePanelUIHandler.ActivateTimerChoice(_choiceResultEvent, _timerPortIndex, _timerCancellationTokenSource.Token);
    }

    public override async UniTask Exit()
    {
        if (_timerValue > 0)
        {
            _timerCancellationTokenSource.Cancel();
        }
        CancellationTokenSource = new CancellationTokenSource();
        if (IsMerged == false)
        {
            ButtonSwitchSlideUIHandler.ActivateSkipTransition(SkipExitTransition);
        }
        await _choicePanelUIHandler.DisappearanceChoiceVariantsInPlayMode(CancellationTokenSource.Token, _showChoice3Key);
        _compositeDisposable.Dispose();
    }
    protected override void SetInfoToView()
    {
        _choicePanelUIHandler.ShowChoiceVariants(CreateChoiceTexts());
    }

    public override void SkipEnterTransition()
    {
        CancellationTokenSource.Cancel();
        SetInfoToView();
        _choicePanelUIHandler.ActivateTimerChoice(_choiceResultEvent, _timerPortIndex, _timerCancellationTokenSource.Token);
        _choicePanelUIHandler.ChoiceNodeButtonsHandler.TryActivateButtonsChoice(_choiceResultEvent, _showChoice3Key);
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[] {_localizationChoiceText1, _localizationChoiceText2, _localizationChoiceText3};
    }

    public override void SkipExitTransition()
    {
        CancellationTokenSource.Cancel();
        _choicePanelUIHandler.HideChoiceVariants();
        _sendCurrentNodeEvent.Execute(GetNextNode());
    }

    private void TryInitAllStats()
    {
        if (IsPlayMode() == false)
        {
            _choiceNodeInitializer.TryInitStats(ref _baseStatsChoice1);
            _choiceNodeInitializer.TryInitStats(ref _baseStatsChoice2);
            _choiceNodeInitializer.TryInitStats(ref _baseStatsChoice3);
        }
    }

    private ChoiceData CreateChoiceTexts()
    {
        if (_showChoice3Key == true)
        {
            return new ChoiceData(_localizationChoiceText1, _choice1Price, _choice1AdditionaryPrice,
                _localizationChoiceText2, _choice2Price, _choice2AdditionaryPrice,
                _localizationChoiceText3, _choice3Price, _choice3AdditionaryPrice, _addTimer == true ? _timerValue : 0);
        }
        else
        {
            return new ChoiceData(_localizationChoiceText1, _choice1Price, _choice1AdditionaryPrice,
                _localizationChoiceText2, _choice2Price, _choice2AdditionaryPrice,
                _addTimer == true ? _timerValue : _defaultTimerValue);
        }
    }

    private void SetNextNodeFromResultChoice(int buttonPressIndex)
    {
        _choiceResultEvent.Dispose();
        var stats = _allStatsChoice[buttonPressIndex];
        
        TryShowNotification(stats, buttonPressIndex);
        if (_showOutput == true)
        {
            TryFindConnectedPorts(OutputPortBaseNode);
        }
        else
        {
            TryFindConnectedPorts(GetOutputPort($"{_namesPortsPorts[buttonPressIndex]}"));
        }
        _gameStatsProvider.GameStatsHandler.UpdateStats(stats);
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

    private void TryShowNotification(List<BaseStat> stats, int buttonPressIndex)
    {
        switch (buttonPressIndex)
        {
            case 0:
                if (_showNotificationChoice1)
                {
                    ShowNotification(_notificationPanelUIHandler.GetTextStats(stats, _gameStatsProvider));
                }
                break;
            case 1:
                if (_showNotificationChoice2)
                {
                    ShowNotification(_notificationPanelUIHandler.GetTextStats(stats, _gameStatsProvider));
                }
                break;
            case 2:
                if (_showNotificationChoice3)
                {
                    ShowNotification(_notificationPanelUIHandler.GetTextStats(stats, _gameStatsProvider));
                }
                break;
        }

        void ShowNotification(string text)
        {
            if (string.IsNullOrWhiteSpace(text) == false)
            {
                CompositeDisposable compositeDisposable = SetLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
                {
                    _notificationPanelUIHandler.SetText(_notificationPanelUIHandler.GetTextStats(_allStatsChoice[buttonPressIndex], _gameStatsProvider));
                });
                _notificationPanelUIHandler.EmergenceNotificationPanelInPlayMode(text, CancellationTokenSource.Token, false, compositeDisposable).Forget();
            }
        }
    }
}