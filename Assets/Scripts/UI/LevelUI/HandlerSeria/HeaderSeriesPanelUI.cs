using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeaderSeriesPanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text1;
    [SerializeField] private TextMeshProUGUI _text2;
    
    public TextMeshProUGUI Text1 => _text1;
    public TextMeshProUGUI Text2 => _text2;
}