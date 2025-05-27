
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanelChoiceHandler
{
    private const int _minIndex = 0;
    private readonly ILocalizationChanger _localizationChanger;
    private readonly Button _leftButton;
    private readonly Button _rightButton;
    private readonly TextMeshProUGUI _textChoice;

    public SettingPanelChoiceHandler(ILocalizationChanger localizationChanger, Button leftButton, Button rightButton, TextMeshProUGUI textChoice)
    {
        _localizationChanger = localizationChanger;
        _leftButton = leftButton;
        _rightButton = rightButton;
        _textChoice = textChoice;
        
        _leftButton.onClick.AddListener(MoveLeft);
        _rightButton.onClick.AddListener(MoveRight);
        SetTextAndExecute();
    }

    private void MoveLeft()
    {
        int index = _localizationChanger.CurrentLanguageKeyIndex.Value;
        if (index == _minIndex)
        {
            _localizationChanger.CurrentLanguageKeyIndex.Value = _localizationChanger.GetMyLanguageNamesCount - 1;
        }
        else
        {
            _localizationChanger.CurrentLanguageKeyIndex.Value = --index;
        }
        SetTextAndExecute();
    }

    private void MoveRight()
    {
        int index = _localizationChanger.CurrentLanguageKeyIndex.Value;
        if (index == _localizationChanger.GetMyLanguageNamesCount - 1)
        {
            _localizationChanger.CurrentLanguageKeyIndex.Value = _minIndex;
        }
        else
        {
            _localizationChanger.CurrentLanguageKeyIndex.Value = ++index;
        }
        SetTextAndExecute();
    }
    private void SetTextAndExecute()
    {
        _textChoice.text = _localizationChanger.GetName;
    }
}