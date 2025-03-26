using UnityEngine;
using UnityEngine.UI;

public class GameControlPanelView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Button _buttonShowPanel;
    [SerializeField] private Button _buttonSettings;
    [SerializeField] private Button _buttonGoToMainMenu;
    [SerializeField] private Button _buttonShop;
    [SerializeField] private Button _buttonWardrobe;

    public CanvasGroup CanvasGroup => _canvasGroup;
    public Button ButtonShowPanel => _buttonShowPanel;
    public Button ButtonSettings => _buttonSettings;
    public Button ButtonGoToMainMenu => _buttonGoToMainMenu;
    public Button ButtonShop => _buttonShop;
    public Button ButtonWardrobe => _buttonWardrobe;
}