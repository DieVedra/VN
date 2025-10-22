using System.Collections.Generic;
using UnityEngine;

public class NodeGraphInitializer
{
    public readonly SendCurrentNodeEvent<BaseNode> SendCurrentNodeEvent;
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
        SendCurrentNodeEvent = new SendCurrentNodeEvent<BaseNode>();
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
        if (node is ChangeLookCustomCharacterNode changeLookCustomCharacterNode)
        {
            changeLookCustomCharacterNode.InitMyChangeLookCustomCharacterNode(_characterProvider.GetCustomizationCharacters(seriaIndex));
            return;
        }
        if (node is CharacterNode characterNode)
        {
            characterNode.ConstructMyCharacterNode(_characterProvider.GetCharacters(seriaIndex),
                _levelUIProvider.CharacterPanelUIHandler, _background, _characterViewer);
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
            if (customizationNode == null)
            {
                Debug.Log(1);
            }
            customizationNode.ConstructMyCustomizationNode(
                _levelUIProvider.CustomizationCharacterPanelUIHandler,
                _levelUIProvider.CustomizationCurtainUIHandler,
                _characterProvider.GetCustomizationCharacters(seriaIndex),
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
            switchToNextSeriaNode.Construct(SwitchToNextSeriaEvent);
            return;
        }
        if (node is ShowArtNode showImageNode)
        {
            showImageNode.Construct(_background);
            return;
        }

        if (node is AddContactToPhoneNode addContactToPhoneNode)
        {
            addContactToPhoneNode.ConstructMyAddContactToPhoneNode(_phoneProvider.GetPhones(seriaIndex),
                _phoneProvider.GetContactsAddToPhone(seriaIndex), _levelUIProvider.NotificationPanelUIHandler, seriaIndex);
            return;
        }
        if (node is PhoneNode phoneNode)
        {
            phoneNode.ConstructMyPhoneNode(_phoneProvider.GetPhones(seriaIndex), _phoneProvider.GetContactsAddToPhone(seriaIndex),
                _levelUIProvider.PhoneUIHandler,
                _levelUIProvider.CustomizationCurtainUIHandler);
        }
    }
}