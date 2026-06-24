using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
        GameEndPanelHandler gameEndPanelHandler,
        LoadAssetsPercentHandler loadAssetsPercentHandler, OnAwaitLoadContentEvent<AwaitLoadContentPanel> onAwaitLoadContentEvent,
        OnEndGameEvent onEndGameEvent, PhoneContentProvider phoneContentProvider, PanelResourceVisionHandler panelResourceVisionHandler,
        Action phoneInitOperation)
        : base(levelUIView, blackFrameUIHandler, choiceCasesViews, wallet, disableNodesContentEvent, switchToNextNodeEvent,
            customizationCharacterPanelUI, phoneContentProvider, panelResourceVisionHandler, phoneInitOperation)
    {
        GameControlPanelUIHandler = gameControlPanelUIHandler;
        GameEndPanelHandler = gameEndPanelHandler;
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
        GameEndPanelHandler.Shutdown();
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        List<LocalizationString> strings = new List<LocalizationString>();
        strings.AddRange(GameControlPanelUIHandler.GetLocalizableContent());
        strings.AddRange(PhoneUIHandler.GetLocalizableContent());
        strings.AddRange(AwaitLoadContentPanelHandler.GetLocalizableContent());
        strings.AddRange(GameEndPanelHandler.GetLocalizableContent());
        return strings;
    }
}