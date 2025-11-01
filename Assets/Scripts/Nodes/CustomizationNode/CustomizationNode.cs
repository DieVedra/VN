using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

[NodeWidth(450),NodeTint("#6C0054")]
public class CustomizationNode : BaseNode, ILocalizable
{
    [SerializeField, HideInInspector] private bool _showFoldoutSettingsHairstyles;
    [SerializeField, HideInInspector] private bool _showFoldoutSettingsClothes;
    [SerializeField, HideInInspector] private bool _showFoldoutSettingsBodies;
    [SerializeField, HideInInspector] private bool _showFoldoutSettingsSwimsuits;
    [SerializeField] private int _customizationCharacterIndex;
    
    [SerializeField] private List<CustomizationSettings> _settingsBodies;
    [SerializeField] private List<CustomizationSettings> _settingsHairstyles;
    [SerializeField] private List<CustomizationSettings> _settingsClothes;
    [SerializeField] private List<CustomizationSettings> _settingsSwimsuits;  

    private const int SmileEmotionIndex = 2;
    private Background _background;
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
    public IReadOnlyList<ICustomizationSettings> SettingsBodies => _settingsBodies;
    public IReadOnlyList<ICustomizationSettings> SettingsHairstyles => _settingsHairstyles;
    public IReadOnlyList<ICustomizationSettings> SettingsClothes => _settingsClothes;
    public IReadOnlyList<ICustomizationSettings> SettingsSwimsuits => _settingsSwimsuits;
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
        _gameStatsHandler = new GameStatsHandler(_gameStatsProvider.GetStatsFromCurrentSeria(seriaIndex));
        _gameStatsProvider.GetStatsFromCurrentSeria(seriaIndex);
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
        ButtonSwitchSlideUIHandler.ActivateButtonSwitchToNextNode();

        _customizationCharacterPanelUIHandler.Dispose();
        _wardrobeCharacterViewer.ResetCharacterView();
        _wardrobeCharacterViewer.gameObject.SetActive(false);
    }

    protected override void SetInfoToView()
    {
        _wardrobeCharacterViewer.gameObject.SetActive(true);
        _background.SetWardrobeBackground();
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
        string text = _notificationPanelUIHandler.GetTextStats(stats, _gameStatsProvider);
        Debug.Log($"TryShowNotification    {text}");
        if (string.IsNullOrWhiteSpace(text) == false)
        {
            CompositeDisposable compositeDisposable = SetLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
            {
                _notificationPanelUIHandler.SetText(_notificationPanelUIHandler.GetTextStats(stats, _gameStatsProvider));
            });
            _notificationPanelUIHandler.EmergenceNotificationPanelInPlayMode(text, CancellationTokenSource.Token, compositeDisposable).Forget();
        }
    }
    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return CreateLocalizationArray(_settingsBodies, _settingsClothes, _settingsHairstyles, _settingsSwimsuits);
    }

    private void ResetBodiesCustomizationSettings()
    {
        _customizationNodeInitializer.InitCustomizationSettings(_settingsBodies, _customizableCharacter.GetBodiesSprites(), 1,1);
    }

    private void ResetHairstylesCustomizationSettings()
    {
        _customizationNodeInitializer.InitCustomizationSettings(_settingsHairstyles, _customizableCharacter.HairstylesData);
    }

    private void ResetClothesCustomizationSettings()
    {
        _customizationNodeInitializer.InitCustomizationSettings(_settingsClothes, _customizableCharacter.ClothesData, 1);
    }

    private void ResetSwimsuitsCustomizationSettings()
    {
        _customizationNodeInitializer.InitCustomizationSettings(_settingsSwimsuits, _customizableCharacter.SwimsuitsData, 1);
    }

    private void ReInitBodiesCustomizationSettings()
    {
        if (_settingsBodies == null)
        {
            _settingsBodies = new List<CustomizationSettings>();
        }
        _customizationNodeInitializer.ReInitCustomizationSettings(_settingsBodies, _customizableCharacter.GetBodiesSprites(), 1,1);
    }

    private void ReinitHairstylesCustomizationSettings()
    {
        if (_settingsHairstyles == null)
        {
            _settingsHairstyles = new List<CustomizationSettings>();
        }
        _customizationNodeInitializer.ReInitCustomizationSettings(_settingsHairstyles, _customizableCharacter.HairstylesData);
    }

    private void ReinitClothesCustomizationSettings()
    {
        if (_settingsClothes == null)
        {
            _settingsClothes = new List<CustomizationSettings>();
        }
        _customizationNodeInitializer.ReInitCustomizationSettings(_settingsClothes, _customizableCharacter.ClothesData, 1);
    }

    private void ReinitSwimsuitsCustomizationSettings()
    {
        if (_settingsSwimsuits == null)
        {
            _settingsSwimsuits = new List<CustomizationSettings>();
        }
        _customizationNodeInitializer.ReInitCustomizationSettings(_settingsSwimsuits, _customizableCharacter.SwimsuitsData, 1);
    }

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
        for (int i = 0; i < lists.Length; i++)
        {
            if (lists[i] != null)
            {
                foreach (var variable in lists[i])
                {
                    strings.Add(variable.LocalizationName);
                    foreach (var t in variable.GameStatsLocalizationStrings)
                    {
                        strings.Add(t.LocalizationName);
                    }
                }
            }
        }
        return strings;
    }
}