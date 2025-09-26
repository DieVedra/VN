using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;

public class PhoneTime
{
    private const string _separ1 = ":";
    private const string _separ2 = ".";
    private const float _delay = 60f;
    private const int _lastMinute = 59;
    private const int _maxMinute = 60;
    private const int _firstMinute = 0;
    private int _currentHour;
    private int _currentMinute;
    private string _currentData;
    private CancellationTokenSource _cancellationTokenSource;
    public bool IsStarted { get; private set; }


    public string GetCurrentTime()
    {
        return $"{_currentHour.ToString()}{_separ1}{_currentMinute.ToString()}";
    }
    public string GetData()
    {
        return $"";
    }
    public async UniTask Start(int startHour, int startMinute, string date, bool playModeKey)
    {
        if (playModeKey)
        {
            if (IsStarted == false)
            {
                _currentData = date;
                IsStarted = true;
                _cancellationTokenSource = new CancellationTokenSource();
                _currentHour = startHour;
                CheckStartMinute(startHour, startMinute, date);
                while (IsStarted)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(_delay), cancellationToken: _cancellationTokenSource.Token);
                    if (_currentMinute == _lastMinute)
                    {
                        _currentMinute = _firstMinute;
                    }
                    else
                    {
                        _currentMinute++;
                    }
                }
            }
        }
        else
        {
            CheckStartMinute(startHour, startMinute, date);
        }
    }

    private void CheckStartMinute(int startHour, int startMinute, string date)
    {
        _currentData = date;
        _currentHour = startHour;

        if (startMinute < _maxMinute && startMinute > _firstMinute)
        {
            _currentMinute = startMinute;
        }
        else
        {
            _currentMinute = _firstMinute;
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