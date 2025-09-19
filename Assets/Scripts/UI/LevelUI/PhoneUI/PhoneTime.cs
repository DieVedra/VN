
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
    private int _currentData;
    
    private CancellationTokenSource _cancellationTokenSource;
    private bool _isStarted;
    public string GetCurrentTime()
    {
        return $"{_currentHour.ToString()}{_separ1}{_currentMinute.ToString()}";
    }
    public string GetData()
    {
        return $"";
    }
    public async UniTask Start(TextMeshProUGUI timeText, int startHour, int startMinute, int data)
    {
        if (_isStarted == false)
        {
            _isStarted = true;
            _cancellationTokenSource = new CancellationTokenSource();
            _currentHour = startHour;
            if (startMinute < _maxMinute && startMinute > _firstMinute)
            {
                _currentMinute = startMinute;
            }
            else
            {
                _currentMinute = _firstMinute;
            }
            while (_isStarted)
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
                timeText.text = GetCurrentTime();
            }
        }
    }

    public void Stop()
    {
        if (_isStarted == true)
        {
            _isStarted = false;
            _cancellationTokenSource.Cancel();
        }
    }
}