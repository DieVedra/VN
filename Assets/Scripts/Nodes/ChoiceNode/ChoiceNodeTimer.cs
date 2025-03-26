
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ChoiceNodeTimer
{
    private readonly string _separator = ":";
    private readonly int _countSecondsInMinute = 60;
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

    public void TrySetTimerValue(int time)
    {
        if (time > 0)
        {
            _timerPanelCanvasGroup.gameObject.SetActive(true);
            _timerPanelCanvasGroup.alpha = 1f;
            _timerCanBeOn = true;
            _currentTime = time;
            SetTime();
        }
        else
        {
            _timerPanelCanvasGroup.gameObject.SetActive(false);
            _timerPanelCanvasGroup.alpha = 0f;
            _timerCanBeOn = false;
        }
    }
    public async UniTaskVoid TryShowTimerPanelAnim(CancellationToken cancellationToken)
    {
        if (_timerCanBeOn == true)
        {
            _timerPanelCanvasGroup.alpha = 0f;
            _timerPanelCanvasGroup.DOFade(1f, 0.5f).WithCancellation(cancellationToken);
            await _timerImageRectTransform.DOScale(0.85f, 3f * 0.5f).WithCancellation(cancellationToken);
            
            _timerImageRectTransform.DOScale(1.25f, 3f).SetLoops(-1, LoopType.Yoyo).WithCancellation(cancellationToken);
        }
    }

    public void TryHideTimerPanelAnim(CancellationToken cancellationToken)
    {
        if (_timerCanBeOn == true)
        {
            _timerPanelCanvasGroup.DOFade(0f, 0.5f).WithCancellation(cancellationToken);
            _timerPanelCanvasGroup.gameObject.SetActive(false);
        }
    }
    public async UniTaskVoid TryStartTimer(ChoiceResultEvent choiceResultEvent, Action operation, int index, CancellationToken cancellationToken)
    {
        if (_timerCanBeOn)
        {
            while (_currentTime > 0)
            {
                SetTime();
                await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: cancellationToken);
                _currentTime--;

            }

            if (_currentTime == 0)
            {
                operation?.Invoke();
                choiceResultEvent.Execute(index);
            }
        }
    }

    private void SetTime()
    {
        _currentSeconds = _currentTime % _countSecondsInMinute;
        if (_currentSeconds > 0)
        {
            _currentMinutes = _currentTime / _countSecondsInMinute;
        }

        if (_currentSeconds < 10)
        {
            _minutesString = $"0{_currentSeconds}";
        }
        else
        {
            _minutesString = _currentSeconds.ToString();
        }
        _timerPanelText.text = $"{_currentMinutes} {_separator} {_minutesString}";
    }
}