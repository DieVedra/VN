using System.Collections.Generic;

public class NodeGraphInitializer
{
    public readonly SendCurrentNodeEvent<BaseNode> SendCurrentNodeEvent;
    public readonly SwitchToNextNodeEvent SwitchToNextNodeEvent;
    public readonly SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph> SwitchToAnotherNodeGraphEvent;

    private readonly Background _background;
    private readonly LevelUIProvider _levelUIProvider;
    private readonly CharacterViewer _characterViewer;
    private readonly WardrobeCharacterViewer _wardrobeCharacterViewer;
    private readonly CustomizableCharacter _customizableCharacter;
    private readonly Sound _sound;
    private readonly GameStatsCustodian _gameStatsCustodian;
    private readonly Wallet _wallet;
    private readonly DisableNodesContentEvent _disableNodesContentEvent;
    private readonly SwitchToNextSeriaEvent<bool> _switchToNextSeriaEvent;
    private readonly List<BackgroundContent> _backgrounds;
    private readonly List<Character> _characters;


    public NodeGraphInitializer(List<Character> characters, List<BackgroundContent> backgrounds, Background background, 
        LevelUIProvider levelUIProvider, CharacterViewer characterViewer, WardrobeCharacterViewer wardrobeCharacterViewer, 
        CustomizableCharacter customizableCharacter,
        Sound sound, GameStatsCustodian gameStatsCustodian, Wallet wallet,
        SwitchToNextNodeEvent switchToNextNodeEvent, SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph> switchToAnotherNodeGraphEvent,
        DisableNodesContentEvent disableNodesContentEvent , SwitchToNextSeriaEvent<bool> switchToNextSeriaEvent)
    {
        _backgrounds = backgrounds;
        _background = background;
        _levelUIProvider = levelUIProvider;
        _characterViewer = characterViewer;
        _wardrobeCharacterViewer = wardrobeCharacterViewer;
        _customizableCharacter = customizableCharacter;
        _sound = sound;
        _gameStatsCustodian = gameStatsCustodian;
        _wallet = wallet;
        SwitchToNextNodeEvent = switchToNextNodeEvent;
        SwitchToAnotherNodeGraphEvent = switchToAnotherNodeGraphEvent;
        _disableNodesContentEvent = disableNodesContentEvent;
        _switchToNextSeriaEvent = switchToNextSeriaEvent;
        SendCurrentNodeEvent = new SendCurrentNodeEvent<BaseNode>();
        if (characters == null)
        {
            _characters = new List<Character>();
        }
        else
        {
            _characters = characters;
        }
    }

    public void Init(List<BaseNode> nodes)
    {
        foreach (var node in nodes)
        {
            node.ConstructBaseNode(_levelUIProvider.ButtonSwitchSlideUIHandler, SwitchToNextNodeEvent, _disableNodesContentEvent);

            if (node is CharacterNode characterNode)
            {
                characterNode.ConstructMyCharacterNode(_characters, _levelUIProvider.CharacterPanelUIHandler, _background, _characterViewer);
                continue;
            }

            if (node is NarrativeNode narrativeNode)
            {
                narrativeNode.ConstructMyNarrativeNode(_levelUIProvider.NarrativePanelUIHandler);
                continue;
            }

            if (node is NotificationNode notificationNode)
            {
                notificationNode.ConstructMyNotificationNode(_levelUIProvider.NotificationPanelUIHandler);
                continue;
            }
            
            if (node is SmoothTransitionNode smoothTransitionNode)
            {
                smoothTransitionNode.ConstructMySmoothTransitionNode(_levelUIProvider.CurtainUIHandler);
                continue;
            }
            
            if (node is BackgroundNode backgroundNode)
            {
                backgroundNode.ConstructBackgroundNode(_backgrounds, _background);
                continue;
            }

            if (node is SwitchToAnotherNodeGraphNode switchToAnotherNodeGraphNode)
            {
                switchToAnotherNodeGraphNode.ConstructSwitchToAnotherNodeGraphNode(SwitchToAnotherNodeGraphEvent);
                continue;
            }
            
            if (node is MergerNode mergerNode)
            {
                mergerNode.ConstructMyMergerNode();
                continue;
            }
            
            if (node is ChoiceNode choiceNode)
            {
                choiceNode.ConstructMyChoiceNode(_gameStatsCustodian, _levelUIProvider.ChoicePanelUIHandler, SendCurrentNodeEvent);
                continue;
            }
            if (node is SoundNode soundNode)
            {
                soundNode.ConstructMySoundNode(_sound);
                continue;
            }
            
            if (node is CustomizationNode customizationNode)
            {
                customizationNode.ConstructMyCustomizationNode(
                    _levelUIProvider.CustomizationCharacterPanelUIHandler,
                    _levelUIProvider.CustomizationCurtainUIHandler,
                    _customizableCharacter, _background, _sound, _gameStatsCustodian, _wallet, _wardrobeCharacterViewer);
                continue;
            }

            if (node is SwitchNode switchNode)
            {
                switchNode.ConstructMySwitchNode(_gameStatsCustodian);
                continue;
            }

            if (node is AddSpriteNodeToBackground addSpriteNodeToBackground)
            {
                addSpriteNodeToBackground.ConstructMyAddSpriteNode(_background.GetBackgroundContent, _background.AdditionalImagesToBackground);
                continue;
            }

            if (node is HeaderNode handlerNode)
            {
                handlerNode.Construct(_levelUIProvider.HeaderSeriesPanelHandlerUI, _levelUIProvider.CurtainUIHandler, _levelUIProvider.ButtonSwitchSlideUIHandler);
                continue;
            }

            if (node is CharacterColorByBackgroundNode characterColorByBackgroundNode)
            {
                characterColorByBackgroundNode.Construct(_characterViewer);
                continue;
            }
            if (node is SwitchToNextSeriaNode switchToNextSeriaNode)
            {
                switchToNextSeriaNode.Construct(_switchToNextSeriaEvent);
                continue;
            }
            
            if (node is ShowImageNode showImageNode)
            {
                showImageNode.Construct(_background);
            }
        }
        _disableNodesContentEvent.Execute();
    }
}