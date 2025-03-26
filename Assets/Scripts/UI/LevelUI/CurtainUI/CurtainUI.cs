

using UnityEngine;
using UnityEngine.UI;

public class CurtainUI : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private float _durationAnim = 1f;
    private readonly float _unfadeSkipValue = 0.2f;
    
    public Image Image => _image;
    public float DurationAnim => _durationAnim;
    public float UnfadeSkipValue => _unfadeSkipValue;
}