
using UnityEngine;

public class MainMenuUIView : MonoBehaviour
{
    [SerializeField] private MyScrollUIView _myScrollUIView;
    [SerializeField] private SettingsButtonView _settingsButtonView;
    [SerializeField] private Transform _monetPanelParent;
    [SerializeField] private Transform _heartsPanelParent;
    [SerializeField] private BottomPanelView _bottomPanelView;


    [SerializeField] private Color _monetPanelColor;
    [SerializeField] private Color _monetPanelButtonColor;
    [SerializeField] private Color _heartsPanelColor;
    [SerializeField] private Color _heartsPanelButtonColor;
    
    
    public MyScrollUIView MyScrollUIView => _myScrollUIView;
    public SettingsButtonView SettingsButtonView => _settingsButtonView;
    public Transform MonetPanelTransform => _monetPanelParent;
    public Transform HeartsPanelTransform => _heartsPanelParent;
    public BottomPanelView BottomPanelView => _bottomPanelView;
    public Color MonetPanelColor => _monetPanelColor;
    public Color MonetPanelButtonColor => _monetPanelButtonColor;
    public Color HeartsPanelColor => _heartsPanelColor;
    public Color HeartsPanelButtonColor => _heartsPanelButtonColor;
}