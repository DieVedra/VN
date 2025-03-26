using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BottomPanelView : MonoBehaviour
{
    [SerializeField] private Button _gameExitButton;
    [SerializeField] private Button _showAdvertisingButton;

    [SerializeField] private TextMeshProUGUI _gameExitButtonText;
    [SerializeField] private TextMeshProUGUI _showAdvertisingButtonText;
    
    public Button GameExitButton => _gameExitButton;
    public Button ShowAdvertisingButton => _showAdvertisingButton;
    
    public TextMeshProUGUI GameExitButtonText => _gameExitButtonText;
    public TextMeshProUGUI ShowAdvertisingButtonText => _showAdvertisingButtonText;
}