using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContactsScreenView : BaseScreen
{
    [SerializeField] private TextMeshProUGUI _textFind;
    [SerializeField] private TextMeshProUGUI _textCalls;
    [SerializeField] private TextMeshProUGUI _textExit;
    [SerializeField] private TextMeshProUGUI _textContacts;
    [SerializeField] private RectTransform _contactsTransform;
    [SerializeField] private Button _buttonExit;
    
    public Color ColorTopPanel => TopPanelColor;
    public Image ImageBackground =>  BackgroundImage;
    public TextMeshProUGUI TextFind => _textFind;
    public TextMeshProUGUI TextCalls => _textCalls;
    public TextMeshProUGUI TextExit => _textExit;
    public TextMeshProUGUI TextContacts => _textContacts;
    public RectTransform ContactsTransform => _contactsTransform;
    public Button ButtonExit => _buttonExit;
}