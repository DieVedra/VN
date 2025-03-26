using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class StatViewHandler
{
    private readonly float _duration;
    private readonly CanvasGroup _statPanelCanvasGroup;
    private readonly TextMeshProUGUI _statPanelText;
    private CancellationTokenSource _cancellationTokenSource;
    private bool _isShowed;

    public bool IsShowed => _isShowed;
    public StatViewHandler(CanvasGroup statPanelCanvasGroup, TextMeshProUGUI statPanelText, float duration)
    {
        _statPanelCanvasGroup = statPanelCanvasGroup;
        _statPanelText = statPanelText;
        _duration = duration;
        if (statPanelCanvasGroup.gameObject.activeSelf == true)
        {
            PanelOff();
        }
        else
        {
            _isShowed = false;
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
    }

    public bool CheckViewStatToShow(ICustomizationSettings customizationSettings) // проверяет добавлено что то в статы или они пустые
    {
        bool result = false;
        for (int i = 0; i < customizationSettings.GameStats.Count; i++)
        {
            if (customizationSettings.GameStats[i].ShowKey == true)
            {
                result = true;
                break;
            }
        }

        return result;
    }

    public Stat GetStatInfo(List<Stat> gameStats)
    {
        int index = 0;
        for (int i = 0; i < gameStats.Count; i++)
        {
            if (gameStats[i].ShowKey == true && gameStats[i].Value != 0)
            {
                index = i;
            }
        }
        return gameStats[index];
    }

    public string CreateLabel(Stat stat)
    {
        string label;
        if (stat.Value > 0)
        {
            label = $"+{stat.Value} {stat.Name}";
        }
        else
        {
            label = $"-{stat.Value} {stat.Name}";
        }
        return label;
    }
    public async UniTask HideToShowAnim(Color color, string text)
    {
        await HideAnim();
        await ShowAnim(color, text);
    }
    public async UniTask HideAnim()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        await _statPanelCanvasGroup.DOFade(0f, _duration).WithCancellation(_cancellationTokenSource.Token);
        PanelOff();
    }

    public async UniTask ShowAnim(Color color, string text)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        PanelOn(color, text);
        _statPanelCanvasGroup.alpha = 0f;
        await _statPanelCanvasGroup.DOFade(1f, _duration).WithCancellation(_cancellationTokenSource.Token);
    }

    public void Show(Color color, string text)
    {
        PanelOn(color, text);
        _statPanelCanvasGroup.alpha = 1f;
    }

    public void Hide()
    {
        _statPanelCanvasGroup.alpha = 0f;
        PanelOff();
    }
    private void PanelOff()
    {
        _statPanelCanvasGroup.gameObject.SetActive(false);
        _isShowed = false;
    }

    private void PanelOn(Color color, string text)
    {
        _statPanelCanvasGroup.gameObject.SetActive(true);
        _isShowed = true;
        _statPanelText.text = text;
        _statPanelText.color = color;
    }
}