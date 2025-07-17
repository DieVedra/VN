
using UnityEngine;

public class LevelUIView : MonoBehaviour
{
    [SerializeField] private NarrativePanelUI _narrativePanelUI;
    [SerializeField] private NotificationPanelUI _notificationPanelUI;
    [SerializeField] private CharacterPanelUI _characterPanelUI;
    [SerializeField] private ChoicePanelUI _choicePanelUI;
    [SerializeField] private ButtonSwitchSlideUI buttonSwitchSlideUI;
    [SerializeField] private CustomizationCharacterPanelUI _customizationCharacterPanelUI;
    [SerializeField] private HeaderSeriesPanelUI _headerSeriesPanelUI;
    [SerializeField] private GameControlPanelView _gameControlPanelView;
    [SerializeField] private ResourcePanelWithCanvasGroupView _monetPanel;
    [SerializeField] private ResourcePanelWithCanvasGroupView _heartsPanel;
    public NarrativePanelUI NarrativePanelUI => _narrativePanelUI;
    public NotificationPanelUI NotificationPanelUI => _notificationPanelUI;
    public CharacterPanelUI CharacterPanelUI => _characterPanelUI;
    public ChoicePanelUI ChoicePanelUI => _choicePanelUI;
    public ButtonSwitchSlideUI ButtonSwitchSlideUI => buttonSwitchSlideUI;
    public CustomizationCharacterPanelUI CustomizationCharacterPanelUI => _customizationCharacterPanelUI;
    public HeaderSeriesPanelUI HeaderSeriesPanelUI => _headerSeriesPanelUI;
    public GameControlPanelView GameControlPanelView => _gameControlPanelView;
    public ResourcePanelWithCanvasGroupView MonetPanel => _monetPanel;
    public ResourcePanelWithCanvasGroupView HeartsPanel => _heartsPanel;
}