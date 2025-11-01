using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class StatViewHandler
{
    private const char _plus = '+';
    private const char _minus = '-';
    private const char _space = ' ';
    private readonly float _duration;
    private readonly CanvasGroup _statPanelCanvasGroup;
    private readonly TextMeshProUGUI _statPanelText;
    private CancellationTokenSource _cancellationTokenSource;
    private StringBuilder _stringBuilder = new StringBuilder();
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

    public CustomizationStat GetStatInfo(List<CustomizationStat> gameStats)
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

    public string CreateLabel(CustomizationStat stat)
    {
        _stringBuilder.Clear();
        if (stat.Value > 0)
        {
            _stringBuilder.Append(_plus);
        }
        else
        {
            _stringBuilder.Append(_minus);
        }
        _stringBuilder.Append(stat.Value);
        _stringBuilder.Append(_space);
        _stringBuilder.Append(stat.NameText);

        return _stringBuilder.ToString();
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