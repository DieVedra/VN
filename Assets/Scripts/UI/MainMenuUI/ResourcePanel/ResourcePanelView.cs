using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePanelView : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _text;
    [SerializeField] protected Image _icon;
    [SerializeField] protected Image _panel;
    [SerializeField] protected Transform _parent;
    [SerializeField] protected Button _button;
    [SerializeField] protected CanvasGroup _canvasGroup;
    [SerializeField] protected AnimationCurve _curveWithAddButton;
    [SerializeField] protected AnimationCurve _curveWithoutAddButton;

    public TextMeshProUGUI Text => _text;
    public Image Icon => _icon;
    public Image Panel => _panel;
    public Transform Parent => _parent;
    public Button Button => _button;
    public CanvasGroup CanvasGroup => _canvasGroup;
    public AnimationCurve CurveWithAddButton => _curveWithAddButton;
    public AnimationCurve CurveWithoutAddButton => _curveWithoutAddButton;
}