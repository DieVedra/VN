using System;
using System.Threading;
using Cysharp.Threading.Tasks;

public class PhoneTime
{
    private const string _separ1 = ":";
    private const float _delay = 60f;
    private const int _lastMinute = 59;
    private const int _maxMinute = 60;
    private const int _firstMinute = 0;
    private CancellationTokenSource _cancellationTokenSource;
    private int _currentHour;
    public int CurrentMinute { get; private set; }
    public bool IsStarted { get; private set; }
    
    public string GetCurrentTime()
    {
        return $"{_currentHour.ToString()}{_separ1}{CurrentMinute.ToString()}";
    }

    public void Restart(int startMinute)
    {
        Start(_currentHour, startMinute, true).Forget();
    }
    public async UniTask Start(int startHour, int startMinute, bool playModeKey)
    {
        if (playModeKey)
        {
            if (IsStarted == false)
            {
                IsStarted = true;
                _cancellationTokenSource = new CancellationTokenSource();
                _currentHour = startHour;
                CheckStartMinute(startHour, startMinute);
                while (IsStarted)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(_delay), cancellationToken: _cancellationTokenSource.Token);
                    if (CurrentMinute == _lastMinute)
                    {
                        CurrentMinute = _firstMinute;
                    }
                    else
                    {
                        CurrentMinute++;
                    }
                }
            }
        }
        else
        {
            CheckStartMinute(startHour, startMinute);
        }
    }

    private void CheckStartMinute(int startHour, int startMinute)
    {
        _currentHour = startHour;

        if (startMinute < _maxMinute && startMinute > _firstMinute)
        {
            CurrentMinute = startMinute;
        }
        else
        {
            CurrentMinute = _firstMinute;
        }
    }

    public void Stop()
    {
        if (IsStarted == true)
        {
            IsStarted = false;
            _cancellationTokenSource.Cancel();
        }
    }
}