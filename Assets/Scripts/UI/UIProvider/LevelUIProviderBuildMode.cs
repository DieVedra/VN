using UnityEngine;
public class LevelUIProviderBuildMode : LevelUIProviderEditMode
{
    public readonly GameControlPanelUIHandler GameControlPanelUIHandler;
    public readonly GameEndPanelHandler GameEndPanelHandler;
    public readonly AwaitLoadContentPanelHandler AwaitLoadContentPanelHandler;
    
    public LevelUIProviderBuildMode(LevelUIView levelUIView, BlackFrameUIHandler blackFrameUIHandler, Wallet wallet,
        DisableNodesContentEvent disableNodesContentEvent, SwitchToNextNodeEvent switchToNextNodeEvent,
        CustomizationCharacterPanelUI customizationCharacterPanelUI, BlockGameControlPanelUIEvent<bool> blockGameControlPanelUI, 
        ILevelLocalizationHandler localizationHandler, GlobalSound globalSound, MainMenuLocalizationHandler mainMenuLocalizationHandler,
        GlobalUIHandler globalUIHandler, ButtonTransitionToMainSceneUIHandler buttonTransitionToMainSceneUIHandler, LoadAssetsPercentHandler loadAssetsPercentHandler) 
        : base(levelUIView, blackFrameUIHandler, wallet, disableNodesContentEvent, switchToNextNodeEvent, customizationCharacterPanelUI)
    {
        if (Application.isPlaying)
        {
            GameControlPanelUIHandler = new GameControlPanelUIHandler(levelUIView.GameControlPanelView, globalUIHandler,
                globalSound, wallet, mainMenuLocalizationHandler, blackFrameUIHandler, buttonTransitionToMainSceneUIHandler,
                localizationHandler, blockGameControlPanelUI);
            GameEndPanelHandler = new GameEndPanelHandler(globalUIHandler.LoadIndicatorUIHandler, blackFrameUIHandler,
                buttonTransitionToMainSceneUIHandler, levelUIView.transform);
            AwaitLoadContentPanelHandler = new AwaitLoadContentPanelHandler(blackFrameUIHandler, globalUIHandler.LoadIndicatorUIHandler, loadAssetsPercentHandler);
        }
    }
    public void Dispose()
    {
        GameControlPanelUIHandler.Dispose();
        ChoicePanelUIHandler.Dispose();
    }
}