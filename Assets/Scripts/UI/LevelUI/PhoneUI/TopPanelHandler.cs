using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class TopPanelHandler
{
    private const float _percentMultiplier = 0.01f;
    private const string _percentSymbol = "%";
    private const float _posXSignalIndicatorShowTime = -123f;
    private const float _posXSignalIndicatorHideTime = -188f;
    private readonly RectTransform _signalIndicatorRectTransform;
    private readonly IReadOnlyList<Image> _signalIndicatorImage;
    private readonly TextMeshProUGUI _timeText;
    private readonly TextMeshProUGUI _butteryText;
    private readonly Image _butteryImage;
    private readonly Image _butteryIndicatorImage;
    private CompositeDisposable _compositeDisposable;
    private bool _timeTextInited;
    private Vector2 _posShowTime => new Vector2(_posXSignalIndicatorShowTime, _signalIndicatorRectTransform.anchoredPosition.y);
    private Vector2 _posHideTime => new Vector2(_posXSignalIndicatorHideTime, _signalIndicatorRectTransform.anchoredPosition.y);

    public TopPanelHandler(RectTransform signalIndicatorRectTransform, IReadOnlyList<Image> signalIndicatorImage,
        TextMeshProUGUI timeText, TextMeshProUGUI butteryText, Image butteryImage, Image butteryIndicatorImage)
    {
        _signalIndicatorImage = signalIndicatorImage;
        _signalIndicatorRectTransform = signalIndicatorRectTransform;
        _timeText = timeText;
        _butteryText = butteryText;
        _butteryImage = butteryImage;
        _butteryIndicatorImage = butteryIndicatorImage;
        _timeTextInited = false;
    }

    public void Init(PhoneTime phoneTime, bool playMode, int butteryPercent = 85)
    {
        if (playMode == true & _timeTextInited == false)
        {
            _timeTextInited = true;
            _compositeDisposable = new CompositeDisposable();
            Observable.EveryUpdate().Subscribe(_ =>
            {
                _timeText.text = phoneTime.GetCurrentTime();
            }).AddTo(_compositeDisposable);
        }

        _butteryIndicatorImage.fillAmount = butteryPercent * _percentMultiplier;
        _butteryText.text = $"{butteryPercent}{_percentSymbol}";
        _signalIndicatorRectTransform.parent.gameObject.SetActive(true);
    }

    public void SetColorAndMode(Color color, bool showTimeKey = true)
    {
        for (int i = 0; i < _signalIndicatorImage.Count; i++)
        {
            _signalIndicatorImage[i].color = color;
        }
        _timeText.color = color;
        if (showTimeKey)
        {
            _signalIndicatorRectTransform.anchoredPosition = _posShowTime;
            _timeText.gameObject.SetActive(true);
        }
        else
        {
            _signalIndicatorRectTransform.anchoredPosition = _posHideTime;
            _timeText.gameObject.SetActive(false);
        }
        _butteryImage.color = color;
        _butteryIndicatorImage.color = color;
        _butteryText.color = color;
    }
    public void Shutdown()
    {
        _timeTextInited = false;
        _compositeDisposable?.Clear();
    }
}