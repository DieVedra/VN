using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class TopPanelHandler
{
    private const string _percentSymbol = "%";
    private IReadOnlyList<Image> _signalIndicatorImage;
    private TextMeshProUGUI _timeText;
    private TextMeshProUGUI _butteryText;
    private Image _butteryImage;
    private Image _butteryIndicatorImage;
    private PhoneTime _phoneTime;
    private CompositeDisposable _compositeDisposable;

    public TopPanelHandler(IReadOnlyList<Image> signalIndicatorImage, TextMeshProUGUI timeText, TextMeshProUGUI butteryText,
        Image butteryImage, Image butteryIndicatorImage)
    {
        _signalIndicatorImage = signalIndicatorImage;
        _timeText = timeText;
        _butteryText = butteryText;
        _butteryImage = butteryImage;
        _butteryIndicatorImage = butteryIndicatorImage;
    }

    public void Init(Color color, PhoneTime phoneTime, int butteryPercent = 85)
    {
        _compositeDisposable = new CompositeDisposable();
        for (int i = 0; i < _signalIndicatorImage.Count; i++)
        {
            _signalIndicatorImage[i].color = color;
        }

        _butteryImage.fillAmount = butteryPercent;
        _butteryText.text = $"{_butteryText}{_percentSymbol}";
        Observable.EveryUpdate().Subscribe(_ =>
        {
            _timeText.text = _phoneTime.GetCurrentTime();
        }).AddTo(_compositeDisposable);

        _phoneTime = phoneTime;
    }

    public void Dispose()
    {
        _compositeDisposable?.Clear();
    }
}