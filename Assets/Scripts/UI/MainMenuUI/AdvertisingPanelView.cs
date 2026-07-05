using UnityEngine;
using UnityEngine.UI;


public class AdvertisingPanelView : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Button _buttonExit;
    
    public RectTransform RectTransform => _rectTransform;
    public Button ButtonExit => _buttonExit;
}