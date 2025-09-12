using System;
using System.Collections.Generic;

public class NodeGraphInitializer : PostponementInitializer
{
    public readonly SendCurrentNodeEvent<BaseNode> SendCurrentNodeEvent;
    public readonly SwitchToNextNodeEvent SwitchToNextNodeEvent;
    public readonly SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph> SwitchToAnotherNodeGraphEvent;
    public readonly DisableNodesContentEvent DisableNodesContentEvent;
    public readonly SwitchToNextSeriaEvent<bool> SwitchToNextSeriaEvent;
    public readonly SetLocalizationChangeEvent SetLocalizationChangeEvent;

    
    private readonly IGameStatsProvider _gameStatsProvider;
    private readonly List<BackgroundContent> _backgrounds;

    public NodeGraphInitializer(List<BackgroundContent> backgrounds, Background background, 
        LevelUIProviderEditMode levelUIProvider, CharacterViewer characterViewer, WardrobeCharacterViewer wardrobeCharacterViewer,
        Sound sound, Wallet wallet, IGameStatsProvider gameStatsProvider,
        SwitchToNextNodeEvent switchToNextNodeEvent, SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph> switchToAnotherNodeGraphEvent,
        DisableNodesContentEvent disableNodesContentEvent , SwitchToNextSeriaEvent<bool> switchToNextSeriaEvent,
        SetLocalizationChangeEvent setLocalizationChangeEvent)
        :base(background, levelUIProvider, sound, gameStatsProvider, characterViewer, wallet, wardrobeCharacterViewer)
    {
        _backgrounds = backgrounds;
        _gameStatsProvider = gameStatsProvider;
        SwitchToNextNodeEvent = switchToNextNodeEvent;
        SwitchToAnotherNodeGraphEvent = switchToAnotherNodeGraphEvent;
        DisableNodesContentEvent = disableNodesContentEvent;
        SwitchToNextSeriaEvent = switchToNextSeriaEvent;
        SetLocalizationChangeEvent = setLocalizationChangeEvent;
        SendCurrentNodeEvent = new SendCurrentNodeEvent<BaseNode>();
    }

    public void Init(List<BaseNode> nodes, Func<IReadOnlyList<Character>> charactersForThisSeria, int seriaIndex)
    {
        CharacterNodeForPostponementInit = null;
        CustomizationNodeForPostponementInit = null;
        int count = nodes.Count;
        for (int i = 0; i < count; i++)
        {
            InitOneNode(nodes[i], seriaIndex);
        }
        TryPostponementInit(charactersForThisSeria, seriaIndex);
    }

    public void InitOneNode(BaseNode node, int seriaIndex)
    {
        node.ConstructBaseNode(LevelUIProvider.ButtonSwitchSlideUIHandler, SwitchToNextNodeEvent, DisableNodesContentEvent, SetLocalizationChangeEvent);
        if (node is ChangeLookCustomCharacterNode changeLookCustomCharacterNode)
        {
            
            return;
        }
        if (node is CharacterNode characterNode)
        {
            if (CharacterNodeForPostponementInit == null)
            {
                CharacterNodeForPostponementInit = new List<CharacterNode>();
            }
            CharacterNodeForPostponementInit.Add(characterNode);
            return;
        }
        if (node is NarrativeNode narrativeNode)
        {
            narrativeNode.ConstructMyNarrativeNode(LevelUIProvider.NarrativePanelUIHandler);
            return;
        }
        if (node is NotificationNode notificationNode)
        {
            notificationNode.ConstructMyNotificationNode(LevelUIProvider.NotificationPanelUIHandler);
            return;
        }
        if (node is SmoothTransitionNode smoothTransitionNode)
        {
            smoothTransitionNode.ConstructMySmoothTransitionNode(LevelUIProvider.CurtainUIHandler);
            return;
        }
        if (node is BackgroundNode backgroundNode)
        {
            backgroundNode.ConstructBackgroundNode(_backgrounds, Background);
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
            soundNode.ConstructMySoundNode(Sound);
            return;
        }
        if (node is ChoiceNode choiceNode)
        {
            choiceNode.ConstructMyChoiceNode(_gameStatsProvider, LevelUIProvider.ChoicePanelUIHandler, SendCurrentNodeEvent, seriaIndex);
            return;
        }
        if (node is CustomizationNode customizationNode)
        {
            if (CustomizationNodeForPostponementInit == null)
            {
                CustomizationNodeForPostponementInit = new List<CustomizationNode>();
            }
            CustomizationNodeForPostponementInit.Add(customizationNode);
            return;
        }
        if (node is SwitchNode switchNode)
        {
            switchNode.ConstructMySwitchNode(_gameStatsProvider, seriaIndex);
            return;
        }
        if (node is AddSpriteNodeToBackground addSpriteNodeToBackground)
        {
            addSpriteNodeToBackground.ConstructMyAddSpriteNode(Background);
            return;
        }
        if (node is HeaderNode handlerNode)
        {
            handlerNode.Construct(_backgrounds, Background, LevelUIProvider.HeaderSeriesPanelHandlerUI,
                LevelUIProvider.CurtainUIHandler, LevelUIProvider.ButtonSwitchSlideUIHandler, Sound);
            return;
        }
        if (node is CharacterColorByBackgroundNode characterColorByBackgroundNode)
        {
            characterColorByBackgroundNode.Construct(CharacterViewer);
            return;
        }
        if (node is SwitchToNextSeriaNode switchToNextSeriaNode)
        {
            switchToNextSeriaNode.Construct(SwitchToNextSeriaEvent);
            return;
        }
        if (node is ShowArtNode showImageNode)
        {
            showImageNode.Construct(Background);
        }
    }
}