using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayStoryPanel : StoryPanel
{
    [SerializeField] private Button _wardrobeButton;
    [SerializeField] private Button _resetProgressButton;
    [SerializeField] private Button _likeButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private TextMeshProUGUI _textSeria;
    [SerializeField] private TextMeshProUGUI _progressText;
    [SerializeField] private TextMeshProUGUI _playButtonText;
    [SerializeField] private RectTransform _progressPanel;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _likeImage;
    [SerializeField] private int _hierarchyIndex;
    [SerializeField] private float _hideScaleValue;
    
    public Button WardrobeButton => _wardrobeButton;
    public Button ResetProgressButton => _resetProgressButton;
    public Button LikeButton => _likeButton;
    public Button ExitButton => _exitButton;
    public TextMeshProUGUI TextSeria => _textSeria;
    public TextMeshProUGUI ProgressText => _progressText;
    public TextMeshProUGUI PlayButtonText => _playButtonText;
    public RectTransform ProgressPanel => _progressPanel;
    public CanvasGroup CanvasGroup => _canvasGroup;
    public Image LikeImage => _likeImage;
    public int HierarchyIndex => _hierarchyIndex;
    public float HideScaleValue => _hideScaleValue;
}