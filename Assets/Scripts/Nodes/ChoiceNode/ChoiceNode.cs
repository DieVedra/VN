
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;
using UniRx;
using XNode;


[NodeTint("#7B0800"), NodeWidth(250)]
public class ChoiceNode : BaseNode
{
    [SerializeField, TextArea] private string _choiceText1;
    [SerializeField, TextArea] private string _choiceText2;
    [SerializeField, TextArea] private string _choiceText3;
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
    
    private ChoiceResultEvent _choiceResultEvent;
    private GameStatsCustodian _gameStatsCustodian;
    private ChoicePanelUIHandler _choicePanelUIHandler;
    private SendCurrentNodeEvent _sendCurrentNodeEvent;
    private CancellationTokenSource _timerCancellationTokenSource;
    private string[] _namesPortsPorts;
    private List<List<BaseStat>> _allStatsChoice;
    public IReadOnlyList<string> NamesPorts => _namesPortsPorts;
    public void ConstructMyChoiceNode(GameStatsCustodian gameStatsCustodian, ChoicePanelUIHandler choicePanelUIHandler,
        SendCurrentNodeEvent sendCurrentNodeEvent)
    {
        _namesPortsPorts = new[] {$"Choice1Output", $"Choice2Output", $"Choice3Output"};
        _allStatsChoice = new List<List<BaseStat>>(){_baseStatsChoice1, _baseStatsChoice2, _baseStatsChoice3};
        _choiceResultEvent = new ChoiceResultEvent();
        _choiceResultEvent.Subscribe(SetNextNodeFromResultChoice);
        _gameStatsCustodian = gameStatsCustodian;
        _choicePanelUIHandler = choicePanelUIHandler;
        _sendCurrentNodeEvent = sendCurrentNodeEvent;
        TryInitAllStats();
        _gameStatsCustodian.StatsChangedReactiveCommand.Subscribe(_ =>
        {
            TryInitAllStats();
        });
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

    public override void SkipExitTransition()
    {
        CancellationTokenSource.Cancel();
        _choicePanelUIHandler.HideChoiceVariants();
        _sendCurrentNodeEvent.Execute(GetNextNode());
    }
    private void TryInitAllStats()
    {
        TryInitStats(ref _baseStatsChoice1);
        TryInitStats(ref _baseStatsChoice2);
        TryInitStats(ref _baseStatsChoice3);
    }
    private void TryInitStats(ref List<BaseStat> oldBaseStatsChoice)
    {
        if (oldBaseStatsChoice == null || oldBaseStatsChoice.Count != _gameStatsCustodian.Count)
        {
            List<BaseStat> newBaseStats = _gameStatsCustodian.GetGameBaseStatsForm();
            if (oldBaseStatsChoice != null && oldBaseStatsChoice.Count > 0)
            {
                if (oldBaseStatsChoice.Count > newBaseStats.Count)
                {
                    for (int i = 0; i < newBaseStats.Count; i++)
                    {
                        ReInitStat(ref newBaseStats, i, oldBaseStatsChoice[i]);
                    }
                }
                else if (oldBaseStatsChoice.Count < newBaseStats.Count)
                {
                    for (int i = 0; i < oldBaseStatsChoice.Count; i++)
                    {
                        ReInitStat(ref newBaseStats, i, oldBaseStatsChoice[i]);
                    }
                }
            }
            oldBaseStatsChoice = newBaseStats;
        }
    }
    private ChoiceData CreateChoiceTexts()
    {
        if (_showChoice3Key == true)
        {
            return new ChoiceData(_choiceText1, _choice1Price,
                _choiceText2, _choice2Price,
                _choiceText3, _choice3Price,  _addTimer == true ? _timerValue : 0);
        }
        else
        {
            return new ChoiceData(_choiceText1, _choice1Price,_choiceText2, _choice2Price, _addTimer == true ? _timerValue : 0);
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
        _gameStatsCustodian.UpdateStat(_allStatsChoice[buttonPressIndex]);
        SwitchToNextNodeEvent.Execute();
    }
    private void ReInitStat(ref List<BaseStat> newStats, int index, BaseStat oldStat)
    {
        if (newStats[index].Name == oldStat.Name)
        {
            newStats[index] = new BaseStat(oldStat.Name, oldStat.Value);
        }
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