
using UnityEngine;

public class LevelUIView : MonoBehaviour
{
    [SerializeField] private NarrativePanelUI _narrativePanelUI;
    [SerializeField] private NotificationPanelUI _notificationPanelUI;
    [SerializeField] private CharacterPanelUI _characterPanelUI;
    // [SerializeField] private CurtainUI _curtainUI;
    [SerializeField] private ChoicePanelUI _choicePanelUI;
    [SerializeField] private ButtonSwitchSlideUI buttonSwitchSlideUI;
    [SerializeField] private CustomizationCharacterPanelUI _customizationCharacterPanelUI;
    [SerializeField] private HeaderSeriesPanelUI _headerSeriesPanelUI;
    [SerializeField] private GameControlPanelView _gameControlPanelView;
    [SerializeField] private RectTransform _monetPanel;
    public NarrativePanelUI NarrativePanelUI => _narrativePanelUI;
    public NotificationPanelUI NotificationPanelUI => _notificationPanelUI;
    public CharacterPanelUI CharacterPanelUI => _characterPanelUI;
    // public CurtainUI CurtainUI => _curtainUI;
    public ChoicePanelUI ChoicePanelUI => _choicePanelUI;
    public ButtonSwitchSlideUI ButtonSwitchSlideUI => buttonSwitchSlideUI;
    public CustomizationCharacterPanelUI CustomizationCharacterPanelUI => _customizationCharacterPanelUI;
    public HeaderSeriesPanelUI HeaderSeriesPanelUI => _headerSeriesPanelUI;
    public GameControlPanelView GameControlPanelView => _gameControlPanelView;
    public RectTransform MonetPanel => _monetPanel;
}