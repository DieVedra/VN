using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
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

    public TopPanelHandler(IReadOnlyList<Image> signalIndicatorImage, TextMeshProUGUI timeText, TextMeshProUGUI butteryText,
        Image butteryImage, Image butteryIndicatorImage)
    {
        _signalIndicatorImage = signalIndicatorImage;
        _timeText = timeText;
        _butteryText = butteryText;
        _butteryImage = butteryImage;
        _butteryIndicatorImage = butteryIndicatorImage;
        _phoneTime = new PhoneTime();
    }

    public void Init(Color color, int startHour, int startMinute, int data, int butteryPercent = 85)
    {
        for (int i = 0; i < _signalIndicatorImage.Count; i++)
        {
            _signalIndicatorImage[i].color = color;
        }

        _butteryImage.fillAmount = butteryPercent;
        _butteryText.text = $"{_butteryText}{_percentSymbol}";
        _phoneTime.Start(_timeText, startHour, startMinute, data).Forget();
    }
}