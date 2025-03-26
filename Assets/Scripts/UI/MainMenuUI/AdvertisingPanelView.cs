using UnityEngine;
using UnityEngine.UI;


public class AdvertisingPanelView : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Button _buttonExit;
    [SerializeField] private int _monetReward;
    [SerializeField] private int _heartsReward;
    
    public RectTransform RectTransform => _rectTransform;
    public Button ButtonExit => _buttonExit;
    public int MonetReward => _monetReward;
    public int HeartsReward => _heartsReward;
}