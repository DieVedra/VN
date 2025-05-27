using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryPanel : MonoBehaviour
{
    [SerializeField] protected Image _imageBackground;
    [SerializeField] private Image _imageEffectChange;
    [SerializeField] protected Image _imageLabel;
    [SerializeField] protected TextMeshProUGUI _textDescription;
    [SerializeField] protected TextMeshProUGUI _textButtonOpen;
    [SerializeField] protected TextMeshProUGUI _textButtonContinue;
    [SerializeField] protected Button _buttonOpen;
    [SerializeField] protected Button _buttonContinue;
    public Image ImageEffectChange => _imageEffectChange;
    public TextMeshProUGUI TextDescription => _textDescription;
    public TextMeshProUGUI TextButtonOpen => _textButtonOpen;
    public TextMeshProUGUI TextButtonContinue => _textButtonContinue;
    public Button ButtonOpen => _buttonOpen;
    public Button ButtonContinue => _buttonContinue;


    public void Construct(Story story)
    {
        _imageBackground.sprite = story.SpriteStorySkin;
        _imageLabel.sprite = story.SpriteLogo;
        _textDescription.text = story.Description;
    }
}