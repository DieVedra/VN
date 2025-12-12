using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockScreenView : BaseScreen
{
    [SerializeField] private TextMeshProUGUI _time;
    [SerializeField] private TextMeshProUGUI _data;
    [SerializeField] private Button _blockScreenButton;
    
    public Image ImageBackground =>  BackgroundImage;
    public TextMeshProUGUI Time => _time;
    public TextMeshProUGUI Data => _data;
    public Button BlockScreenButton => _blockScreenButton;
}