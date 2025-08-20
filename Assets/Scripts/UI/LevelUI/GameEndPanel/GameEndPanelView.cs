using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameEndPanelView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textLabel;
    [SerializeField] private TextMeshProUGUI _textDescription;
    [SerializeField] private TextMeshProUGUI _textButton;
    [SerializeField] private Button _buttonBackToMenu;
    
    public TextMeshProUGUI TextLabel => _textLabel;
    public TextMeshProUGUI TextDescription => _textDescription;
    public TextMeshProUGUI TextButton => _textButton;
    public Button ButtonBackToMenu => _buttonBackToMenu;
}