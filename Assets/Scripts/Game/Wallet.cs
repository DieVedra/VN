using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class Wallet
{
    private const float _delayAddValue = 0.005f;
    private readonly ReactiveProperty<int> _monetsReactiveProperty;
    private readonly ReactiveProperty<int> _heartsReactiveProperty;
    private CancellationTokenSource _cancellationTokenSource;
    
    private int _lastMonetAddValue = 0;
    private int _lastHeartsAddValue = 0;
    private int _lastMonetRemoveValue = 0;
    private int _lastHeartsRemoveValue = 0;

    private int _previousMonetValue;
    private int _previousHeartsValue;
    
    private bool _addResourceGraduallyIsProgress;
    private bool _removeResourceGraduallyIsProgress;
    public int Monets => _monetsReactiveProperty.Value;
    public int Hearts => _heartsReactiveProperty.Value;
    public ReactiveProperty<int> MonetsReactiveProperty => _monetsReactiveProperty;
    public ReactiveProperty<int> HeartsReactiveProperty => _heartsReactiveProperty;
    
    public Wallet(int monets, int hearts)
    {
        _monetsReactiveProperty = new ReactiveProperty<int>();
        _heartsReactiveProperty = new ReactiveProperty<int>();
        _monetsReactiveProperty.Value = monets;
        _heartsReactiveProperty.Value = hearts;
        _previousMonetValue = monets;
        _previousHeartsValue = hearts;
        _addResourceGraduallyIsProgress = false;
        _removeResourceGraduallyIsProgress = false;
    }

    public Wallet(SaveData saveData)
    {
        _monetsReactiveProperty = new ReactiveProperty<int>();
        _heartsReactiveProperty = new ReactiveProperty<int>();
        _monetsReactiveProperty.Value = saveData.Monets;
        _heartsReactiveProperty.Value = saveData.Hearts;
        _previousMonetValue = saveData.Monets;
        _previousHeartsValue = saveData.Hearts;
        _addResourceGraduallyIsProgress = false;
        _removeResourceGraduallyIsProgress = false;
        
        _monetsReactiveProperty.Subscribe(_ =>
        {
            saveData.Monets = Monets;
        });
        _heartsReactiveProperty.Subscribe(_ =>
        {
            saveData.Hearts = Hearts;
        });
    }

    public void Dispose()
    {
        TryAbortGraduallyResourceManipulation();
        _monetsReactiveProperty.Dispose();
        _heartsReactiveProperty.Dispose();
    }
    public void AddCash(int value, bool immediately = true)
    {
        if (value > 0)
        {
            _lastMonetAddValue = value;
            _previousMonetValue = _monetsReactiveProperty.Value;
            if (immediately == true)
            {
                _monetsReactiveProperty.Value += value;
            }
            else
            {
                AddGradually(_monetsReactiveProperty, value).Forget();
            }
        }
    }
    public void AddHearts(int value, bool immediately = true)
    {
        if (value > 0)
        {
            _lastHeartsAddValue = value;
            _previousHeartsValue = _heartsReactiveProperty.Value;
            if (immediately == true)
            {
                _heartsReactiveProperty.Value += value;
            }
            else
            {
                AddGradually(_heartsReactiveProperty, value).Forget();
            }
        }
    }
    public bool CashAvailable(int value)
    {
        if (value > 0 && _monetsReactiveProperty.Value >= value)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool HeartsAvailable(int value)
    {
        if (value > 0 && _heartsReactiveProperty.Value >= value)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public (bool, int) RemoveCash(int value, bool immediately = true)
    {
        if (CashAvailable(value) == true)
        {
            _lastMonetRemoveValue = value;
            _previousMonetValue = _monetsReactiveProperty.Value;
            if (immediately == true)
            {
                _monetsReactiveProperty.Value -= value;
            }
            else
            {
                RemoveGradually(_monetsReactiveProperty, value).Forget();
            }
            return (true, _monetsReactiveProperty.Value);
        }
        else
        {
            return (false, _monetsReactiveProperty.Value);
        }
    }
    public (bool, int) RemoveHearts(int value, bool immediately = true)
    {
        if (HeartsAvailable(value) == true)
        {
            _lastHeartsRemoveValue = value;
            _previousHeartsValue = _heartsReactiveProperty.Value;
            if (immediately == true)
            {
                _heartsReactiveProperty.Value -= value;
            }
            else
            {
                RemoveGradually(_heartsReactiveProperty, value).Forget();
            }
            return (true, _heartsReactiveProperty.Value);
        }
        else
        {
            return (false, _heartsReactiveProperty.Value);
        }
    }

    private async UniTask AddGradually(ReactiveProperty<int> reactiveProperty, int value)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _addResourceGraduallyIsProgress = true;
        for (int i = 0; i < value; i++)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_delayAddValue), cancellationToken: _cancellationTokenSource.Token);
            reactiveProperty.Value++;
        }
        _addResourceGraduallyIsProgress = false;
    }
    private async UniTask RemoveGradually(ReactiveProperty<int> reactiveProperty, int value)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _removeResourceGraduallyIsProgress = true;
        for (int i = 0; i < value; i++)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_delayAddValue), cancellationToken: _cancellationTokenSource.Token);
            reactiveProperty.Value--;
        }

        _removeResourceGraduallyIsProgress = false;
    }

    private void TryAbortGraduallyResourceManipulation()
    {
        _cancellationTokenSource?.Cancel();

        if (_addResourceGraduallyIsProgress == true)
        {
            _monetsReactiveProperty.Value = _previousMonetValue + _lastMonetAddValue;
            _heartsReactiveProperty.Value = _previousHeartsValue + _lastHeartsAddValue;
            _addResourceGraduallyIsProgress = false;
        }

        if (_removeResourceGraduallyIsProgress == true)
        {
            _monetsReactiveProperty.Value = _previousMonetValue - _lastMonetRemoveValue;
            _heartsReactiveProperty.Value = _previousHeartsValue - _lastHeartsRemoveValue;
            _removeResourceGraduallyIsProgress = false;
        }
    }
}