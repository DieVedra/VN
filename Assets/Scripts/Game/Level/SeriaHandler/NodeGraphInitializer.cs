using System.Collections.Generic;

public class NodeGraphInitializer
{
    public readonly SwitchToNextNodeEvent SwitchToNextNodeEvent;
    public readonly SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph> SwitchToAnotherNodeGraphEvent;
    public readonly DisableNodesContentEvent DisableNodesContentEvent;
    public readonly SwitchToNextSeriaEvent<bool> SwitchToNextSeriaEvent;
    public readonly SetLocalizationChangeEvent SetLocalizationChangeEvent;
    
    private readonly IGameStatsProvider _gameStatsProvider;
    private readonly IPhoneProvider _phoneProvider;
    private readonly List<BackgroundContent> _backgrounds;
    private readonly ICharacterProvider _characterProvider;
    private readonly Background _background;
    private readonly LevelUIProviderEditMode _levelUIProvider;
    private readonly CharacterViewer _characterViewer;
    private readonly WardrobeCharacterViewer _wardrobeCharacterViewer;
    private readonly Sound _sound;
    private readonly Wallet _wallet;

    public NodeGraphInitializer(List<BackgroundContent> backgrounds, ICharacterProvider characterProvider, Background background, 
        LevelUIProviderEditMode levelUIProvider, CharacterViewer characterViewer, WardrobeCharacterViewer wardrobeCharacterViewer,
        Sound sound, Wallet wallet, IGameStatsProvider gameStatsProvider, IPhoneProvider phoneProvider,
        SwitchToNextNodeEvent switchToNextNodeEvent, SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph> switchToAnotherNodeGraphEvent,
        DisableNodesContentEvent disableNodesContentEvent , SwitchToNextSeriaEvent<bool> switchToNextSeriaEvent,
        SetLocalizationChangeEvent setLocalizationChangeEvent)
    {
        _backgrounds = backgrounds;
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
                backgroundNode.ConstructBackgroundNode(_backgrounds, _background);
                return;
            
            case SwitchToAnotherNodeGraphNode switchToAnotherNodeGraphNode:
                switchToAnotherNodeGraphNode.ConstructSwitchToAnotherNodeGraphNode(SwitchToAnotherNodeGraphEvent);
                return;
            
            case MergerNode mergerNode:
                mergerNode.ConstructMyMergerNode();
                return;
            
            case SoundNode soundNode:
                soundNode.ConstructMySoundNode(_sound);
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
            
            case SwitchNode switchNode:
                switchNode.ConstructMySwitchNode(_gameStatsProvider, seriaIndex);
                return;
            
            case AddSpriteNodeToBackground addSpriteNodeToBackground:
                addSpriteNodeToBackground.ConstructMyAddSpriteNode(_background);
                return;
            
            case HeaderNode handlerNode:
                handlerNode.Construct(_backgrounds, _background, _levelUIProvider.HeaderSeriesPanelHandlerUI,
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
                    _levelUIProvider.PhoneUIHandler,
                    _levelUIProvider.CustomizationCurtainUIHandler, seriaIndex);
                return;
            
            case ChangeStatsNode changeStatsNode:
                changeStatsNode.ConstructMyChangeStatsNode(_gameStatsProvider, _levelUIProvider.NotificationPanelUIHandler,
                    seriaIndex);
                return;
        }
    }
}