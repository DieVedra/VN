using UnityEngine;

public class LevelUIProviderBuildMode : LevelUIProviderEditMode
{
    public readonly GameControlPanelUIHandler GameControlPanelUIHandler;
    public readonly GameEndPanelHandler GameEndPanelHandler;

    
    public LevelUIProviderBuildMode(LevelUIView levelUIView, BlackFrameUIHandler blackFrameUIHandler, Wallet wallet,
        DisableNodesContentEvent disableNodesContentEvent, SwitchToNextNodeEvent switchToNextNodeEvent,
        CustomizationCharacterPanelUI customizationCharacterPanelUI, BlockGameControlPanelUIEvent<bool> blockGameControlPanelUI, 
        ILevelLocalizationHandler localizationHandler, GlobalSound globalSound, MainMenuLocalizationHandler mainMenuLocalizationHandler,
        GlobalUIHandler globalUIHandler, ButtonTransitionToMainSceneUIHandler buttonTransitionToMainSceneUIHandler) 
        : base(levelUIView, blackFrameUIHandler, wallet, disableNodesContentEvent, switchToNextNodeEvent, customizationCharacterPanelUI)
    {
        if (Application.isPlaying)
        {
            GameControlPanelUIHandler = new GameControlPanelUIHandler(levelUIView.GameControlPanelView, globalUIHandler,
                globalSound, wallet, mainMenuLocalizationHandler, blackFrameUIHandler, buttonTransitionToMainSceneUIHandler,
                localizationHandler, blockGameControlPanelUI);
            GameEndPanelHandler = new GameEndPanelHandler(globalUIHandler.LoadIndicatorUIHandler, blackFrameUIHandler,
                buttonTransitionToMainSceneUIHandler, levelUIView.transform);
        }
    }

    public void Dispose()
    {
        GameControlPanelUIHandler.Dispose();
        ChoicePanelUIHandler.Dispose();
    }
}