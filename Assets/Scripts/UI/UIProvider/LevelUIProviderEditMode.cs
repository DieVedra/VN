using System;
using System.Collections.Generic;
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
    public readonly PanelResourceVisionHandler PanelResourceVisionHandler;
    private CompositeDisposable _compositeDisposable;

    public LevelUIProviderEditMode(
        LevelUIView levelUIView, BlackFrameUIHandler blackFrameUIHandler, IReadOnlyList<ChoiceCaseView> choiceCasesViews,
        Wallet wallet, DisableNodesContentEvent disableNodesContentEvent, SwitchToNextNodeEvent switchToNextNodeEvent,
        CustomizationCharacterPanelUI customizationCharacterPanelUI, PhoneContentProvider phoneContentProvider,
        PanelResourceVisionHandler panelResourceVisionHandler, Action phoneInitOperation)
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

        PanelResourceVisionHandler = panelResourceVisionHandler;
        ChoicePanelUIHandler = new ChoicePanelUIHandler(choiceCasesViews, choicePanelUI, wallet, PanelResourceVisionHandler);
        ButtonSwitchSlideUIHandler = new ButtonSwitchSlideUIHandler(buttonSwitchSlideUI, switchToNextNodeEvent);
        CustomizationCharacterPanelUIHandler = new CustomizationCharacterPanelUIHandler(customizationCharacterPanelUI, PanelResourceVisionHandler);
        HeaderSeriesPanelHandlerUI = new HeaderSeriesPanelHandlerUI(headerSeriesPanelUI);
        CurtainUIHandler = new CurtainUIHandler(blackFrameUIHandler.BlackFrameView);
        CustomizationCurtainUIHandler = new CustomizationCurtainUIHandler(blackFrameUIHandler.BlackFrameView);

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
                // levelUIView.MonetPanelRectTransform.gameObject.SetActive(false);
                // levelUIView.HeartsPanelRectTransform.gameObject.SetActive(false);
                levelUIView.PhoneUIView.gameObject.SetActive(false);
            });
        }
        else
        {
            _compositeDisposable = new CompositeDisposable();
        }

        PhoneUIHandler =
            new PhoneUIHandler(phoneContentProvider, _compositeDisposable, phoneInitOperation);
    }

    public virtual void Shutdown()
    {
        NarrativePanelUIHandler.Dispose();
        _compositeDisposable?.Clear();
        PanelResourceVisionHandler.Shutdown();
    }
}