using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
public class LevelUIProviderBuildMode : LevelUIProviderEditMode, ILocalizable
{
    public readonly GameControlPanelUIHandler GameControlPanelUIHandler;
    public readonly GameEndPanelHandler GameEndPanelHandler;
    public readonly AwaitLoadContentPanelHandler AwaitLoadContentPanelHandler;

    public LevelUIProviderBuildMode(LevelUIView levelUIView, BlackFrameUIHandler blackFrameUIHandler, Wallet wallet,
        DisableNodesContentEvent disableNodesContentEvent, SwitchToNextNodeEvent switchToNextNodeEvent,
        CustomizationCharacterPanelUI customizationCharacterPanelUI, BlockGameControlPanelUIEvent<bool> blockGameControlPanelUI, 
        ILevelLocalizationHandler localizationHandler, GlobalSound globalSound, PanelsLocalizationHandler panelsLocalizationHandler,
        GlobalUIHandler globalUIHandler, ButtonTransitionToMainSceneUIHandler buttonTransitionToMainSceneUIHandler,
        LoadAssetsPercentHandler loadAssetsPercentHandler, OnAwaitLoadContentEvent<AwaitLoadContentPanel> onAwaitLoadContentEvent,
        OnEndGameEvent onEndGameEvent, PoolsProvider poolsProvider)
        : base(levelUIView, blackFrameUIHandler, wallet, disableNodesContentEvent, switchToNextNodeEvent, customizationCharacterPanelUI, poolsProvider)
    {
        if (Application.isPlaying)
        {
            GameControlPanelUIHandler = new GameControlPanelUIHandler(levelUIView.GameControlPanelView, globalUIHandler,
                globalSound, wallet, panelsLocalizationHandler, blackFrameUIHandler, buttonTransitionToMainSceneUIHandler,
                localizationHandler, blockGameControlPanelUI);
            GameEndPanelHandler = new GameEndPanelHandler(globalUIHandler.LoadIndicatorUIHandler, blackFrameUIHandler,
                buttonTransitionToMainSceneUIHandler, levelUIView.transform);
            AwaitLoadContentPanelHandler = new AwaitLoadContentPanelHandler(blackFrameUIHandler, globalUIHandler.LoadScreenUIHandler,
                loadAssetsPercentHandler, onAwaitLoadContentEvent);
            onEndGameEvent.Subscribe(()=>
            {
                GameEndPanelHandler.ShowPanel().Forget();
            });
        }
    }
    public override void Dispose()
    {
        base.Dispose();
        GameControlPanelUIHandler.Dispose();
        ChoicePanelUIHandler.Dispose();
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return ListExtensions.MergeIReadOnlyLists(
            new[] {
                GameEndPanelHandler.TextDescription, GameEndPanelHandler.TextLabel, GameEndPanelHandler.ButtonBackToMenu,
                AwaitLoadContentPanelHandler.AwaitLoadText}, 
            GameControlPanelUIHandler.GetLocalizableContent(),
            PhoneUIHandler.GetLocalizableContent());
    }
}