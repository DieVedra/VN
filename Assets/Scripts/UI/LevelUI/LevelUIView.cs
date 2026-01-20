
using UnityEngine;

public class LevelUIView : MonoBehaviour
{
    [SerializeField] private PhoneUIView _phoneUIView;
    [SerializeField] private NarrativePanelUI _narrativePanelUI;
    [SerializeField] private NotificationPanelUI _notificationPanelUI;
    [SerializeField] private CharacterPanelUI _characterPanelUI;
    [SerializeField] private ChoicePanelUI _choicePanelUI;
    [SerializeField] private ButtonSwitchSlideUI buttonSwitchSlideUI;
    [SerializeField] private CustomizationCharacterPanelUI _customizationCharacterPanelUI;
    [SerializeField] private HeaderSeriesPanelUI _headerSeriesPanelUI;
    [SerializeField] private GameControlPanelView _gameControlPanelView;
    [SerializeField] private RectTransform _monetPanel;
    [SerializeField] private RectTransform _heartsPanel;
    public PhoneUIView PhoneUIView
    {
        get => _phoneUIView;
        set => _phoneUIView = value;
    }
    public NarrativePanelUI NarrativePanelUI => _narrativePanelUI;
    public NotificationPanelUI NotificationPanelUI => _notificationPanelUI;
    public CharacterPanelUI CharacterPanelUI => _characterPanelUI;
    public ChoicePanelUI ChoicePanelUI => _choicePanelUI;
    public ButtonSwitchSlideUI ButtonSwitchSlideUI => buttonSwitchSlideUI;
    public CustomizationCharacterPanelUI CustomizationCharacterPanelUI => _customizationCharacterPanelUI;
    public HeaderSeriesPanelUI HeaderSeriesPanelUI => _headerSeriesPanelUI;
    public GameControlPanelView GameControlPanelView => _gameControlPanelView;
    public RectTransform MonetPanelRectTransform => _monetPanel;
    public RectTransform HeartsPanelRectTransform => _heartsPanel;
}