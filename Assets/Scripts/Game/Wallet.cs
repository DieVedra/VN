using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

[Serializable]
public class Wallet
{
    private const float _delayAddValue = 0.005f;
    private const int _graduallyValue = 1;
    
    private readonly ReactiveCommand<int> _monetsCountChanged;
    private readonly ReactiveCommand<int> _heartsCountChanged;
    
    private CancellationTokenSource _cancellationTokenSource;
    private int _lastMonetAddValue = 0;
    private int _lastHeartsAddValue = 0;
    private int _lastMonetRemoveValue = 0;
    private int _lastHeartsRemoveValue = 0;
    
    private int _monets;
    private int _hearts;
    
    private int _previousMonetValue;
    private int _previousHeartsValue;
    
    private bool _addResourceGraduallyIsProgress;
    private bool _removeResourceGraduallyIsProgress;
    public int GetMonetsCount => _monets;
    public int GetHeartsCount => _hearts;
    public ReactiveCommand<int> MonetsCountChanged => _monetsCountChanged;
    public ReactiveCommand<int> HeartsCountChanged => _heartsCountChanged;
    
    public Wallet(int monets, int hearts)
    {
        _monetsCountChanged = new ReactiveCommand<int>();
        _heartsCountChanged = new ReactiveCommand<int>();
        _monets = monets;
        _hearts = hearts;
        _previousMonetValue = monets;
        _previousHeartsValue = hearts;
        _addResourceGraduallyIsProgress = false;
        _removeResourceGraduallyIsProgress = false;
    }

    public Wallet(SaveData saveData)
    {
        _monetsCountChanged = new ReactiveCommand<int>();
        _heartsCountChanged = new ReactiveCommand<int>();
        _monets = saveData.Monets;
        _hearts = saveData.Hearts;
        _previousMonetValue = saveData.Monets;
        _previousHeartsValue = saveData.Hearts;
        _addResourceGraduallyIsProgress = false;
        _removeResourceGraduallyIsProgress = false;
        
        _monetsCountChanged.Subscribe(_ =>
        {
            saveData.Monets = GetMonetsCount;
        });
        _heartsCountChanged.Subscribe(_ =>
        {
            saveData.Hearts = GetHeartsCount;
        });
    }

    public void Shutdown()
    {
        TryAbortGraduallyResourceManipulation();
    }
    public void AddCash(int value, bool immediately = true)
    {
        if (value > 0)
        {
            _lastMonetAddValue = value;
            _previousMonetValue = _monets;
            _monets += value;
            if (immediately == false)
            {
                AddGradually(_monetsCountChanged, _previousMonetValue, value).Forget();
            }
            _monetsCountChanged.Execute(_monets);
        }
    }
    public void AddHearts(int value, bool immediately = true)
    {
        if (value > 0)
        {
            _lastHeartsAddValue = value;
            _previousHeartsValue = _hearts;
            _hearts += value;
            if (immediately == false)
            {
                AddGradually(_heartsCountChanged, _previousHeartsValue, value).Forget();
            }
            _heartsCountChanged.Execute(_hearts);
        }
    }
    public bool CashAvailable(int value)
    {
        if (value >= 0 && _monets >= value)
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
        if (value >= 0 && _hearts >= value)
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
            _previousMonetValue = _monets;
            _monets -= value;
            if (immediately == false)
            {
                RemoveGradually(_monetsCountChanged, _previousMonetValue, value).Forget();
            }
            _monetsCountChanged.Execute(_monets);
            return (true, _monets);
        }
        else
        {
            return (false, _monets);
        }
    }
    public (bool, int) RemoveHearts(int value, bool immediately = true)
    {
        if (HeartsAvailable(value) == true)
        {
            _lastHeartsRemoveValue = value;
            _previousHeartsValue = _hearts;
            _hearts -= value;
            if (immediately == false)
            {
                RemoveGradually(_heartsCountChanged, _previousHeartsValue, value).Forget();
            }
            _heartsCountChanged.Execute(_hearts);
            return (true, _hearts);
        }
        else
        {
            return (false, _hearts);
        }
    }

    private async UniTask AddGradually(ReactiveCommand<int> command, int previousMonetValue, int value)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _addResourceGraduallyIsProgress = true;
        int graduallyValue = previousMonetValue;
        for (int i = 0; i < value; i++)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_delayAddValue), cancellationToken: _cancellationTokenSource.Token);
            
            command.Execute(graduallyValue += _graduallyValue);
        }
        _addResourceGraduallyIsProgress = false;
    }
    private async UniTask RemoveGradually(ReactiveCommand<int> command, int previousMonetValue, int value)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _removeResourceGraduallyIsProgress = true;
        int graduallyValue = previousMonetValue;
        for (int i = 0; i < value; i++)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_delayAddValue), cancellationToken: _cancellationTokenSource.Token);
            command.Execute(graduallyValue -= _graduallyValue);
        }
        _removeResourceGraduallyIsProgress = false;
    }

    private void TryAbortGraduallyResourceManipulation()
    {
        _cancellationTokenSource?.Cancel();

        if (_addResourceGraduallyIsProgress == true)
        {
            _monets = _previousMonetValue + _lastMonetAddValue;
            _hearts = _previousHeartsValue + _lastHeartsAddValue;
            _addResourceGraduallyIsProgress = false;
        }

        if (_removeResourceGraduallyIsProgress == true)
        {
            _monets = _previousMonetValue - _lastMonetRemoveValue;
            _hearts = _previousHeartsValue - _lastHeartsRemoveValue;
            _removeResourceGraduallyIsProgress = false;
        }
    }
}