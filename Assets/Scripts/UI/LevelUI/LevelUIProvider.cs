using UniRx;

public class LevelUIProvider
{
    public readonly NarrativePanelUIHandler NarrativePanelUIHandler;
    public readonly NotificationPanelUIHandler NotificationPanelUIHandler;
    public readonly CharacterPanelUIHandler CharacterPanelUIHandler;
    public readonly CurtainUIHandler CurtainUIHandler;
    public readonly CustomizationCurtainUIHandler CustomizationCurtainUIHandler;
    public readonly ChoicePanelUIHandler ChoicePanelUIHandler;
    public readonly ButtonSwitchSlideUIHandler ButtonSwitchSlideUIHandler;
    public readonly CustomizationCharacterPanelUIHandler CustomizationCharacterPanelUIHandler;
    public readonly HeaderSeriesPanelHandlerUI HeaderSeriesPanelHandlerUI;
    public readonly GameControlPanelUIHandler GameControlPanelUIHandler;
    
    public LevelUIProvider(LevelUIView levelUIView, Wallet wallet, ReactiveCommand onSceneTransition,
        DisableNodesContentEvent disableNodesContentEvent, SwitchToNextNodeEvent switchToNextNodeEvent,
        CustomizationCharacterPanelUI customizationCharacterPanelUI)
    {
        levelUIView.gameObject.SetActive(true);
        NarrativePanelUI narrativePanelUI = levelUIView.NarrativePanelUI;
        NotificationPanelUI notificationPanelUI = levelUIView.NotificationPanelUI;
        CharacterPanelUI characterPanelUI = levelUIView.CharacterPanelUI;
        CurtainUI curtainUI = levelUIView.CurtainUI;
        ChoicePanelUI choicePanelUI = levelUIView.ChoicePanelUI;
        ButtonSwitchSlideUI buttonSwitchSlideUI = levelUIView.ButtonSwitchSlideUI;

        
        HeaderSeriesPanelUI headerSeriesPanelUI = levelUIView.HeaderSeriesPanelUI;
        NarrativePanelUIHandler = new NarrativePanelUIHandler(narrativePanelUI);
        NotificationPanelUIHandler = new NotificationPanelUIHandler(notificationPanelUI);
        CharacterPanelUIHandler = new CharacterPanelUIHandler(characterPanelUI);
        CurtainUIHandler = new CurtainUIHandler(curtainUI);
        CustomizationCurtainUIHandler = new CustomizationCurtainUIHandler(curtainUI);
        ChoicePanelUIHandler = new ChoicePanelUIHandler(choicePanelUI, wallet);
        ButtonSwitchSlideUIHandler = new ButtonSwitchSlideUIHandler(buttonSwitchSlideUI, switchToNextNodeEvent);
        CustomizationCharacterPanelUIHandler = new CustomizationCharacterPanelUIHandler(customizationCharacterPanelUI);
        HeaderSeriesPanelHandlerUI = new HeaderSeriesPanelHandlerUI(headerSeriesPanelUI);
        GameControlPanelUIHandler = new GameControlPanelUIHandler(levelUIView.GameControlPanelView, onSceneTransition);
        
        disableNodesContentEvent.Subscribe(() =>
        {
            narrativePanelUI.gameObject.SetActive(false);
            notificationPanelUI.gameObject.SetActive(false);
            characterPanelUI.gameObject.SetActive(false);
            choicePanelUI.gameObject.SetActive(false);
            customizationCharacterPanelUI.gameObject.SetActive(false);
            headerSeriesPanelUI.gameObject.SetActive(false);
            levelUIView.MonetPanel.gameObject.SetActive(false);
        });
    }

    public void Dispose()
    {
        GameControlPanelUIHandler.Dispose();
    }
}