using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SettingsPanelChoiceField : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;
    [SerializeField] private TextMeshProUGUI _textChoice;


    public TextMeshProUGUI Text => _text;
    public TextMeshProUGUI TextChoice => _textChoice;
    public Button LeftButton => _leftButton;
    public Button RightButton => _rightButton;
}