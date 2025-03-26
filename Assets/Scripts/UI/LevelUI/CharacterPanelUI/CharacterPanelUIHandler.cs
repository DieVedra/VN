
using TMPro;
using UnityEngine;

public class CharacterPanelUIHandler
{
    private readonly Vector3 _defaultPosition;
    private readonly Vector3 _leftPosition;
    private readonly Vector3 _rightPosition;
    private readonly Vector3 _leftOffset = new Vector3(-200f, 45f, 0f);
    private readonly Vector3 _rightOffset = new Vector3(200f, 45f, 0f);
    
    private readonly Vector3 _leftScaleImage = new Vector3(-1f,1f,1f);
    private readonly Vector3 _rightScaleImage = Vector3.one;
    
    private readonly Vector3 _editModeStartScale = Vector3.one;
    private readonly Vector3 _playModeStartScale = Vector3.zero;

    private readonly CharacterPanelUI _characterPanelUI;
    private readonly TextConsistentlyViewer _textConsistentlyViewer;
    private readonly AnimationPanelWithScale _animationPanelWithScale;
    
    public TextConsistentlyViewer TextConsistentlyViewer => _textConsistentlyViewer;
    public AnimationPanelWithScale AnimationPanelWithScale => _animationPanelWithScale;
    public CharacterPanelUIHandler(CharacterPanelUI characterPanelUI)
    {
        _textConsistentlyViewer = new TextConsistentlyViewer(characterPanelUI.TalkTextComponent);
        _characterPanelUI = characterPanelUI;
        _defaultPosition = _characterPanelUI.PanelTransform.anchoredPosition;
        _leftPosition = _defaultPosition + _leftOffset;
        _rightPosition = _defaultPosition + _rightOffset;
        _animationPanelWithScale = new AnimationPanelWithScale(_characterPanelUI.PanelTransform, _characterPanelUI.CanvasGroup,
            _rightPosition, _leftPosition, _defaultPosition, characterPanelUI.DurationAnim);
    }
    public void CharacterTalkInEditMode(CharacterTalkData data)
    {
        SetPanelDirection(data);
        _characterPanelUI.TalkTextComponent.text = data.TalkText;
        _characterPanelUI.CanvasGroup.alpha = 1f;
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
    }

    public void DisappearanceCharacterTalkInPlayMode()
    {
        
        _characterPanelUI.PanelTransform.anchoredPosition = _defaultPosition;
        _characterPanelUI.gameObject.SetActive(false);
        _characterPanelUI.CanvasGroup.alpha = 0f;
    }

    private bool SetPanelDirection(CharacterTalkData data)
    {
        _characterPanelUI.gameObject.SetActive(true);
        _characterPanelUI.NameTextComponent.text = data.Name;
        if (data.DirectionType == DirectionType.Right)
        {
            _characterPanelUI.ImageTransform.localScale = _rightScaleImage;
            _characterPanelUI.NameTextComponent.alignment = TextAlignmentOptions.Right;
            return true;
        }
        else
        {
            _characterPanelUI.ImageTransform.localScale = _leftScaleImage;
            _characterPanelUI.NameTextComponent.alignment = TextAlignmentOptions.Left;
            return false;
        }
    }
}