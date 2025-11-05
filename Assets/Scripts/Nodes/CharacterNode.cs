using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

[NodeTint("#515000")]
public class CharacterNode : BaseNode, IPutOnSwimsuit, ILocalizable
{
    [SerializeField] private LocalizationString _localizationText;
    [SerializeField] private int _indexCharacter;
    [SerializeField] private int _indexEmotion;
    [SerializeField] private int _indexLook;
    [SerializeField] private int _previousIndexCharacter;
    [SerializeField] private int _previousCharactersCount;
    [SerializeField] private DirectionType _directionType;
    [SerializeField] private bool _foldoutIsOpen;
    [SerializeField] private bool _toggleIsSwimsuit;
    [SerializeField] private bool _toggleShowPanel = true;
    [SerializeField] private bool _overrideName;
    // [SerializeField] private string _overridedName;
    [SerializeField] private LocalizationString _overridedNameLocalization;

    private Background _background;
    private CharacterTalkData _characterTalkData;
    private CharacterPanelUIHandler _characterPanelUIHandler;
    private CharacterViewer _characterViewer;
    private CompositeDisposable _compositeDisposable;
    public IReadOnlyList<Character> Characters { get; private set; }

    public void ConstructMyCharacterNode(IReadOnlyList<Character> characters, CharacterPanelUIHandler characterPanelUIHandler,
        Background background, CharacterViewer characterViewer)
    {
        Characters = characters;
        _characterPanelUIHandler = characterPanelUIHandler;
        _background = background;
        _characterViewer = characterViewer;
    }

    public override async UniTask Enter(bool isMerged = false)
    {
        CancellationTokenSource = new CancellationTokenSource();
        _compositeDisposable = SetLocalizationChangeEvent.SubscribeWithCompositeDisposable(SetLocalizationText);
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
        _compositeDisposable.Dispose();
        _compositeDisposable = null;
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
        _characterViewer.ResetCharacterView();
        _characterViewer.gameObject.SetActive(true);
        _characterViewer.SetDirection(_directionType);
        if (Characters[_indexCharacter] is CustomizableCharacter customizableCharacter)
        {
            if (_toggleIsSwimsuit)
            {
                _characterViewer.SetClothes(customizableCharacter.GetSwimsuitSprite());
            }
            else
            {
                _characterViewer.SetClothes(customizableCharacter.GetClothesSprite());
            }
            _characterViewer.SetHairstyle(customizableCharacter.GetHairstyleSprite());
            _characterViewer.SetLook(customizableCharacter.GetLookMySprite());
            _characterViewer.SetEmotion(GetEmotionFromCustomization(customizableCharacter));
        }
        else
        {
            _characterViewer.SetEmotion(GetEmotionCharacter());
            _characterViewer.SetLook(GetLook());
        }
        _characterTalkData = CreateCharacterTalkData();
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

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        List<LocalizationString> localizationText = new List<LocalizationString>();
        if (_overrideName == true)
        {
            localizationText.Add(_overridedNameLocalization);
        }
        localizationText.Add(_localizationText);
        return localizationText;
    }

    private CharacterTalkData CreateCharacterTalkData()
    {
        return new CharacterTalkData(_directionType,
            GetName(),
            _localizationText);
    }
    private MySprite GetLook()
    {
        return Characters[_indexCharacter].GetLookMySprite(_indexLook);
    }

    private MySprite GetEmotionCharacter()
    {
        if (_indexEmotion == 0)
        {
            return null;
        }
        else
        {
            int index = _indexEmotion;
            return Characters[_indexCharacter].GetEmotionMySprite(--index);
        }
    }

    private MySprite GetEmotionFromCustomization(CustomizableCharacter customizableCharacter)
    {
        
        return customizableCharacter.GetEmotionMySprite(_indexEmotion);
    }

    private string GetName()
    {
        if (_overrideName == true)
        {
            return _overridedNameLocalization;
        }
        else
        {
            return Characters[_indexCharacter].MyNameText;
        }
    }

    protected override void TryActivateButtonSwitchToNextSlide()
    {
        if (IsMerged == false)
        {
            ButtonSwitchSlideUIHandler.ActivateButtonSwitchToNextNode();
        }
    }

    private void SetLocalizationText()
    {
        _characterPanelUIHandler.SetLocalizationText(CreateCharacterTalkData());
    }
}