using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ConfirmedPanelView : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private TextMeshProUGUI _textLabel;
    [SerializeField] private TextMeshProUGUI _textTranscription;
    [SerializeField] private TextMeshProUGUI _textButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _confirmedButton;
    
    public RectTransform RectTransform => _rectTransform;
    public TextMeshProUGUI TextLabel => _textLabel;
    public TextMeshProUGUI TextTranscription => _textTranscription;
    public TextMeshProUGUI TextButton => _textButton;
    public Button ExitButton => _exitButton;
    public Button ConfirmedButton => _confirmedButton;
}