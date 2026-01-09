using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

[NodeWidth(450),NodeTint("#6C0054")]
public class CustomizationNode : BaseNode, ILocalizable
{
    [SerializeField] private List<CustomizationSettings> _settingsBodies;
    [SerializeField] private List<CustomizationSettings> _settingsHairstyles;
    [SerializeField] private List<CustomizationSettings> _settingsClothes;
    [SerializeField] private List<CustomizationSettings> _settingsSwimsuits;

    [SerializeField] private int _customizationCharacterIndex;
    [SerializeField] private bool _addNotificationKey;
    [SerializeField] private LocalizationString _notificationText;
    private const int SmileEmotionIndex = 2;
    private IBackgroundProviderToCustomizationNode _background;
    
    private SelectedCustomizationContentIndexes _selectedCustomizationContentIndexes;
    
    private GameStatsHandler _gameStatsHandler;
    private CustomizationCharacterPanelUIHandler _customizationCharacterPanelUIHandler;
    private IGameStatsProvider _gameStatsProvider;
    private CustomizationCurtainUIHandler _customizationCurtainUIHandler;
    private CustomizationEndEvent<CustomizationResult> _customizationEndEvent;
    private Sound _sound;
    private Wallet _wallet;
    private CustomizationNodeInitializer _customizationNodeInitializer;
    private WardrobeCharacterViewer _wardrobeCharacterViewer;
    private CompositeDisposable _compositeDisposable;
    private NotificationPanelUIHandler _notificationPanelUIHandler;
    private CustomizableCharacter _customizableCharacter => CustomizableCharacters[_customizationCharacterIndex];

#if UNITY_EDITOR
    public IReadOnlyList<ICustomizationSettings> SettingsBodies => _settingsBodies;
    public IReadOnlyList<ICustomizationSettings> SettingsHairstyles => _settingsHairstyles;
    public IReadOnlyList<ICustomizationSettings> SettingsClothes => _settingsClothes;
    public IReadOnlyList<ICustomizationSettings> SettingsSwimsuits => _settingsSwimsuits;
#endif
    public IReadOnlyList<CustomizableCharacter> CustomizableCharacters { get; private set; }


    public void ConstructMyCustomizationNode(CustomizationCharacterPanelUIHandler customizationCharacterPanelUIHandler,
        CustomizationCurtainUIHandler customizationCurtainUIHandler,
        IReadOnlyList<CustomizableCharacter> customizableCharacters, Background background, Sound sound,
        IGameStatsProvider gameStatsProvider, Wallet wallet,
        WardrobeCharacterViewer wardrobeCharacterViewer, NotificationPanelUIHandler notificationPanelUIHandler, int seriaIndex)
    {
        _sound = sound;
        _gameStatsProvider = gameStatsProvider;
        _wallet = wallet;
        _background = background;
        CustomizableCharacters = customizableCharacters;
        _customizationCharacterPanelUIHandler = customizationCharacterPanelUIHandler;
        _customizationCurtainUIHandler = customizationCurtainUIHandler;
        _wardrobeCharacterViewer = wardrobeCharacterViewer;
        _notificationPanelUIHandler = notificationPanelUIHandler;
        _customizationEndEvent = new CustomizationEndEvent<CustomizationResult>();
        _gameStatsHandler = new GameStatsHandler(_gameStatsProvider.GetEmptyStatsFromCurrentSeria(seriaIndex));
        _gameStatsProvider.GetEmptyStatsFromCurrentSeria(seriaIndex);
        
#if UNITY_EDITOR
        if (IsPlayMode() == false)
        {
            _customizationNodeInitializer = new CustomizationNodeInitializer(_gameStatsHandler);
            if (_customizableCharacter != null)
            {
                ReInitBodiesCustomizationSettings();
                ReinitHairstylesCustomizationSettings();
                ReinitClothesCustomizationSettings();
                ReinitSwimsuitsCustomizationSettings();
            }
        }
#endif

    }

    public override async UniTask Enter(bool isMerged = false)
    {
        CancellationTokenSource = new CancellationTokenSource();
        ButtonSwitchSlideUIHandler.DeactivatePushOption();
        _wardrobeCharacterViewer.gameObject.SetActive(true);
        SetInfoToView();
        _compositeDisposable = _customizationEndEvent.SubscribeWithCompositeDisposable(CustomizationEnd);
        await UniTask.WhenAll(
            _sound.SmoothPlayWardrobeAudio(CancellationTokenSource.Token),
            _customizationCurtainUIHandler.CurtainOpens(CancellationTokenSource.Token));
    }

    public override async UniTask Exit()
    {
        CancellationTokenSource = new CancellationTokenSource();
        await _customizationCharacterPanelUIHandler.HideCustomizationContentInPlayMode();
        _compositeDisposable.Clear();
        await UniTask.WhenAll(
            _sound.SmoothAudio.SmoothStopAudio(CancellationTokenSource.Token, AudioSourceType.Music),
            _customizationCurtainUIHandler.CurtainCloses(CancellationTokenSource.Token));
        _customizationCharacterPanelUIHandler.ShutDown();
        _wardrobeCharacterViewer.ResetCharacterView();
        _wardrobeCharacterViewer.gameObject.SetActive(false);
        _background.DisableWardrobeBackground();
    }

    protected override void SetInfoToView()
    {
        _wardrobeCharacterViewer.gameObject.SetActive(true);
        _background.EnableWardrobeBackground();
        _selectedCustomizationContentIndexes = CreateCustomizationContent();
        _customizableCharacter.SetIndexes(
            GetIndex(_selectedCustomizationContentIndexes.SpriteIndexesBodies, _customizableCharacter.BodyIndex),
            GetIndex(_selectedCustomizationContentIndexes.SpriteIndexesHairstyles, _customizableCharacter.HairstyleIndex),
            GetIndex(_selectedCustomizationContentIndexes.SpriteIndexesClothes, _customizableCharacter.ClothesIndex),
            GetIndex(_selectedCustomizationContentIndexes.SpriteIndexesSwimsuits, _customizableCharacter.SwimsuitsIndex));
        
        if (_selectedCustomizationContentIndexes.SpriteIndexesClothes.Count == 0)
        {
            _wardrobeCharacterViewer.SetClothes(_customizableCharacter.GetSwimsuitSprite());
        }
        else
        {
            _wardrobeCharacterViewer.SetClothes(_customizableCharacter.GetClothesSprite());
        }

        _wardrobeCharacterViewer.SetCustomizableCharacterIndex(_customizationCharacterIndex);
        _wardrobeCharacterViewer.SetHairstyle(_customizableCharacter.GetHairstyleSprite());
        _wardrobeCharacterViewer.SetLook(_customizableCharacter.GetLookMySprite());
        
        _wardrobeCharacterViewer.SetEmotion(_customizableCharacter.GetEmotionMySprite(SmileEmotionIndex));

        if (IsPlayMode())
        {
            _customizationCharacterPanelUIHandler.ShowCustomizationContentInPlayMode(
                _wardrobeCharacterViewer, _selectedCustomizationContentIndexes, _wallet,
                new CalculateStatsHandler(_gameStatsHandler.GetGameCustomizationStatsForm()),
                SetLocalizationChangeEvent, _customizationEndEvent);
        }
        else
        {
            _customizationCharacterPanelUIHandler.SetContentInEditMode();
        }
    }

    private void CustomizationEnd(CustomizationResult customizationResult)
    {
        _wardrobeCharacterViewer.PlayPSEndCustomizationEffect();
        TryShowNotification(customizationResult.Stats);
        _gameStatsProvider.GameStatsHandler.UpdateStats(customizationResult.Stats);
        _wallet.RemoveCash(customizationResult.MonetsToRemove);
        _wallet.RemoveHearts(customizationResult.HeartsToRemove);
        SwitchToNextNodeEvent.Execute();
    }

    private void TryShowNotification(List<CustomizationStat> stats)
    {
        string text = GetAllTextNotification();
        if (string.IsNullOrWhiteSpace(text) == false)
        {
            CompositeDisposable compositeDisposable = SetLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
            {
                _notificationPanelUIHandler.SetText(GetAllTextNotification());
            });
            _notificationPanelUIHandler.EmergenceNotificationPanelInPlayMode(text, CancellationTokenSource.Token, true, compositeDisposable).Forget();
        }

        string GetAllTextNotification()
        {
            if (_addNotificationKey)
            {
                return _notificationPanelUIHandler.GetMergedText(_notificationText,
                    _notificationPanelUIHandler.GetTextStats(stats, _gameStatsProvider));
            }
            else
            {
                return _notificationPanelUIHandler.GetTextStats(stats, _gameStatsProvider);
            }
        }
    }
    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return CreateLocalizationArray(_settingsBodies, _settingsClothes, _settingsHairstyles, _settingsSwimsuits);
    }
#if UNITY_EDITOR

    private void ResetBodiesCustomizationSettings()
    {
        _customizationNodeInitializer.InitCustomizationSettings(_settingsBodies, _customizableCharacter.GetBodiesSprites());
    }

    private void ResetHairstylesCustomizationSettings()
    {
        _customizationNodeInitializer.InitCustomizationSettings(_settingsHairstyles, _customizableCharacter.HairstylesData);
    }

    private void ResetClothesCustomizationSettings()
    {
        _customizationNodeInitializer.InitCustomizationSettings(_settingsClothes, _customizableCharacter.ClothesData);
    }

    private void ResetSwimsuitsCustomizationSettings()
    {
        _customizationNodeInitializer.InitCustomizationSettings(_settingsSwimsuits, _customizableCharacter.SwimsuitsData);
    }

    private void ReInitBodiesCustomizationSettings()
    {
        _settingsBodies ??= new List<CustomizationSettings>();
        _customizationNodeInitializer.ReInitCustomizationSettings(_settingsBodies, _customizableCharacter.GetBodiesSprites());
    }

    private void ReinitHairstylesCustomizationSettings()
    {
        _settingsHairstyles ??= new List<CustomizationSettings>();
        _customizationNodeInitializer.ReInitCustomizationSettings(_settingsHairstyles, _customizableCharacter.HairstylesData);
    }

    private void ReinitClothesCustomizationSettings()
    {
        _settingsClothes ??= new List<CustomizationSettings>();
        _customizationNodeInitializer.ReInitCustomizationSettings(_settingsClothes, _customizableCharacter.ClothesData);
    }

    private void ReinitSwimsuitsCustomizationSettings()
    {
        if (_settingsSwimsuits == null)
        {
            _settingsSwimsuits = new List<CustomizationSettings>();
        }
        _customizationNodeInitializer.ReInitCustomizationSettings(_settingsSwimsuits, _customizableCharacter.SwimsuitsData);
    }
#endif

    private SelectedCustomizationContentIndexes CreateCustomizationContent()
    {
        return CustomizationNodeInitializer.CreateCustomizationContent(_settingsBodies, _settingsHairstyles, _settingsClothes, _settingsSwimsuits, _customizableCharacter);
    }

    private int GetIndex(IReadOnlyList<ICustomizationSettings> spriteIndexes, int currentIndex)
    {
        if (spriteIndexes.Count == 0)
        {
            return currentIndex;
        }
        else
        {
            return spriteIndexes[0].Index;
        }
    }

    private IReadOnlyList<LocalizationString> CreateLocalizationArray(params List<CustomizationSettings>[] lists)
    {
        List<LocalizationString> strings = new List<LocalizationString>();
        foreach (var t1 in lists)
        {
            if (t1 != null)
            {
                foreach (var cs in t1)
                {
                    if (cs.KeyAdd == true)
                    {
                        strings.Add(cs.LocalizationNameToGame);
                        foreach (var ls in cs.GameStatsLocalizationStrings)
                        {
                            strings.Add(ls.LocalizationNameToGame);
                        }
                    }
                }
            }
        }
        return strings;
    }
}