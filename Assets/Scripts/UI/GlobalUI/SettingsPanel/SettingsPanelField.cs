using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SettingsPanelField : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Toggle _toggle;
    public TextMeshProUGUI Text => _text;
    public Toggle Toggle => _toggle;
}