
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
    private BlackFrameUIHandler _darkeningBackgroundFrameUIHandler;
    private LoadIndicatorUIHandler _loadIndicatorUIHandler;
    private ConfirmedPanelUIHandler _confirmedPanelUIHandler;
    private CancellationTokenSource _cancellationTokenSource;
    private ButtonTransitionToMainSceneUIHandler _buttonTransitionToMainSceneUIHandler;
    private LoadScreenUIHandler _loadScreenUIHandler;
    
    private SettingsPanelUIHandler _settingsPanelUIHandler;
    private SettingsPanelButtonUIHandler _settingsPanelButtonUIHandler;
    
    
    private ShopMoneyPanelUIHandler _shopMoneyPanelUIHandler;
    
    private ReactiveProperty<bool> _anyWindowIsOpen;
    private bool _panelIsVisible;
    public GameControlPanelUIHandler(GameControlPanelView gameControlPanelView, GlobalUIHandler globalUIHandler,
        ReactiveCommand onSceneTransition, GlobalSound globalSound,
        MainMenuLocalizationHandler mainMenuLocalizationHandler, BlackFrameUIHandler darkeningBackgroundFrameUIHandler)
    {
        _gameControlPanelView = gameControlPanelView;
        _loadScreenUIHandler = globalUIHandler.LoadScreenUIHandler;
        _settingsPanelUIHandler = globalUIHandler.SettingsPanelUIHandler;
        _shopMoneyPanelUIHandler = globalUIHandler.ShopMoneyPanelUIHandler;
        _parent = globalUIHandler.GlobalUITransforn;
        _onSceneTransition = onSceneTransition;
        _globalSound = globalSound;
        _localizationChanger = mainMenuLocalizationHandler;
        _darkeningBackgroundFrameUIHandler = darkeningBackgroundFrameUIHandler;
        _loadIndicatorUIHandler = globalUIHandler.LoadIndicatorUIHandler;
        _panelIsVisible = false;
        _anyWindowIsOpen = new ReactiveProperty<bool> {Value = false};
        _gameControlPanelView.CanvasGroup.alpha = 0f;
        _gameControlPanelView.SettingsButtonView.gameObject.SetActive(false);
        _gameControlPanelView.ButtonGoToMainMenu.gameObject.SetActive(false);
        
        _settingsPanelButtonUIHandler = new SettingsPanelButtonUIHandler(globalUIHandler.GlobalUITransforn, globalUIHandler.SettingsPanelUIHandler,
            _darkeningBackgroundFrameUIHandler, globalUIHandler.LoadIndicatorUIHandler);
        _settingsPanelButtonUIHandler.Init(_gameControlPanelView.SettingsButtonView, globalSound.SoundStatus, _localizationChanger, false);
        
        
        SubscribeButtonShowPanel();
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
    }
    private async UniTaskVoid PressShowButton()
    {
        if (_panelIsVisible == false)
        {
            _gameControlPanelView.ButtonShowPanel.onClick.RemoveAllListeners();
            _cancellationTokenSource = new CancellationTokenSource();
            
            _gameControlPanelView.SettingsButtonView.gameObject.SetActive(true);
            
            
            _gameControlPanelView.ButtonGoToMainMenu.gameObject.SetActive(true);
            
            _gameControlPanelView.ButtonGoToMainMenu.onClick.AddListener(() =>
            {
                
                PrepareTransitionToMainScene().Forget();
            });
            
            await _gameControlPanelView.CanvasGroup.DOFade(AnimationValuesProvider.MaxValue, AnimationValuesProvider.MaxValue).WithCancellation(_cancellationTokenSource.Token);
            SubscribeButtonShowPanel();
            _panelIsVisible = true;
            
            await UniTask.Delay(TimeSpan.FromSeconds(_timeDelay), cancellationToken: _cancellationTokenSource.Token);
            
            SetPanelInvisible().Forget();
        }
        else
        {
            SetPanelInvisible().Forget();
        }
    }

    private void SubscribeButtonShowPanel()
    {
        _gameControlPanelView.ButtonShowPanel.onClick.AddListener(() =>
        { 
            PressShowButton().Forget();
        });
    }

    private async UniTaskVoid SetPanelInvisible()
    {
        UnsubscribeButtons();
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        await _gameControlPanelView.CanvasGroup.DOFade(AnimationValuesProvider.MinValue, AnimationValuesProvider.MaxValue).WithCancellation(_cancellationTokenSource.Token);
        _panelIsVisible = false;
        if (_cancellationTokenSource.IsCancellationRequested == false)
        {
            _gameControlPanelView.SettingsButtonView.gameObject.SetActive(false);
            _gameControlPanelView.ButtonGoToMainMenu.gameObject.SetActive(false);
        }

        SubscribeButtonShowPanel();
    }

    private void UnsubscribeButtons()
    {
        _gameControlPanelView.SettingsButtonView.Button.onClick.RemoveAllListeners();
        _gameControlPanelView.ButtonGoToMainMenu.onClick.RemoveAllListeners();
        _gameControlPanelView.ButtonShowPanel.onClick.RemoveAllListeners();
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
}