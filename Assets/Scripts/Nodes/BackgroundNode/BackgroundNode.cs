using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[NodeWidth(350),NodeTint("#DB620A")]
public class BackgroundNode : BaseNode
{
    [SerializeField] private BackgroundNodeMode _backgroundNodeMode;
    [SerializeField] private int _index;
    [SerializeField] private int _indexTo;
    [SerializeField] private string _key;
    
    [SerializeField] private float _changeColorDuration;
    [SerializeField] private float _changeMode2Duration;
    
    [SerializeField] private bool _awaitedSmoothChangeBackground;
    [SerializeField] private bool _awaitedSmoothBackgroundChangePosition;
    [SerializeField] private bool _awaitedSetColorOverlayBackground;
    [SerializeField] private bool _awaited;
    
    [SerializeField] private bool _isSmoothCurtain;
    [SerializeField] private bool _mode3Enable;
    [SerializeField] private BackgroundPosition _backgroundPosition;
    [SerializeField] private BackgroundPosition _backgroundPositionMode2;

    [SerializeField] private Color _color;

    private const float _minValue = 0f;
    private const float _defaultDuration = 0.7f;
    private IBackgroundsProviderToBackgroundNode _background;
    public IReadOnlyDictionary<string, BackgroundContent> BackgroundsDictionary => _background?.GetBackgroundContentDictionary;
    public bool IsSmoothCurtain => _isSmoothCurtain;
    public void ConstructBackgroundNode(IBackgroundsProviderToBackgroundNode background)
    {
        _background = background;
        if (Math.Abs(_changeMode2Duration - _minValue) < 0.05f)
        {
            _changeMode2Duration = _defaultDuration;
        }
    }

    public override async UniTask Enter(bool isMerged = false)
    {
        CancellationTokenSource = new CancellationTokenSource();
        IsMerged = isMerged;
        switch (_backgroundNodeMode)
        {
            case BackgroundNodeMode.Mode1:
                await Mode1(isMerged);
                break;
            case BackgroundNodeMode.Mode2:
                await Mode2(isMerged);
                break;
            case BackgroundNodeMode.Mode3:
                await Mode3(isMerged);
                break;
        }
        TrySwitchToNextNode();
    }

    private async UniTask Mode1(bool isMerged)
    {
        if (_isSmoothCurtain == true)
        {
            if (_awaited)
            {
                if (isMerged == false)
                {
                    ButtonSwitchSlideUIHandler.ActivateSkipTransition(SkipEnterTransition);
                }
                await _background.SmoothBackgroundChangePosition(CancellationTokenSource.Token, _backgroundPosition, _key);
            }
            else
            {
                _background.SmoothBackgroundChangePosition(CancellationTokenSource.Token, _backgroundPosition, _key).Forget();
            }
        }
        else
        {
            SetInfoToView();
        }
    }

    private async UniTask Mode2(bool isMerged)
    {
        if (_awaited)
        {
            if (isMerged == false)
            {
                ButtonSwitchSlideUIHandler.ActivateSkipTransition(SkipEnterTransition);
            }
            await _background.SmoothChangeBackground(_key,  _changeMode2Duration, _backgroundPositionMode2, CancellationTokenSource.Token);
        }
        else
        {
            _background.SmoothChangeBackground(_key,  _changeMode2Duration, _backgroundPositionMode2, CancellationTokenSource.Token).Forget();
        }
    }
    private async UniTask Mode3(bool isMerged)
    {
        if (_awaited)
        {
            if (isMerged == false)
            {
                ButtonSwitchSlideUIHandler.ActivateSkipTransition(SkipEnterTransition);
            }
            await _background.SetColorOverlayBackground(_color, CancellationTokenSource.Token, _changeColorDuration, _mode3Enable);
        }
        else
        {
            _background.SetColorOverlayBackground(_color, CancellationTokenSource.Token, _changeColorDuration, _mode3Enable).Forget();
        }
    }
    public override void SkipEnterTransition()
    {
        if (_isSmoothCurtain == true)
        {
            CancellationTokenSource.Cancel();
            SetInfoToView();
        }
    }

    protected override void SetInfoToView()
    {
        switch (_backgroundNodeMode)
        {
            case BackgroundNodeMode.Mode1:
                _background.SetBackgroundPosition(_backgroundPosition, _key);
                break;
            case BackgroundNodeMode.Mode2:
                _background.SmoothChangeBackgroundEmmidiately(_key, _backgroundPositionMode2);
                break;
            case BackgroundNodeMode.Mode3:
                _background.SetColorOverlayBackground(_color, _mode3Enable);
                break;
        }
    }
    private void TrySwitchToNextNode()
    {
        if (IsMerged == false)
        {
            SwitchToNextNodeEvent.Execute();
        }
    }
}