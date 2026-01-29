using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
public class LevelUIProviderBuildMode : LevelUIProviderEditMode, ILocalizable
{
    public readonly GameControlPanelUIHandler GameControlPanelUIHandler;
    public readonly GameEndPanelHandler GameEndPanelHandler;
    public readonly AwaitLoadContentPanelHandler AwaitLoadContentPanelHandler;
    public readonly ShopMoneyButtonsUIHandler ShopMoneyButtonsUIHandler;

    public LevelUIProviderBuildMode(LevelUIView levelUIView, GameControlPanelUIHandler gameControlPanelUIHandler,
        ShopMoneyButtonsUIHandler shopMoneyButtonsUIHandler, IReadOnlyList<ChoiceCaseView> choiceCasesViews, BlackFrameUIHandler blackFrameUIHandler, Wallet wallet,
        DisableNodesContentEvent disableNodesContentEvent, SwitchToNextNodeEvent switchToNextNodeEvent,
        CustomizationCharacterPanelUI customizationCharacterPanelUI, GlobalUIHandler globalUIHandler, 
        ButtonTransitionToMainSceneUIHandler buttonTransitionToMainSceneUIHandler,
        LoadAssetsPercentHandler loadAssetsPercentHandler, OnAwaitLoadContentEvent<AwaitLoadContentPanel> onAwaitLoadContentEvent,
        OnEndGameEvent onEndGameEvent, PhoneContentProvider phoneContentProvider, PanelResourceHandler panelResourceHandler, Action phoneInitOperation)
        : base(levelUIView, blackFrameUIHandler, choiceCasesViews, wallet, disableNodesContentEvent, switchToNextNodeEvent,
            customizationCharacterPanelUI, phoneContentProvider, panelResourceHandler, phoneInitOperation)
    {
        GameControlPanelUIHandler = gameControlPanelUIHandler;
        GameEndPanelHandler = new GameEndPanelHandler(globalUIHandler.LoadIndicatorUIHandler, blackFrameUIHandler,
            buttonTransitionToMainSceneUIHandler, levelUIView.transform);
        ShopMoneyButtonsUIHandler = shopMoneyButtonsUIHandler;
        AwaitLoadContentPanelHandler = new AwaitLoadContentPanelHandler(blackFrameUIHandler, globalUIHandler.LoadScreenUIHandler,
            loadAssetsPercentHandler, onAwaitLoadContentEvent);
        onEndGameEvent.Subscribe(()=>
        {
            GameEndPanelHandler.ShowPanel().Forget();
        });
    }
    public override void Shutdown()
    {
        base.Shutdown();
        GameControlPanelUIHandler.Shutdown();
        ChoicePanelUIHandler.Shutdown();
        ShopMoneyButtonsUIHandler.Shutdown();
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        List<LocalizationString> strings = new List<LocalizationString>()
        {
            GameEndPanelHandler.TextDescription, GameEndPanelHandler.TextLabel, GameEndPanelHandler.ButtonBackToMenu,
            AwaitLoadContentPanelHandler.AwaitLoadText
        };
        strings.AddRange(GameControlPanelUIHandler.GetLocalizableContent());
        strings.AddRange(PhoneUIHandler.GetLocalizableContent());
        return strings;
    }
}