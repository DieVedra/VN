using TMPro;
using UnityEngine;

public class CharacterPanelUIHandler : PanelUIHandler
{
    private const float _oneValue = 1f;
    private const float _zeroValue = 0f;
    private const float _offsetValueX = 200f;
    private const float _offsetValueY = 45f;
    private readonly Vector3 _leftPosition;
    private readonly Vector3 _rightPosition;
    private readonly Vector3 _leftOffset = new Vector3(-_offsetValueX, _offsetValueY, _zeroValue);
    private readonly Vector3 _rightOffset = new Vector3(_offsetValueX, _offsetValueY, _zeroValue);

    private readonly Vector3 _leftScaleImage = new Vector3(-_oneValue,_oneValue,_oneValue);
    private readonly Vector3 _rightScaleImage = Vector3.one;

    private readonly Vector3 _editModeStartScale = Vector3.one;
    private readonly Vector3 _playModeStartScale = Vector3.zero;

    private readonly CharacterPanelUI _characterPanelUI;
    private readonly TextConsistentlyViewer _textConsistentlyViewer;
    private readonly AnimationPanelWithScale _animationPanelWithScale;
    private readonly TextMeshProUGUI _talkTextComponent;
    private readonly RectTransform _nameTextRectTransform;
    private readonly RectTransform _imageRectTransform;
    public TextConsistentlyViewer TextConsistentlyViewer => _textConsistentlyViewer;
    private Vector3 _defaultPosition => _characterPanelUI.PanelTransform.anchoredPosition;
    public AnimationPanelWithScale AnimationPanelWithScale => _animationPanelWithScale;
    public CharacterPanelUIHandler(CharacterPanelUI characterPanelUI)
    {
        _textConsistentlyViewer = new TextConsistentlyViewer(characterPanelUI.TalkTextComponent);
        _characterPanelUI = characterPanelUI;
        _talkTextComponent = _characterPanelUI.TalkTextComponent;
        _leftPosition = _defaultPosition + _leftOffset;
        _rightPosition = _defaultPosition + _rightOffset;
        _animationPanelWithScale = new AnimationPanelWithScale(_characterPanelUI.PanelTransform, _characterPanelUI.CanvasGroup,
            _rightPosition, _leftPosition, _defaultPosition, characterPanelUI.DurationAnim);
        _nameTextRectTransform = _characterPanelUI.NameTextTransform;
        _imageRectTransform = characterPanelUI.ImageTransform;
    }
    public void CharacterTalkInEditMode(CharacterTalkData data)
    {
        SetPanelDirection(data);
        SetLocalizationText(data);
        _characterPanelUI.CanvasGroup.alpha = _oneValue;
        _characterPanelUI.PanelTransform.localScale = _editModeStartScale;
        _characterPanelUI.PanelTransform.anchoredPosition = _defaultPosition;
    }
    
    public void EmergenceCharacterTalkInPlayMode(CharacterTalkData data)
    {
        _characterPanelUI.PanelTransform.localScale = _playModeStartScale;
        if (SetPanelDirection(data))
        {
            _characterPanelUI.PanelTransform.anchoredPosition = _rightPosition;
        }
        else
        {
            _characterPanelUI.PanelTransform.anchoredPosition = _leftPosition;
        }
        SetLocalizationText(data);
        _characterPanelUI.CanvasGroup.alpha = _zeroValue;
        _characterPanelUI.PanelTransform.localScale = _editModeStartScale;
        _characterPanelUI.PanelTransform.anchoredPosition = _defaultPosition;
    }

    public void DisappearanceCharacterTalkInPlayMode()
    {
        _characterPanelUI.PanelTransform.anchoredPosition = _defaultPosition;
        _characterPanelUI.gameObject.SetActive(false);
        _characterPanelUI.CanvasGroup.alpha = _zeroValue;
    }
    public void SetLocalizationText(CharacterTalkData data)
    {
        _characterPanelUI.NameTextComponent.text = data.Name;
        _characterPanelUI.TalkTextComponent.text = data.TalkText;
        ResizePanel();
    }
    private bool SetPanelDirection(CharacterTalkData data)
    {
        _characterPanelUI.gameObject.SetActive(true);
        if (data.DirectionType == DirectionType.Right)
        {
            _characterPanelUI.ImageTransform.localScale = _rightScaleImage;
            _nameTextRectTransform.localScale = _rightScaleImage;
            _characterPanelUI.TalkTextTransform.localScale = _rightScaleImage;
            _characterPanelUI.NameTextComponent.alignment = TextAlignmentOptions.Right;
            return true;
        }
        else
        {
            _characterPanelUI.ImageTransform.localScale = _leftScaleImage;
            _nameTextRectTransform.localScale = _leftScaleImage;
            _characterPanelUI.TalkTextTransform.localScale = _leftScaleImage;
            _characterPanelUI.NameTextComponent.alignment = TextAlignmentOptions.Left;
            return false;
        }
    }

    private void ResizePanel()
    {
        _talkTextComponent.ForceMeshUpdate();
        Size = _talkTextComponent.GetRenderedValues(true);
        Size.x = _imageRectTransform.sizeDelta.x;
        Size.y = Size.y + _characterPanelUI.HeightOffset * Multiplier;
        if (Size.y <= _characterPanelUI.ImageHeightDefault)
        {
            Size.y = _characterPanelUI.ImageHeightDefault;
        }
        _imageRectTransform.sizeDelta = Size;
    }
}