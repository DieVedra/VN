using System.Collections.Generic;
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
    [SerializeField, HideInInspector] private bool _showStatsChoice1Key;
    [SerializeField, HideInInspector] private bool _showStatsChoice2Key;
    [SerializeField, HideInInspector] private bool _showStatsChoice3Key;
    [SerializeField, HideInInspector] private bool _showChoice3Key;
    [SerializeField, HideInInspector] private bool _showOutput;
    [SerializeField, HideInInspector] private bool _addTimer;
    [SerializeField, HideInInspector] private List<BaseStat> _baseStatsChoice1;
    [SerializeField, HideInInspector] private List<BaseStat> _baseStatsChoice2;
    [SerializeField, HideInInspector] private List<BaseStat> _baseStatsChoice3;

    [SerializeField, HideInInspector, Output] private Empty Choice1Output;
    [SerializeField, HideInInspector, Output] private Empty Choice2Output;
    [SerializeField, HideInInspector, Output] private Empty Choice3Output;

    private const string _port1 = "Choice1Output";
    private const string _port2 = "Choice2Output";
    private const string _port3 = "Choice3Output";
    private ChoiceResultEvent<int> _choiceResultEvent;
    private ChoicePanelUIHandler _choicePanelUIHandler;
    private ChoiceNodeInitializer _choiceNodeInitializer;
    private SendCurrentNodeEvent<BaseNode> _sendCurrentNodeEvent;
    private CancellationTokenSource _timerCancellationTokenSource;
    private CompositeDisposable _compositeDisposable;
    private string[] _namesPortsPorts;
    private List<List<BaseStat>> _allStatsChoice;
    public IReadOnlyList<string> NamesPorts => _namesPortsPorts;
    public IReadOnlyList<ILocalizationString> BaseStatsChoice1Localizations => _baseStatsChoice1;
    public IReadOnlyList<ILocalizationString> BaseStatsChoice2Localizations => _baseStatsChoice2;
    public IReadOnlyList<ILocalizationString> BaseStatsChoice3Localizations => _baseStatsChoice3;
    
    public void ConstructMyChoiceNode(IGameStatsProvider gameStatsProvider, ChoicePanelUIHandler choicePanelUIHandler,
        SendCurrentNodeEvent<BaseNode> sendCurrentNodeEvent, int seriaIndex)
    {
        _namesPortsPorts = new[] {_port1, _port2, _port3};
        _allStatsChoice = new List<List<BaseStat>>(){_baseStatsChoice1, _baseStatsChoice2, _baseStatsChoice3};
        _choiceResultEvent = new ChoiceResultEvent<int>();
        _choiceNodeInitializer = new ChoiceNodeInitializer(gameStatsProvider.GetStatsFromCurrentSeria(seriaIndex));
        _choiceResultEvent.Subscribe(SetNextNodeFromResultChoice);
        _choicePanelUIHandler = choicePanelUIHandler;
        _sendCurrentNodeEvent = sendCurrentNodeEvent;
        TryInitAllStats();
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
        _compositeDisposable = SetLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
        {
            _choicePanelUIHandler.SetTexts(CreateChoiceTexts());
        });
        IsMerged = isMerged;
        if (IsMerged == false)
        {
            ButtonSwitchSlideUIHandler.ActivateSkipTransition(SkipEnterTransition);
        }
        await _choicePanelUIHandler.ShowChoiceVariantsInPlayMode(CancellationTokenSource.Token, CreateChoiceTexts());
        ButtonSwitchSlideUIHandler.DeactivatePushOption();
        _choicePanelUIHandler.ActivateTimerChoice(_choiceResultEvent, _timerPortIndex, _timerCancellationTokenSource.Token);
        _choicePanelUIHandler.ActivateButtonsChoice(_choiceResultEvent, _showChoice3Key);
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
        _choicePanelUIHandler.ActivateButtonsChoice(_choiceResultEvent, _showChoice3Key);
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
            return new ChoiceData(_localizationChoiceText1, _choice1Price,
                _localizationChoiceText2, _choice2Price,
                _localizationChoiceText3, _choice3Price,  _addTimer == true ? _timerValue : 0);
        }
        else
        {
            return new ChoiceData(_localizationChoiceText1, _choice1Price,
                _localizationChoiceText2, _choice2Price,
                _addTimer == true ? _timerValue : 0);
        }
    }

    private void SetNextNodeFromResultChoice(int buttonPressIndex)
    {
        _choiceResultEvent.Dispose();
        if (_showOutput == true)
        {
            TryFindConnectedPorts(OutputPortBaseNode);
        }
        else
        {
            TryFindConnectedPorts(GetOutputPort($"{_namesPortsPorts[buttonPressIndex]}"));
        }
        _choiceNodeInitializer.GameStatsHandler.UpdateStat(_allStatsChoice[buttonPressIndex]);
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
}