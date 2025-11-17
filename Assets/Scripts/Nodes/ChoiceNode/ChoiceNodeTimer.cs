using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ChoiceNodeTimer
{
    private const string _separator = ":";
    private const int _tenSeconds = 10;
    private const int _zeroSeconds = 0;
    private const int _countSecondsInMinute = 60;
    private const float _minValue = 0f;
    private const float _maxValue = 1f;
    private const float _halfValue = 0.5f;
    private const float _duration = 3f;
    private const float _showEndValue = 0.85f;
    private const float _scaleEndValue = 1.25f;
    private readonly TextMeshProUGUI _timerPanelText;
    private readonly CanvasGroup _timerPanelCanvasGroup;
    private readonly RectTransform _timerImageRectTransform;
    private int _currentTime;
    private int _currentMinutes;
    private int _currentSeconds;
    private string _minutesString;
    private bool _timerCanBeOn;
    public ChoiceNodeTimer(TextMeshProUGUI timerPanelText, CanvasGroup timerPanelCanvasGroup, RectTransform timerImageRectTransform)
    {
        _timerPanelText = timerPanelText;
        _timerPanelCanvasGroup = timerPanelCanvasGroup;
        _timerImageRectTransform = timerImageRectTransform;
    }

    public void TrySetTimerValue(int time = 0)
    {
        if (time > 0)
        {
            _timerPanelCanvasGroup.gameObject.SetActive(true);
            _timerPanelCanvasGroup.alpha = _maxValue;
            _timerCanBeOn = true;
            _currentTime = time;
            SetTime();
        }
        else
        {
            _timerPanelCanvasGroup.gameObject.SetActive(false);
            _timerPanelCanvasGroup.alpha = _minValue;
            _timerCanBeOn = false;
        }
    }
    public async UniTaskVoid TryShowTimerPanelAnim(CancellationToken cancellationToken)
    {
        if (_timerCanBeOn == true)
        {
            _timerPanelCanvasGroup.alpha = _minValue;
            _timerPanelCanvasGroup.DOFade(_maxValue, _halfValue).WithCancellation(cancellationToken);
            await _timerImageRectTransform.DOScale(_showEndValue, _duration * _halfValue).WithCancellation(cancellationToken);
            _timerImageRectTransform.DOScale(_scaleEndValue, _duration).SetLoops(-1, LoopType.Yoyo).WithCancellation(cancellationToken);
        }
    }

    public void TryHideTimerPanelAnim(CancellationToken cancellationToken)
    {
        if (_timerCanBeOn == true)
        {
            _timerPanelCanvasGroup.DOFade(_minValue, _halfValue).WithCancellation(cancellationToken);
            _timerPanelCanvasGroup.gameObject.SetActive(false);
        }
    }
    public async UniTaskVoid TryStartTimer(ChoiceResultEvent<ChoiceCase> choiceResultEvent, Action operation, ChoiceCase choiceCaseResult, CancellationToken cancellationToken)
    {
        if (_timerCanBeOn)
        {
            while (_currentTime > 0)
            {
                SetTime();
                await UniTask.Delay(TimeSpan.FromSeconds(_maxValue), cancellationToken: cancellationToken);
                _currentTime--;

            }

            if (_currentTime == 0)
            {
                operation?.Invoke();
                choiceResultEvent.Execute(choiceCaseResult);
            }
        }
    }

    private void SetTime()
    {
        _currentSeconds = _currentTime % _countSecondsInMinute;
        if (_currentSeconds > _zeroSeconds)
        {
            _currentMinutes = _currentTime / _countSecondsInMinute;
        }

        if (_currentSeconds < _tenSeconds)
        {
            _minutesString = $"{_zeroSeconds}{_currentSeconds}";
        }
        else
        {
            _minutesString = _currentSeconds.ToString();
        }
        _timerPanelText.text = $"{_currentMinutes} {_separator} {_minutesString}";
    }
}