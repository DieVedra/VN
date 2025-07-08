
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class GameControlPanelUIHandler
{
    private const float _timeDelay = 3f;
    private readonly GameControlPanelView _gameControlPanelView;
    private readonly ReactiveCommand _onSceneTransition;
    private readonly GlobalSound _globalSound;
    private readonly ILocalizationChanger _localizationChanger;
    private readonly Transform _parent;
    private readonly ILevelLocalizationHandler _levelLocalizationHandler;
    private readonly ReactiveCommand<bool> _blockGameControlPanelUI;
    private BlackFrameUIHandler _darkeningBackgroundFrameUIHandler;
    private LoadIndicatorUIHandler _loadIndicatorUIHandler;
    private ConfirmedPanelUIHandler _confirmedPanelUIHandler;
    private CancellationTokenSource _cancellationTokenSource;
    private ButtonTransitionToMainSceneUIHandler _buttonTransitionToMainSceneUIHandler;
    private LoadScreenUIHandler _loadScreenUIHandler;
    
    private SettingsPanelUIHandler _settingsPanelUIHandler;
    private SettingsPanelButtonUIHandler _settingsPanelButtonUIHandler;
    private ShopMoneyButtonsUIHandler _shopMoneyButtonsUIHandler;
    
    private ShopMoneyPanelUIHandler _shopMoneyPanelUIHandler;
    
    private ReactiveProperty<bool> _anyWindowIsOpen;
    private bool _panelIsVisible;
    private bool _panelIsBlocked;
    public GameControlPanelUIHandler(GameControlPanelView gameControlPanelView, GlobalUIHandler globalUIHandler,
        ReactiveCommand onSceneTransition, GlobalSound globalSound, Wallet wallet,
        MainMenuLocalizationHandler mainMenuLocalizationHandler, BlackFrameUIHandler darkeningBackgroundFrameUIHandler,
        ILevelLocalizationHandler localizationHandler, ReactiveCommand<bool> blockGameControlPanelUI)
    {
        _gameControlPanelView = gameControlPanelView;
        _loadScreenUIHandler = globalUIHandler.LoadScreenUIHandler;
        _settingsPanelUIHandler = globalUIHandler.SettingsPanelUIHandler;
        _shopMoneyPanelUIHandler = globalUIHandler.ShopMoneyPanelUIHandler;
        _parent = globalUIHandler.GlobalUITransforn;
        _loadIndicatorUIHandler = globalUIHandler.LoadIndicatorUIHandler;
        _onSceneTransition = onSceneTransition;
        _globalSound = globalSound;
        _localizationChanger = mainMenuLocalizationHandler;
        _darkeningBackgroundFrameUIHandler = darkeningBackgroundFrameUIHandler;
        _levelLocalizationHandler = localizationHandler;
        _blockGameControlPanelUI = blockGameControlPanelUI;
        _panelIsVisible = false;
        _panelIsBlocked = false;
        _anyWindowIsOpen = new ReactiveProperty<bool> {Value = false};
        _gameControlPanelView.CanvasGroup.alpha = 0f;
        _gameControlPanelView.SettingsButtonView.gameObject.SetActive(false);
        _gameControlPanelView.ButtonGoToMainMenu.gameObject.SetActive(false);

        
        _settingsPanelButtonUIHandler = new SettingsPanelButtonUIHandler(globalUIHandler.GlobalUITransforn, globalUIHandler.SettingsPanelUIHandler,
            globalUIHandler.LoadIndicatorUIHandler);
        
        
        
        _settingsPanelButtonUIHandler.BaseInit(_gameControlPanelView.SettingsButtonView, _darkeningBackgroundFrameUIHandler,
            globalSound.SoundStatus, _localizationChanger, false);
        _settingsPanelButtonUIHandler.InitInLevel(_levelLocalizationHandler);

        
        _shopMoneyButtonsUIHandler = new ShopMoneyButtonsUIHandler(globalUIHandler.LoadIndicatorUIHandler, wallet,
            globalUIHandler.ShopMoneyPanelUIHandler, globalUIHandler.GlobalUITransforn);
        _shopMoneyButtonsUIHandler.Init(_darkeningBackgroundFrameUIHandler, gameControlPanelView.ShopMoneyButtonView);
        _gameControlPanelView.ButtonShowPanel.onClick.AddListener(() =>
        {
            if (_panelIsBlocked == false)
            {
                ShowGameControlPanel().Forget();
            }
        });
        _gameControlPanelView.ButtonGoToMainMenu.onClick.AddListener(() =>
        {
            PrepareTransitionToMainScene().Forget();
        });
        blockGameControlPanelUI.Subscribe(BlockPanel);
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
    }
    private async UniTaskVoid ShowGameControlPanel()
    {
        if (_panelIsVisible == false)
        {
            await SetPanelVisible();
            
            await UniTask.Delay(TimeSpan.FromSeconds(_timeDelay), cancellationToken: _cancellationTokenSource.Token);
            
            SetPanelInvisible().Forget();
        }
        else
        {
            SetPanelInvisible().Forget();
        }
    }

    private async UniTask SetPanelVisible()
    {
        _panelIsVisible = true;

        ReInitCancellationSource();
            
        _gameControlPanelView.SettingsButtonView.gameObject.SetActive(true);
        _gameControlPanelView.ShopMoneyButtonView.gameObject.SetActive(true);
        _gameControlPanelView.ButtonGoToMainMenu.gameObject.SetActive(true);
        await DoFade(AnimationValuesProvider.MaxValue, GetCurrentDuration());
    }
    private async UniTaskVoid SetPanelInvisible()
    {
        _panelIsVisible = false;
        ReInitCancellationSource();
        await DoFade(AnimationValuesProvider.MinValue, GetCurrentDuration());

        OffButtonsOnPanel();
    }
    private void OffButtonsOnPanel()
    {
        _gameControlPanelView.SettingsButtonView.Button.gameObject.SetActive(false);
        _gameControlPanelView.ButtonGoToMainMenu.gameObject.SetActive(false);
        _gameControlPanelView.ShopMoneyButtonView.gameObject.SetActive(false);
    }

    private async UniTask PrepareTransitionToMainScene()
    {
        _darkeningBackgroundFrameUIHandler.CloseTranslucent().Forget();
        await TryCreateAndShowConfirmedPanel();
    }

    private async UniTask TryCreateAndShowConfirmedPanel()
    {
        if (_confirmedPanelUIHandler == null)
        {
            _confirmedPanelUIHandler = new ConfirmedPanelUIHandler(_loadIndicatorUIHandler, _darkeningBackgroundFrameUIHandler, _darkeningBackgroundFrameUIHandler.Transform);
            _buttonTransitionToMainSceneUIHandler = new ButtonTransitionToMainSceneUIHandler(_loadScreenUIHandler, _onSceneTransition);
            await _confirmedPanelUIHandler.Show(
                _buttonTransitionToMainSceneUIHandler.LabelText, _buttonTransitionToMainSceneUIHandler.TranscriptionText,
                _buttonTransitionToMainSceneUIHandler.ButtonText, ButtonTransitionToMainSceneUIHandler.HeightPanel,
                ButtonTransitionToMainSceneUIHandler.FontSizeValue, ()=>
                {
                    _buttonTransitionToMainSceneUIHandler.Press().Forget();
                }, true);
        }
    }

    private void ReInitCancellationSource()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    private float GetCurrentDuration()
    {
        if (_panelIsVisible == false)
        {
            return Mathf.Lerp(AnimationValuesProvider.MinValue, AnimationValuesProvider.MaxValue, _gameControlPanelView.CanvasGroup.alpha);
        }
        else
        {
            return Mathf.Lerp(AnimationValuesProvider.MaxValue, AnimationValuesProvider.MinValue, _gameControlPanelView.CanvasGroup.alpha);
        }
    }

    private async UniTask DoFade(float endValue, float duration)
    {
        await _gameControlPanelView.CanvasGroup.DOFade(endValue, duration).WithCancellation(_cancellationTokenSource.Token);
    }

    private void BlockPanel(bool key)
    {
        _panelIsBlocked = key;
    }
}