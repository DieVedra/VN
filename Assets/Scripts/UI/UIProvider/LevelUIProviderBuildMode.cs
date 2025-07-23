using UnityEngine;

public class LevelUIProviderBuildMode : LevelUIProviderEditMode
{
    public readonly GameControlPanelUIHandler GameControlPanelUIHandler;
    
    public LevelUIProviderBuildMode(LevelUIView levelUIView, BlackFrameUIHandler blackFrameUIHandler, Wallet wallet,
        OnSceneTransitionEvent onSceneTransition, DisableNodesContentEvent disableNodesContentEvent, SwitchToNextNodeEvent switchToNextNodeEvent,
        CustomizationCharacterPanelUI customizationCharacterPanelUI, BlockGameControlPanelUIEvent<bool> blockGameControlPanelUI, 
        ILevelLocalizationHandler localizationHandler, GlobalSound globalSound, MainMenuLocalizationHandler mainMenuLocalizationHandler, GlobalUIHandler globalUIHandler) 
        : base(levelUIView, blackFrameUIHandler, wallet, disableNodesContentEvent, switchToNextNodeEvent, customizationCharacterPanelUI)
    {
        if (Application.isPlaying)
        {
            GameControlPanelUIHandler = new GameControlPanelUIHandler(levelUIView.GameControlPanelView, globalUIHandler, onSceneTransition,
                globalSound, wallet, mainMenuLocalizationHandler, blackFrameUIHandler, localizationHandler, blockGameControlPanelUI);
        }
    }

    public void Dispose()
    {
        GameControlPanelUIHandler.Dispose();
        ChoicePanelUIHandler.Dispose();
    }
}