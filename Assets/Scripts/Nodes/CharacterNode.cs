using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[NodeTint("#515000")]
public class CharacterNode : BaseNode, IPutOnSwimsuit
{
    [SerializeField, TextArea] private string _text = "...";
    [SerializeField, HideInInspector] private int _indexCharacter;
    [SerializeField, HideInInspector] private int _indexEmotion;
    [SerializeField, HideInInspector] private int _indexLook;
    [SerializeField, HideInInspector] private int _previousIndexCharacter;
    [SerializeField, HideInInspector] private int _previousCharactersCount;
    [SerializeField, HideInInspector] private DirectionType _directionType;
    [SerializeField, HideInInspector] private bool _foldoutIsOpen;
    [SerializeField, HideInInspector] private bool _toggleIsSwimsuit;
    [SerializeField, HideInInspector] private bool _toggleShowPanel = true;
    [SerializeField, HideInInspector] private bool _overrideName;
    [SerializeField, HideInInspector] private string _overridedName;

    private List<Character> _characters;
    private Background _background;
    private CharacterTalkData _characterTalkData;
    private CharacterPanelUIHandler _characterPanelUIHandler;
    private CharacterViewer _characterViewer;
    public List<Character> Characters => _characters;

    public void ConstructMyCharacterNode(List<Character> characters, CharacterPanelUIHandler characterPanelUIHandler,
        Background background, CharacterViewer characterViewer)
    {
        _characters = characters;
        _characterPanelUIHandler = characterPanelUIHandler;
        _background = background;
        _characterViewer = characterViewer;
    }

    public override async UniTask Enter(bool isMerged = false)
    {
        CancellationTokenSource = new CancellationTokenSource();
        IsMerged = isMerged;
        _characterViewer.gameObject.SetActive(true);
        SetInfoToView();
        _characterPanelUIHandler.EmergenceCharacterTalkInPlayMode(_characterTalkData);
        if (isMerged == false)
        {
            ButtonSwitchSlideUIHandler.ActivateSkipTransition(SkipEnterTransition);
        }

        _characterPanelUIHandler.TextConsistentlyViewer.ClearText();
        await UniTask.WhenAll(
            _background.SetBackgroundMovementDuringDialogueInPlayMode(CancellationTokenSource.Token, _directionType),
            _characterViewer.SpriteViewer.CharacterAnimations.EmergenceChar(CancellationTokenSource.Token, _directionType),
            _characterPanelUIHandler.AnimationPanelWithScale.UnfadePanelWithScale(_toggleShowPanel, CancellationTokenSource.Token));
        await _characterPanelUIHandler.TextConsistentlyViewer.SetTextConsistently(CancellationTokenSource.Token, _characterTalkData.TalkText);
        TryActivateButtonSwitchToNextSlide();
    }

    public override async UniTask Exit()
    {
        CancellationTokenSource = new CancellationTokenSource();

        if (IsMerged == false)
        {
            ButtonSwitchSlideUIHandler.ActivateSkipTransition(SkipExitTransition);
        }

        await UniTask.WhenAll(
            _characterViewer.SpriteViewer.CharacterAnimations.DisappearanceChar(CancellationTokenSource.Token, _directionType),
            _characterPanelUIHandler.AnimationPanelWithScale.FadePanelWithScale(CancellationTokenSource.Token, _directionType));
        _characterPanelUIHandler.DisappearanceCharacterTalkInPlayMode();
        _characterViewer.gameObject.SetActive(false);
    }

    public void PutOnSwimsuit(bool putOnSwimsuitKey)
    {
        if (putOnSwimsuitKey)
        {
            _indexLook = 1; // 1 is swimsuit index
        }
        else
        {
            _indexLook = 0; // 0 is default look index
        }
        _toggleIsSwimsuit = putOnSwimsuitKey;
    }

    protected override void SetInfoToView()
    {
        _characterViewer.gameObject.SetActive(true);
        _characterViewer.SetDirection(_directionType);
        // _characterViewer.SetName(GetName());
        if (_characters[_indexCharacter] is CustomizableCharacter)
        {
            CustomizableCharacter customizableCharacter = _characters[_indexCharacter] as CustomizableCharacter;
            if (_toggleIsSwimsuit)
            {
                _characterViewer.SetClothes(customizableCharacter.GetSwimsuitSprite());
            }
            else
            {
                _characterViewer.SetClothes(customizableCharacter.GetClothesSprite());
            }
            _characterViewer.SetHairstyle(customizableCharacter.GetHairstyleSprite());
            _characterViewer.SetLook(customizableCharacter.GetBodySprite());
            _characterViewer.SetEmotion(GetEmotionFromCustomization(customizableCharacter));
        }
        else
        {
            _characterViewer.SetEmotion(GetEmotionCharacter());
            _characterViewer.SetLook(GetLook());
        }
        _characterTalkData = new CharacterTalkData(_directionType, _overrideName == true ? _overridedName : GetName(), _text);
        if (_toggleShowPanel == true)
        {
            _characterPanelUIHandler.CharacterTalkInEditMode(_characterTalkData);
        }

        _background.SetBackgroundMovementDuringDialogueInEditMode(_directionType);
    }

    public override void SkipEnterTransition()
    {
        CancellationTokenSource.Cancel();
        _characterViewer.SpriteViewer.CharacterAnimations.MakeVisibleSprite();
        SetInfoToView();
        TryActivateButtonSwitchToNextSlide();
    }
    public override void SkipExitTransition()
    {
        CancellationTokenSource.Cancel();

        _characterViewer.SpriteViewer.CharacterAnimations.MakeInvisibleSprite();
        _characterPanelUIHandler.DisappearanceCharacterTalkInPlayMode();
    }
    private MySprite GetLook()
    {
        return _characters[_indexCharacter].LooksData.MySprites[_indexLook];
    }
    
    private MySprite GetEmotionCharacter()
    {
        MySprite result = null;
        if (_characters[_indexCharacter].EmotionsData != null)
        {
            if (_indexEmotion < _characters[_indexCharacter].EmotionsData.MySprites.Count)
            {
                result = _characters[_indexCharacter].EmotionsData.MySprites[_indexEmotion];
            }
        }
        return result;
    }
    private MySprite GetEmotionFromCustomization(CustomizableCharacter customizableCharacter)
    {
        if (_indexEmotion < customizableCharacter.GetCurrentEmotionsData().MySprites.Count)
        {
            return customizableCharacter.GetCurrentEmotionsData().MySprites[_indexEmotion];
        }
        else
        {
            return null;
        }
    }
    private string GetName()
    {
        return _characters[_indexCharacter].MyName;
    }
    protected override void TryActivateButtonSwitchToNextSlide()
    {
        if (IsMerged == false)
        {
            ButtonSwitchSlideUIHandler.ActivateButtonSwitchToNextNode();
        }
    }
}