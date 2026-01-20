using UnityEngine;

public class MainMenuUIView : MonoBehaviour
{
    [SerializeField] private MyScrollUIView _myScrollUIView;
    [SerializeField] private SettingsButtonView _settingsButtonView;
    [SerializeField] private Transform _monetPanelParent;
    [SerializeField] private Transform _heartsPanelParent;
    [SerializeField] private BottomPanelView _bottomPanelView;
    
    public MyScrollUIView MyScrollUIView => _myScrollUIView;
    public SettingsButtonView SettingsButtonView => _settingsButtonView;
    public Transform MonetPanelTransform => _monetPanelParent;
    public Transform HeartsPanelTransform => _heartsPanelParent;
    public BottomPanelView BottomPanelView => _bottomPanelView;
}