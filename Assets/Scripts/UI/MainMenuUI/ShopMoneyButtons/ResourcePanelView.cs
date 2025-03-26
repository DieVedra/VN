using TMPro;
using UnityEngine;

public class ResourcePanelView : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _text;
    
    public TextMeshProUGUI Text => _text;
}