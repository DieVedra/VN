
using UnityEngine;

public class MainMenuUIView : MonoBehaviour
{
    [SerializeField] private MyScrollUIView _myScrollUIView;
    [SerializeField] private SettingsButtonView _settingsButtonView;
    [SerializeField] private ResourcePanelButtonView _monetPanelView;
    [SerializeField] private ResourcePanelButtonView _heartsPanelView;
    [SerializeField] private BottomPanelView _bottomPanelView;
    public MyScrollUIView MyScrollUIView => _myScrollUIView;
    public SettingsButtonView SettingsButtonView => _settingsButtonView;
    public ResourcePanelButtonView MonetPanelView => _monetPanelView;
    public ResourcePanelButtonView HeartsPanelView => _heartsPanelView;
    public BottomPanelView BottomPanelView => _bottomPanelView;
}