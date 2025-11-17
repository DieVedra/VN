using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
public class LevelUIProviderBuildMode : LevelUIProviderEditMode, ILocalizable
{
    public readonly GameControlPanelUIHandler GameControlPanelUIHandler;
    public readonly GameEndPanelHandler GameEndPanelHandler;
    public readonly AwaitLoadContentPanelHandler AwaitLoadContentPanelHandler;

    public LevelUIProviderBuildMode(LevelUIView levelUIView, BlackFrameUIHandler blackFrameUIHandler, Wallet wallet, IChoicePanelInitializer choicePanelInitializer,
        DisableNodesContentEvent disableNodesContentEvent, SwitchToNextNodeEvent switchToNextNodeEvent,
        CustomizationCharacterPanelUI customizationCharacterPanelUI, BlockGameControlPanelUIEvent<bool> blockGameControlPanelUI, 
        ILevelLocalizationHandler localizationHandler, GlobalSound globalSound, PanelsLocalizationHandler panelsLocalizationHandler,
        GlobalUIHandler globalUIHandler, ButtonTransitionToMainSceneUIHandler buttonTransitionToMainSceneUIHandler,
        LoadAssetsPercentHandler loadAssetsPercentHandler, OnAwaitLoadContentEvent<AwaitLoadContentPanel> onAwaitLoadContentEvent,
        OnEndGameEvent onEndGameEvent, PhoneContentProvider phoneContentProvider, Action phoneInitOperation)
        : base(levelUIView, blackFrameUIHandler, choicePanelInitializer, wallet, disableNodesContentEvent, switchToNextNodeEvent,
            customizationCharacterPanelUI, phoneContentProvider, phoneInitOperation)
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