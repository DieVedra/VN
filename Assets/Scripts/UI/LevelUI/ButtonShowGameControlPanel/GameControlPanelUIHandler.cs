﻿
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class GameControlPanelUIHandler
{
    
    
    
    private readonly GameControlPanelView _gameControlPanelView;
    private readonly ReactiveCommand _onSceneTransition;
    private BlackFrameUIHandler _blackFrameUIHandler;
    private LoadIndicatorUIHandler _loadIndicatorUIHandler;
    private SettingsPanelUIHandler _settingsPanelUIHandler;
    private ConfirmedPanelUIHandler _confirmedPanelUIHandler;
    private BlackFrameView _blackFrameView;
    private CancellationTokenSource _cancellationTokenSource;
    private ButtonTransitionToMainSceneUIHandler _buttonTransitionToMainSceneUIHandler;
    private ReactiveProperty<bool> _anyWindowIsOpen;
    private bool _panelIsVisible;
    public GameControlPanelUIHandler(GameControlPanelView gameControlPanelView, ReactiveCommand onSceneTransition)
    {
        _gameControlPanelView = gameControlPanelView;
        _onSceneTransition = onSceneTransition;
        _panelIsVisible = false;
        _anyWindowIsOpen = new ReactiveProperty<bool> {Value = false};
        _gameControlPanelView.CanvasGroup.alpha = 0f;
        _gameControlPanelView.ButtonSettings.gameObject.SetActive(false);
        _gameControlPanelView.ButtonGoToMainMenu.gameObject.SetActive(false);
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
            
            _gameControlPanelView.ButtonSettings.gameObject.SetActive(true);
            _gameControlPanelView.ButtonSettings.onClick.AddListener(() =>
            {

                ShowSettingsPanel().Forget();
                // _anyWindowIsOpen.Value = true;
            });
            
            _gameControlPanelView.ButtonGoToMainMenu.gameObject.SetActive(true);
            _gameControlPanelView.ButtonGoToMainMenu.onClick.AddListener(() =>
            {
                
                PrepareTransitionToMainScene().Forget();
                // _anyWindowIsOpen.Value = true;
            });
            
            await _gameControlPanelView.CanvasGroup.DOFade(1f, 1f).WithCancellation(_cancellationTokenSource.Token);
            SubscribeButtonShowPanel();
            _panelIsVisible = true;
            
            await UniTask.Delay(TimeSpan.FromSeconds(3f), cancellationToken: _cancellationTokenSource.Token);
            
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
        await _gameControlPanelView.CanvasGroup.DOFade(0f, 1f).WithCancellation(_cancellationTokenSource.Token);
        _panelIsVisible = false;
        _gameControlPanelView.ButtonSettings.gameObject.SetActive(false);
        _gameControlPanelView.ButtonGoToMainMenu.gameObject.SetActive(false);
        SubscribeButtonShowPanel();
    }

    private void UnsubscribeButtons()
    {
        _gameControlPanelView.ButtonSettings.onClick.RemoveAllListeners();
        _gameControlPanelView.ButtonGoToMainMenu.onClick.RemoveAllListeners();
        _gameControlPanelView.ButtonShowPanel.onClick.RemoveAllListeners();
    }

    private async UniTask ShowSettingsPanel()
    {
        await TryCreateBlackFrameUIHandler();
        _blackFrameUIHandler.CloseTranslucent().Forget();
        await TryCreateLoadIndicatorUIHandler();
        _loadIndicatorUIHandler.SetClearIndicateMode();
        _loadIndicatorUIHandler.StartIndicate();
        await TryCreateSettingsPanelUIHandler();
        _loadIndicatorUIHandler.StopIndicate();
        _settingsPanelUIHandler.Show(_blackFrameUIHandler);
    }

    private async UniTask PrepareTransitionToMainScene()
    {
        await TryCreateBlackFrameUIHandler();
        _blackFrameUIHandler.CloseTranslucent().Forget();
        await TryCreateLoadIndicatorUIHandler();
        _loadIndicatorUIHandler.SetClearIndicateMode();
        // _loadIndicatorUIHandler.StartIndicate();
        await TryCreateAndShowConfirmedPanel();
        // _loadIndicatorUIHandler.StopIndicate();
    }
    private async UniTask TryCreateBlackFrameUIHandler()
    {
        if (_blackFrameUIHandler == null)
        {
            _blackFrameView = await new BlackFramePanelAssetProvider().LoadBlackFramePanel(_gameControlPanelView.transform.parent);
            _blackFrameUIHandler = new BlackFrameUIHandler(_blackFrameView);
            _blackFrameUIHandler.SetAsLastSibling();
            _blackFrameView.Image.color = Color.clear;
        }
    }

    private async UniTask TryCreateLoadIndicatorUIHandler()
    {
        if (_loadIndicatorUIHandler == null)
        {
            _loadIndicatorUIHandler = new LoadIndicatorUIHandler();
            await _loadIndicatorUIHandler.Init(_blackFrameUIHandler.Transform);
        }
    }

    private async UniTask TryCreateSettingsPanelUIHandler()
    {
        if (_settingsPanelUIHandler == null)
        {
            _settingsPanelUIHandler = new SettingsPanelUIHandler();
            await _settingsPanelUIHandler.Init(_blackFrameUIHandler.Transform);
        }
    }

    private async UniTask TryCreateAndShowConfirmedPanel()
    {
        if (_confirmedPanelUIHandler == null)
        {
            _confirmedPanelUIHandler = new ConfirmedPanelUIHandler(_loadIndicatorUIHandler, _blackFrameUIHandler, _blackFrameUIHandler.Transform);
            _buttonTransitionToMainSceneUIHandler = new ButtonTransitionToMainSceneUIHandler(_onSceneTransition, _blackFrameUIHandler);
            await _confirmedPanelUIHandler.Show(
                _buttonTransitionToMainSceneUIHandler.LabelText, _buttonTransitionToMainSceneUIHandler.TranscriptionText,
                _buttonTransitionToMainSceneUIHandler.ButtonText, _buttonTransitionToMainSceneUIHandler.HeightPanel,
                _buttonTransitionToMainSceneUIHandler.FontSizeValue, ()=>
                {
                    _buttonTransitionToMainSceneUIHandler.Press().Forget();
                }, true);
        }
    }
}