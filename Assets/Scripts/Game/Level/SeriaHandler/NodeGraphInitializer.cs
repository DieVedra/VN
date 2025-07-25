using System.Collections.Generic;
using UnityEngine;

public class NodeGraphInitializer
{
    public readonly SendCurrentNodeEvent<BaseNode> SendCurrentNodeEvent;
    public readonly SwitchToNextNodeEvent SwitchToNextNodeEvent;
    public readonly SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph> SwitchToAnotherNodeGraphEvent;
    
    private readonly Background _background;
    private readonly LevelUIProviderEditMode _levelUIProvider;
    private readonly CharacterViewer _characterViewer;
    private readonly WardrobeCharacterViewer _wardrobeCharacterViewer;
    private readonly Sound _sound;
    private readonly Wallet _wallet;
    private readonly IGameStatsProvider _gameStatsProvider;
    private readonly DisableNodesContentEvent _disableNodesContentEvent;
    private readonly SwitchToNextSeriaEvent<bool> _switchToNextSeriaEvent;
    private readonly SetLocalizationChangeEvent _setLocalizationChangeEvent;
    private readonly ICharacterProvider _characterProvider;
    private readonly List<BackgroundContent> _backgrounds;
    private List<Stat> _stats;
    public IReadOnlyList<CustomizableCharacter> CustomizableCharacters => _characterProvider.CustomizableCharacters;
    public IWardrobeSeriaDataProvider WardrobeSeriaDataProvider { get; }


    public NodeGraphInitializer(ICharacterProvider characterProvider, List<BackgroundContent> backgrounds, Background background, 
        LevelUIProviderEditMode levelUIProvider, CharacterViewer characterViewer, WardrobeCharacterViewer wardrobeCharacterViewer, 
        IWardrobeSeriaDataProvider wardrobeSeriaDataProvider,
        Sound sound, Wallet wallet, IGameStatsProvider gameStatsProvider,
        SwitchToNextNodeEvent switchToNextNodeEvent, SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph> switchToAnotherNodeGraphEvent,
        DisableNodesContentEvent disableNodesContentEvent , SwitchToNextSeriaEvent<bool> switchToNextSeriaEvent,
        SetLocalizationChangeEvent setLocalizationChangeEvent)
    {
        WardrobeSeriaDataProvider = wardrobeSeriaDataProvider;
        _characterProvider = characterProvider;
        _backgrounds = backgrounds;
        _background = background;
        _levelUIProvider = levelUIProvider;
        _characterViewer = characterViewer;
        _wardrobeCharacterViewer = wardrobeCharacterViewer;
        _sound = sound;
        _wallet = wallet;
        _gameStatsProvider = gameStatsProvider;
        SwitchToNextNodeEvent = switchToNextNodeEvent;
        SwitchToAnotherNodeGraphEvent = switchToAnotherNodeGraphEvent;
        _disableNodesContentEvent = disableNodesContentEvent;
        _switchToNextSeriaEvent = switchToNextSeriaEvent;
        _setLocalizationChangeEvent = setLocalizationChangeEvent;
        SendCurrentNodeEvent = new SendCurrentNodeEvent<BaseNode>();
    }

    public void Init(List<BaseNode> nodes, int seriaIndex)
    {
        foreach (var node in nodes)
        {
            InitOneNode(node, seriaIndex);
        }
    }

    public void InitOneNode(BaseNode node, int seriaIndex)
    {
        node.ConstructBaseNode(_levelUIProvider.ButtonSwitchSlideUIHandler, SwitchToNextNodeEvent, _disableNodesContentEvent, _setLocalizationChangeEvent);
        if (node is CharacterNode characterNode)
        {
            characterNode.ConstructMyCharacterNode(_characterProvider.GetCharacters(), _levelUIProvider.CharacterPanelUIHandler, _background, _characterViewer); 
            return;
        }
        if (node is NarrativeNode narrativeNode)
        {
            narrativeNode.ConstructMyNarrativeNode(_levelUIProvider.NarrativePanelUIHandler);
            return;
        }
        if (node is NotificationNode notificationNode)
        {
            notificationNode.ConstructMyNotificationNode(_levelUIProvider.NotificationPanelUIHandler);
            return;
        }
        if (node is SmoothTransitionNode smoothTransitionNode)
        {
            smoothTransitionNode.ConstructMySmoothTransitionNode(_levelUIProvider.CurtainUIHandler);
            return;
        }
        if (node is BackgroundNode backgroundNode)
        {
            backgroundNode.ConstructBackgroundNode(_backgrounds, _background);
            return;
        }
        if (node is SwitchToAnotherNodeGraphNode switchToAnotherNodeGraphNode)
        {
            switchToAnotherNodeGraphNode.ConstructSwitchToAnotherNodeGraphNode(SwitchToAnotherNodeGraphEvent);
            return;
        }
        if (node is MergerNode mergerNode)
        {
            mergerNode.ConstructMyMergerNode();
            return;
        }
        if (node is SoundNode soundNode)
        {
            soundNode.ConstructMySoundNode(_sound);
            return;
        }
        if (node is ChoiceNode choiceNode)
        {
            choiceNode.ConstructMyChoiceNode(_gameStatsProvider, _levelUIProvider.ChoicePanelUIHandler, SendCurrentNodeEvent, seriaIndex);
            return;
        }
        if (node is CustomizationNode customizationNode)
        {
            customizationNode.ConstructMyCustomizationNode(
                _levelUIProvider.CustomizationCharacterPanelUIHandler,
                _levelUIProvider.CustomizationCurtainUIHandler,
                _characterProvider.CustomizableCharacters,
                WardrobeSeriaDataProvider.GetWardrobeSeriaData(seriaIndex),
                _background, _sound,
                _gameStatsProvider,
                _wallet, _wardrobeCharacterViewer, seriaIndex);
                return;
        }
        if (node is SwitchNode switchNode)
        {
            switchNode.ConstructMySwitchNode(_gameStatsProvider, seriaIndex);
            return;
        }
        if (node is AddSpriteNodeToBackground addSpriteNodeToBackground)
        {
            addSpriteNodeToBackground.ConstructMyAddSpriteNode(_background);
            return;
        }
        if (node is HeaderNode handlerNode)
        {
            handlerNode.Construct(_backgrounds, _background, _levelUIProvider.HeaderSeriesPanelHandlerUI,
                _levelUIProvider.CurtainUIHandler, _levelUIProvider.ButtonSwitchSlideUIHandler, _sound);
            return;
        }
        if (node is CharacterColorByBackgroundNode characterColorByBackgroundNode)
        {
            characterColorByBackgroundNode.Construct(_characterViewer);
            return;
        }
        if (node is SwitchToNextSeriaNode switchToNextSeriaNode)
        {
            switchToNextSeriaNode.Construct(_switchToNextSeriaEvent);
            return;
        }
        if (node is ShowArtNode showImageNode)
        {
            showImageNode.Construct(_background);
        }
    }
}