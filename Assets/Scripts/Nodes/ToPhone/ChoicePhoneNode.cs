using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class ChoicePhoneNode : ChoiceNode
{
    private CustomizationCurtainUIHandler _curtainUIHandler;
    private int _resultCaseIndex;
    public bool IsOver { get; private set; }
    
    public void ConstructMyChoicePhoneNode(IGameStatsProvider gameStatsProvider, ChoicePanelUIHandler choicePanelUIHandler,
        NotificationPanelUIHandler notificationPanelUIHandler, CustomizationCurtainUIHandler curtainUIHandler,
        int seriaIndex)
    {
        IsOver = false;
        ChoiceResultEvent = new ChoiceResultEvent<ChoiceCase>();
        ChoiceNodeInitializer = new ChoiceNodeInitializer(gameStatsProvider.GetEmptyStatsFromCurrentSeria(seriaIndex));
        NotificationPanelUIHandler = notificationPanelUIHandler;
        ChoicePanelUIHandler = choicePanelUIHandler;
        GameStatsProvider = gameStatsProvider;
        _curtainUIHandler = curtainUIHandler;
        for (int i = 0; i < _choiceCases.Count; i++)
        {
            _choiceCases[i].InitLocalizationString();
        }
        if (IsPlayMode() == false)
        {
            ChoiceNodeInitializer.TryInitReInitStatsInCases(_choiceCases);
        }
    }

    public PhoneMessage GetMessage()
    {
        return new PhoneMessage(){
            TextMessage = _choiceCases[_resultCaseIndex].GetLocalizationString(),
            MessageType = PhoneMessageType.Outcoming
        };
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
        
        _curtainUIHandler.CurtainImage.raycastTarget = true;
        _curtainUIHandler.Transform.gameObject.SetActive(true);
        await UniTask.WhenAll(
            _curtainUIHandler.CurtainImage.DOFade(PhoneAnimValues.FadeEndValue, PhoneAnimValues.Duration).WithCancellation(CancellationTokenSource.Token),
            ChoicePanelUIHandler.ShowChoiceVariantsInPlayMode(CancellationTokenSource.Token, ChoiceData, ChoiceResultEvent));
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
        IsOver = true;
        await UniTask.WhenAll(
            _curtainUIHandler.CurtainImage.DOFade(PhoneAnimValues.UnfadeEndValue, PhoneAnimValues.Duration).WithCancellation(CancellationTokenSource.Token),
            ChoicePanelUIHandler.DisappearanceChoiceVariantsInPlayMode(CancellationTokenSource.Token));
        _curtainUIHandler.CurtainImage.raycastTarget = false;
        _curtainUIHandler.Transform.gameObject.SetActive(false);
        CompositeDisposable.Dispose();
        ChoiceData = null;
    }
    protected override void SetInfoToView()
    {
        ChoicePanelUIHandler.ShowChoiceVariants(CreateChoice());
    }

    private void SetNextNodeFromResultChoice(ChoiceCase choiceCaseResult)
    {
        ChoiceResultEvent.Dispose();
        ShowNotification(choiceCaseResult.BaseStatsChoice);
        GetNextNode();
        
        if (_showOutput == true)
        {
            TryFindDefaultNextNodeAndSet();
        }
        else
        {
            _resultCaseIndex = _choiceCases.IndexOf(choiceCaseResult);
            var nodePort = GetOutputPort(GetPortName(_resultCaseIndex));
            
            SetNextNode(nodePort.Connection.node as BaseNode);
        }
        GameStatsProvider.GameStatsHandler.UpdateStats(choiceCaseResult.BaseStatsChoiceIReadOnly);
        Exit().Forget();
    }
}