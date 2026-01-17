using System.Collections.Generic;
using UniRx;

public class NodeGraphInitializer
{
    public readonly SwitchToNextNodeEvent SwitchToNextNodeEvent;
    public readonly SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph> SwitchToAnotherNodeGraphEvent;
    public readonly DisableNodesContentEvent DisableNodesContentEvent;
    public readonly SwitchToNextSeriaEvent<bool> SwitchToNextSeriaEvent;
    public readonly SetLocalizationChangeEvent SetLocalizationChangeEvent;
    private readonly ReactiveProperty<bool> _phoneNodeIsActive;

    private readonly IGameStatsProvider _gameStatsProvider;
    private readonly IPhoneProvider _phoneProvider;
    private readonly IReadOnlyDictionary<string, CustomizableCharacterIndexesCustodian> _customizableCharacterIndexesCustodians;
    private readonly ICharacterProvider _characterProvider;
    private readonly Background _background;
    private readonly LevelUIProviderEditMode _levelUIProvider;
    private readonly CharacterViewer _characterViewer;
    private readonly WardrobeCharacterViewer _wardrobeCharacterViewer;
    private readonly Sound _sound;
    private readonly Wallet _wallet;
    private readonly TaskRunner _taskRunner;

    public NodeGraphInitializer(IReadOnlyDictionary<string, CustomizableCharacterIndexesCustodian> customizableCharacterIndexesCustodians,
        ICharacterProvider characterProvider, Background background, LevelUIProviderEditMode levelUIProvider, CharacterViewer characterViewer,
        WardrobeCharacterViewer wardrobeCharacterViewer, Sound sound, Wallet wallet, IGameStatsProvider gameStatsProvider, IPhoneProvider phoneProvider,
        SwitchToNextNodeEvent switchToNextNodeEvent, SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph> switchToAnotherNodeGraphEvent,
        DisableNodesContentEvent disableNodesContentEvent , SwitchToNextSeriaEvent<bool> switchToNextSeriaEvent,
        SetLocalizationChangeEvent setLocalizationChangeEvent, ReactiveProperty<bool> phoneNodeIsActive)
    {
        _customizableCharacterIndexesCustodians = customizableCharacterIndexesCustodians;
        _characterProvider = characterProvider;
        _background = background;
        _levelUIProvider = levelUIProvider;
        _characterViewer = characterViewer;
        _wardrobeCharacterViewer = wardrobeCharacterViewer;
        _sound = sound;
        _wallet = wallet;
        _gameStatsProvider = gameStatsProvider;
        _phoneProvider = phoneProvider;
        SwitchToNextNodeEvent = switchToNextNodeEvent;
        SwitchToAnotherNodeGraphEvent = switchToAnotherNodeGraphEvent;
        DisableNodesContentEvent = disableNodesContentEvent;
        SwitchToNextSeriaEvent = switchToNextSeriaEvent;
        SetLocalizationChangeEvent = setLocalizationChangeEvent;
        _phoneNodeIsActive = phoneNodeIsActive;
        _taskRunner = new TaskRunner();
    }

    public void Init(List<BaseNode> nodes, int seriaIndex)
    {
        int count = nodes.Count;
        for (int i = 0; i < count; i++)
        {
            InitOneNode(nodes[i], seriaIndex);
        }
    }

    public void InitOneNode(BaseNode node, int seriaIndex)
    {
        node.ConstructBaseNode(_levelUIProvider.ButtonSwitchSlideUIHandler, SwitchToNextNodeEvent, DisableNodesContentEvent, SetLocalizationChangeEvent);
        TryInit(node, seriaIndex);
    }
    private void TryInit(BaseNode node, int seriaIndex)
    {
        switch (node)
        {
            case ChangeLookCustomCharacterNode changeLookCustomCharacterNode:
                changeLookCustomCharacterNode.InitMyChangeLookCustomCharacterNode(
                    _characterProvider.GetCustomizationCharacters(seriaIndex));
                return;
            
            case CharacterNode characterNode:
                characterNode.ConstructMyCharacterNode(_characterProvider.GetCharacters(seriaIndex),
                    _levelUIProvider.CharacterPanelUIHandler, _background, _characterViewer);
                return;
            
            case PhoneNarrativeMessageNode phoneNarrativeMessageNode:
                phoneNarrativeMessageNode.ConstructMyPhoneNarrativeNode(_levelUIProvider.NarrativePanelUIHandler, _levelUIProvider.CustomizationCurtainUIHandler);
                return;
            
            case NarrativeNode narrativeNode:
                narrativeNode.ConstructMyNarrativeNode(_levelUIProvider.NarrativePanelUIHandler);
                return;
            
            case NotificationNode notificationNode:
                notificationNode.ConstructMyNotificationNode(_levelUIProvider.NotificationPanelUIHandler);
                return;
            
            case SmoothTransitionNode smoothTransitionNode:
                smoothTransitionNode.ConstructMySmoothTransitionNode(_levelUIProvider.CurtainUIHandler);
                return;
            
            case BackgroundNode backgroundNode:
                backgroundNode.ConstructBackgroundNode(_background);
                return;
            
            case SwitchToAnotherNodeGraphNode switchToAnotherNodeGraphNode:
                switchToAnotherNodeGraphNode.ConstructSwitchToAnotherNodeGraphNode(SwitchToAnotherNodeGraphEvent);
                return;
            
            case MergerNode mergerNode:
                mergerNode.ConstructMyMergerNode();
                return;
            
            case SoundNode soundNode:
                soundNode.ConstructMySoundNode(_taskRunner, _sound);
                return;
            
            case ChoicePhoneNode choicePhoneNode:
                choicePhoneNode.ConstructMyChoicePhoneNode(_gameStatsProvider, _levelUIProvider.ChoicePanelUIHandler,
                    _levelUIProvider.NotificationPanelUIHandler, _levelUIProvider.CustomizationCurtainUIHandler, seriaIndex);
                return;
            
            case ChoiceNode choiceNode:
                choiceNode.ConstructMyChoiceNode(_gameStatsProvider, _levelUIProvider.ChoicePanelUIHandler,
                    _levelUIProvider.NotificationPanelUIHandler, seriaIndex);
                return;
            
            case CustomizationNode customizationNode:
                customizationNode.ConstructMyCustomizationNode(
                    _levelUIProvider.CustomizationCharacterPanelUIHandler,
                    _levelUIProvider.CustomizationCurtainUIHandler,
                    _characterProvider.GetCustomizationCharacters(seriaIndex),
                    _background, _sound,
                    _gameStatsProvider,
                    _wallet, _wardrobeCharacterViewer, _levelUIProvider.NotificationPanelUIHandler, seriaIndex);
                return;
            
            case PhoneSwitchNode phoneSwitchNode:
                phoneSwitchNode.ConstructMyPhoneSwitchNode(_gameStatsProvider, seriaIndex);
                return;
            
            case SwitchNode switchNode:
                switchNode.ConstructMySwitchNode(_gameStatsProvider, seriaIndex);
                return;
            
            case AddSpriteNodeToBackground addSpriteNodeToBackground:
                addSpriteNodeToBackground.ConstructMyAddSpriteNode(_background);
                return;
            
            case HeaderNode handlerNode:
                
                handlerNode.Construct(_background, _levelUIProvider.HeaderSeriesPanelHandlerUI,
                    _levelUIProvider.CurtainUIHandler, _levelUIProvider.ButtonSwitchSlideUIHandler, _sound);
                return;
            
            case CharacterColorByBackgroundNode characterColorByBackgroundNode:
                characterColorByBackgroundNode.Construct(_characterViewer);
                return;
            
            case SwitchToNextSeriaNode switchToNextSeriaNode:
                switchToNextSeriaNode.Construct(SwitchToNextSeriaEvent);
                return;
            
            case ShowArtNode showImageNode:
                showImageNode.Construct(_background);
                return;
            
            case AddContactToPhoneNode addContactToPhoneNode:
                addContactToPhoneNode.ConstructMyAddContactToPhoneNode(_phoneProvider.GetPhones(seriaIndex),
                    _phoneProvider.GetContactsToAddInPhoneInPlot(seriaIndex), _levelUIProvider.NotificationPanelUIHandler, seriaIndex);
                return;
            
            case PhoneNode phoneNode:
                phoneNode.ConstructMyPhoneNode(_phoneProvider.GetPhones(seriaIndex),
                    _phoneProvider.GetContactsToAddInPhoneInPlot(seriaIndex),
                    _customizableCharacterIndexesCustodians,
                    _levelUIProvider.PhoneUIHandler,
                    _levelUIProvider.CustomizationCurtainUIHandler, _levelUIProvider.NarrativePanelUIHandler,
                    _levelUIProvider.ChoicePanelUIHandler, _phoneNodeIsActive);
                return;
            
            case ChangeStatsNode changeStatsNode:
                changeStatsNode.ConstructMyChangeStatsNode(_gameStatsProvider, _levelUIProvider.NotificationPanelUIHandler,
                    seriaIndex);
                return;
        }
    }
}