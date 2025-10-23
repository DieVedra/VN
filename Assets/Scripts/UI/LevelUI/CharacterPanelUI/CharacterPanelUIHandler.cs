using TMPro;
using UnityEngine;

public class CharacterPanelUIHandler
{
    private const float _oneValue = 1f;
    private const float _zeroValue = 0f;
    private const float _offsetValueX = 200f;
    private const float _offsetValueY = 45f;
    private readonly Vector3 _defaultPosition;
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
    private readonly TextBlockPositionHandler _textBlockPositionHandler;
    private readonly TextMeshProUGUI _talkTextComponent;
    private readonly RectTransform _talkTextRectTransform;
    public TextConsistentlyViewer TextConsistentlyViewer => _textConsistentlyViewer;
    public AnimationPanelWithScale AnimationPanelWithScale => _animationPanelWithScale;
    public CharacterPanelUIHandler(CharacterPanelUI characterPanelUI)
    {
        _textConsistentlyViewer = new TextConsistentlyViewer(characterPanelUI.TalkTextComponent);
        _characterPanelUI = characterPanelUI;
        _talkTextComponent = _characterPanelUI.TalkTextComponent;
        _defaultPosition = _characterPanelUI.PanelTransform.anchoredPosition;
        _leftPosition = _defaultPosition + _leftOffset;
        _rightPosition = _defaultPosition + _rightOffset;
        _animationPanelWithScale = new AnimationPanelWithScale(_characterPanelUI.PanelTransform, _characterPanelUI.CanvasGroup,
            _rightPosition, _leftPosition, _defaultPosition, characterPanelUI.DurationAnim);
        _talkTextRectTransform = _characterPanelUI.TalkTextComponent.GetComponent<RectTransform>();
        _textBlockPositionHandler = new TextBlockPositionHandler(new LineBreaksCountCalculator(), new CharacterTextBlockPositionCurveProvider());
    }
    public void CharacterTalkInEditMode(CharacterTalkData data)
    {
        SetPanelDirection(data);
        UpdatePosition(data.TalkText);
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
        UpdatePosition(data.TalkText);
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
    }
    private bool SetPanelDirection(CharacterTalkData data)
    {
        _characterPanelUI.gameObject.SetActive(true);
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

    private void UpdatePosition(string text)
    {
        _textBlockPositionHandler.UpdatePosition(_talkTextRectTransform, _talkTextComponent, text);
    }
}