using UnityEngine;
using UnityEngine.UI;

public class GameControlPanelView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Button _buttonShowPanel;
    [SerializeField] private SettingsButtonView _settingsButtonView;
    [SerializeField] private Button _buttonGoToMainMenu;
    [SerializeField] private ResourcePanelView _shopMoneyButtonView;
    [SerializeField] private Button _buttonWardrobe;

    public CanvasGroup CanvasGroup => _canvasGroup;
    public Button ButtonShowPanel => _buttonShowPanel;
    public SettingsButtonView SettingsButtonView => _settingsButtonView;
    public Button ButtonGoToMainMenu => _buttonGoToMainMenu;
    public ResourcePanelView ShopMoneyButtonView => _shopMoneyButtonView;
    public Button ButtonWardrobe => _buttonWardrobe;
}