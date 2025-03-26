using TMPro;
using UnityEngine;

public class CharacterPanelUI : MonoBehaviour
{
    [SerializeField] private RectTransform _imageTransform;
    [SerializeField] private RectTransform _panelTransform;
    [SerializeField] private TextMeshProUGUI _nameTextComponent;
    [SerializeField] private TextMeshProUGUI _talkTextComponent;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _durationAnim = 0.2f;
    public RectTransform ImageTransform => _imageTransform;
    public RectTransform PanelTransform => _panelTransform;
    public TextMeshProUGUI NameTextComponent => _nameTextComponent;
    public TextMeshProUGUI TalkTextComponent => _talkTextComponent;
    public CanvasGroup CanvasGroup => _canvasGroup;
    public float DurationAnim => _durationAnim;
}