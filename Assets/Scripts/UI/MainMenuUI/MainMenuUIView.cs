
using UnityEngine;

public class MainMenuUIView : MonoBehaviour
{
    [SerializeField] private MyScroll _myScroll;
    [SerializeField] private SettingsButtonView _settingsButtonView;
    [SerializeField] private ResourcePanelButtonView _monetPanelView;
    [SerializeField] private ResourcePanelButtonView _heartsPanelView;
    [SerializeField] private BottomPanelView _bottomPanelView;
    public MyScroll MyScroll => _myScroll;
    public SettingsButtonView SettingsButtonView => _settingsButtonView;
    public ResourcePanelButtonView MonetPanelView => _monetPanelView;
    public ResourcePanelButtonView HeartsPanelView => _heartsPanelView;
    public BottomPanelView BottomPanelView => _bottomPanelView;
}