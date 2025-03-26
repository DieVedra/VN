using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

[NodeWidth(350),NodeTint("#6C0054")]
public class CustomizationNode : BaseNode
{
    [SerializeField, HideInInspector] private int _hairIndex;
    [SerializeField, HideInInspector] private bool _showFoldoutSettingsHairstyles;
    [SerializeField, HideInInspector] private bool _showFoldoutSettingsClothes;
    [SerializeField, HideInInspector] private bool _showFoldoutSettingsBodies;
    [SerializeField, HideInInspector] private bool _showFoldoutSettingsSwimsuits;
    
    [SerializeField] private List<CustomizationSettings> _settingsBodies;
    [SerializeField] private List<CustomizationSettings> _settingsHairstyles; //блонд каре, коричневые, песочный блонд каре, русые, рыжие волнистые, серо голубые 
    [SerializeField] private List<CustomizationSettings> _settingsClothes; // топ и джинсы, топ с кольцом и джинсовые шорты, юбка и топ, 
    [SerializeField] private List<CustomizationSettings> _settingsSwimsuits; // Купальник черный, Купальник с блестками , Купальник черный мини   

    private readonly int _smileEmotionIndex = 2;
    private readonly int _backgroundCustomizationIndex = 2;
    private Background _background;
    private SelectedCustomizationContentIndexes _selectedCustomizationContentIndexes;
    private CustomizableCharacter _customizableCharacter;
    private CustomizationCharacterPanelUIHandler _customizationCharacterPanelUIHandler;

    private CustomizationCurtainUIHandler _customizationCurtainUIHandler;
    
    private CustomizationEndEvent _customizationEndEvent;
    private Sound _sound;
    private Wallet _wallet;
    private GameStatsCustodian _gameStatsCustodian;
    private CustomizationNodeInitializer _customizationNodeInitializer;
    private WardrobeCharacterViewer _wardrobeCharacterViewer;
    public void ConstructMyCustomizationNode(CustomizationCharacterPanelUIHandler customizationCharacterPanelUIHandler, CustomizationCurtainUIHandler customizationCurtainUIHandler,
        CustomizableCharacter customizableCharacter, Background background, Sound sound, GameStatsCustodian gameStatsCustodian,
        Wallet wallet, WardrobeCharacterViewer wardrobeCharacterViewer)
    {
        _sound = sound;
        _gameStatsCustodian = gameStatsCustodian;
        _customizationNodeInitializer = new CustomizationNodeInitializer(gameStatsCustodian);
        _wallet = wallet;
        _background = background;
        _customizableCharacter = customizableCharacter;
        _customizationCharacterPanelUIHandler = customizationCharacterPanelUIHandler;
        _customizationCurtainUIHandler = customizationCurtainUIHandler;
        _wardrobeCharacterViewer = wardrobeCharacterViewer;
        _customizationEndEvent = new CustomizationEndEvent();
        if (IsPlayMode() == false)
        {
            TryInitCustomizationSettings(ref _settingsBodies, _customizableCharacter.LooksData.MySprites, 1,1);
            TryInitCustomizationSettings(ref _settingsHairstyles, _customizableCharacter.HairstylesData.MySprites);
            TryInitCustomizationSettings(ref _settingsClothes, _customizableCharacter.ClothesData.MySprites, 1);
            TryInitCustomizationSettings(ref _settingsSwimsuits, _customizableCharacter.SwimsuitsData.MySprites, 1);

            _gameStatsCustodian.StatsChangedReactiveCommand.Subscribe(_ =>
            {
                ReinitBodiesCustomizationSettings();
                ReinitHairstylesCustomizationSettings();
                ReinitClothesCustomizationSettings();
                ReinitSwimsuitsCustomizationSettings();
            });
        }
    }

    public override async UniTask Enter(bool isMerged = false)
    {
        CancellationTokenSource = new CancellationTokenSource();
        ButtonSwitchSlideUIHandler.DeactivatePushOption();
        _wardrobeCharacterViewer.gameObject.SetActive(true);
        SetInfoToView();
        _customizationCharacterPanelUIHandler.ButtonsCustomizationHandler.ActivateButtonsCustomization(_selectedCustomizationContentIndexes, _customizationEndEvent);
        _customizationEndEvent.Subscribe(CustomizationEnd);
        await UniTask.WhenAll(
            _sound.SmoothPlayAudio(CancellationTokenSource.Token, null),
            _customizationCurtainUIHandler.CurtainOpens(CancellationTokenSource.Token));
    }

    public override async UniTask Exit()
    {
        CancellationTokenSource = new CancellationTokenSource();
        await UniTask.WhenAll(
            _sound.SmoothStopAudio(CancellationTokenSource.Token, null),
            _customizationCurtainUIHandler.CurtainCloses(CancellationTokenSource.Token),
            _customizationCharacterPanelUIHandler.ButtonPlayHandler.OffAnim());
        ButtonSwitchSlideUIHandler.ActivateButtonSwitchToNextNode();

        _customizationCharacterPanelUIHandler.Dispose();
        _wardrobeCharacterViewer.ResetCharacterView();
        _wardrobeCharacterViewer.gameObject.SetActive(false);
    }

    protected override void SetInfoToView()
    {
        _wardrobeCharacterViewer.gameObject.SetActive(true);
        _background.SetBackgroundPosition(BackgroundPosition.Central, _backgroundCustomizationIndex);
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
        _wardrobeCharacterViewer.SetHairstyle(_customizableCharacter.GetHairstyleSprite());
        _wardrobeCharacterViewer.SetLook(_customizableCharacter.GetBodySprite());
        _wardrobeCharacterViewer.SetEmotion(_customizableCharacter.GetCurrentEmotionsData().MySprites[_smileEmotionIndex]);

        if (IsPlayMode())
        {
            _customizationCharacterPanelUIHandler.ShowCustomizationContentInPlayMode(
                
                _wardrobeCharacterViewer, _selectedCustomizationContentIndexes,
                new CalculatePriceHandler(_wallet.Monets),
                new CalculateStatsHandler(_gameStatsCustodian.GetGameBaseStatsForm()));
        }
        else
        {
            _customizationCharacterPanelUIHandler.SetContentInEditMode(_selectedCustomizationContentIndexes);
        }
    }
    
    private void CustomizationEnd(CustomizationResult customizationResult)
    {
        // PS engage
        _gameStatsCustodian.UpdateStat(customizationResult.Stats);
        _wallet.RemoveCash(_wallet.Monets - customizationResult.PreliminaryBalance);

        SwitchToNextNodeEvent.Execute();
    }

    private void TryInitCustomizationSettings(ref List<CustomizationSettings> settings, List<MySprite> sprites, int skipFirstWordsInLabel = 2, int skipEndWordsInLabel = 0)
    {
        if (sprites != null)
        {
            if (settings == null)
            {
                settings = _customizationNodeInitializer.InitCustomizationSettings(sprites, skipFirstWordsInLabel, skipEndWordsInLabel);
            }
            else if (settings.Count != sprites.Count)
            {
                settings = _customizationNodeInitializer.ReInitCustomizationSettings(ref settings, sprites, skipFirstWordsInLabel, skipEndWordsInLabel);
            }
            else if (settings.Count > 2 && settings[1].Index == 0)
            {
                settings = _customizationNodeInitializer.InitCustomizationSettings(sprites, skipFirstWordsInLabel, skipEndWordsInLabel);
            }
        }
    }

    private void ResetBodiesCustomizationSettings()
    {
        _settingsBodies = _customizationNodeInitializer.InitCustomizationSettings(_customizableCharacter.LooksData.MySprites, 1,1);
    }

    private void ReinitBodiesCustomizationSettings()
    {
        _settingsBodies = _customizationNodeInitializer.ReInitCustomizationSettings(ref _settingsBodies, _customizableCharacter.LooksData.MySprites, 1,1);
    }

    private void ResetHairstylesCustomizationSettings()
    {
        _settingsHairstyles = _customizationNodeInitializer.InitCustomizationSettings(_customizableCharacter.HairstylesData.MySprites);
    }

    private void ReinitHairstylesCustomizationSettings()
    {
        _settingsHairstyles = _customizationNodeInitializer.ReInitCustomizationSettings(ref _settingsHairstyles, _customizableCharacter.HairstylesData.MySprites);
    }

    private void ResetClothesCustomizationSettings()
    {
        _settingsClothes = _customizationNodeInitializer.InitCustomizationSettings(_customizableCharacter.ClothesData.MySprites, 1);
    }

    private void ReinitClothesCustomizationSettings()
    {
        _settingsClothes = _customizationNodeInitializer.ReInitCustomizationSettings(ref _settingsClothes, _customizableCharacter.ClothesData.MySprites, 1);
    }
    private void ResetSwimsuitsCustomizationSettings()
    {
        _settingsSwimsuits = _customizationNodeInitializer.InitCustomizationSettings(_customizableCharacter.SwimsuitsData.MySprites, 1);
    }
    private void ReinitSwimsuitsCustomizationSettings()
    {
        _settingsSwimsuits = _customizationNodeInitializer.ReInitCustomizationSettings(ref _settingsSwimsuits, _customizableCharacter.SwimsuitsData.MySprites, 1);
    }
    private SelectedCustomizationContentIndexes CreateCustomizationContent()
    {
        return _customizationNodeInitializer.CreateCustomizationContent(_settingsBodies, _settingsHairstyles, _settingsClothes, _settingsSwimsuits, _customizableCharacter);
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
}