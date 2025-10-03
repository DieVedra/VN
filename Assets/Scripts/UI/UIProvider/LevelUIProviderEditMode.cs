using UniRx;
using UnityEngine;

public class LevelUIProviderEditMode
{
    public readonly PhoneUIHandler PhoneUIHandler;
    public readonly NarrativePanelUIHandler NarrativePanelUIHandler;
    public readonly NotificationPanelUIHandler NotificationPanelUIHandler;
    public readonly CharacterPanelUIHandler CharacterPanelUIHandler;
    public readonly CurtainUIHandler CurtainUIHandler;
    public readonly CustomizationCurtainUIHandler CustomizationCurtainUIHandler;
    public readonly ChoicePanelUIHandler ChoicePanelUIHandler;
    public readonly ButtonSwitchSlideUIHandler ButtonSwitchSlideUIHandler;
    public readonly CustomizationCharacterPanelUIHandler CustomizationCharacterPanelUIHandler;
    public readonly HeaderSeriesPanelHandlerUI HeaderSeriesPanelHandlerUI;
    public readonly PanelResourceHandler PanelResourceHandler;
    private CompositeDisposable _compositeDisposable;

    public LevelUIProviderEditMode(
        LevelUIView levelUIView, BlackFrameUIHandler blackFrameUIHandler, 
        Wallet wallet, DisableNodesContentEvent disableNodesContentEvent, SwitchToNextNodeEvent switchToNextNodeEvent,
        CustomizationCharacterPanelUI customizationCharacterPanelUI, PoolsProvider poolsProvider)
    {
        levelUIView.gameObject.SetActive(true);
        NarrativePanelUI narrativePanelUI = levelUIView.NarrativePanelUI;
        NotificationPanelUI notificationPanelUI = levelUIView.NotificationPanelUI;
        CharacterPanelUI characterPanelUI = levelUIView.CharacterPanelUI;
        ChoicePanelUI choicePanelUI = levelUIView.ChoicePanelUI;
        ButtonSwitchSlideUI buttonSwitchSlideUI = levelUIView.ButtonSwitchSlideUI;
        HeaderSeriesPanelUI headerSeriesPanelUI = levelUIView.HeaderSeriesPanelUI;

        NarrativePanelUIHandler = new NarrativePanelUIHandler(narrativePanelUI);
        NotificationPanelUIHandler = new NotificationPanelUIHandler(notificationPanelUI);
        CharacterPanelUIHandler = new CharacterPanelUIHandler(characterPanelUI);

        PanelResourceHandler = new PanelResourceHandler(wallet, levelUIView.MonetPanel, levelUIView.HeartsPanel);
        ChoicePanelUIHandler = new ChoicePanelUIHandler(choicePanelUI, wallet, PanelResourceHandler);
        ButtonSwitchSlideUIHandler = new ButtonSwitchSlideUIHandler(buttonSwitchSlideUI, switchToNextNodeEvent);
        CustomizationCharacterPanelUIHandler = new CustomizationCharacterPanelUIHandler(customizationCharacterPanelUI, PanelResourceHandler);
        HeaderSeriesPanelHandlerUI = new HeaderSeriesPanelHandlerUI(headerSeriesPanelUI);
        CurtainUIHandler = new CurtainUIHandler(blackFrameUIHandler.BlackFrameView);
        CustomizationCurtainUIHandler = new CustomizationCurtainUIHandler(blackFrameUIHandler.BlackFrameView);

        PhoneUIHandler = new PhoneUIHandler(poolsProvider, NarrativePanelUIHandler, CustomizationCurtainUIHandler);
        if (levelUIView.PhoneUIView != null)
        {
            PhoneUIHandler.Init(levelUIView.PhoneUIView);
        }

        if (Application.isPlaying == false)
        {
            _compositeDisposable = disableNodesContentEvent.SubscribeWithCompositeDisposable(() =>
            {
                narrativePanelUI.gameObject.SetActive(false);
                notificationPanelUI.gameObject.SetActive(false);
                characterPanelUI.gameObject.SetActive(false);
                choicePanelUI.gameObject.SetActive(false);
                customizationCharacterPanelUI.gameObject.SetActive(false);
                headerSeriesPanelUI.gameObject.SetActive(false);
                levelUIView.MonetPanel.gameObject.SetActive(false);
                levelUIView.HeartsPanel.gameObject.SetActive(false);
            });
        }
    }

    public virtual void Dispose()
    {
        _compositeDisposable?.Clear();
    }
}