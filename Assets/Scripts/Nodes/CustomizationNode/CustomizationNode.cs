﻿using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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
    [SerializeField] private List<CustomizationSettings> _settingsHairstyles;
    [SerializeField] private List<CustomizationSettings> _settingsClothes;
    [SerializeField] private List<CustomizationSettings> _settingsSwimsuits;  

    private const int SmileEmotionIndex = 2;
    private int _seriaIndex;
    private Background _background;
    private SelectedCustomizationContentIndexes _selectedCustomizationContentIndexes;
    private CustomizableCharacter _customizableCharacter;
    private WardrobeSeriaData _wardrobeSeriaData;
    private GameStatsHandler _gameStatsHandler;
    private CustomizationCharacterPanelUIHandler _customizationCharacterPanelUIHandler;
    private IGameStatsProvider _gameStatsProvider;
    private CustomizationCurtainUIHandler _customizationCurtainUIHandler;
    
    private CustomizationEndEvent<CustomizationResult> _customizationEndEvent;
    private Sound _sound;
    private Wallet _wallet;
    private CustomizationNodeInitializer _customizationNodeInitializer;
    private WardrobeCharacterViewer _wardrobeCharacterViewer;
    public IReadOnlyList<ICustomizationSettings> SettingsBodies => _settingsBodies;
    public IReadOnlyList<ICustomizationSettings> SettingsHairstyles => _settingsHairstyles;
    public IReadOnlyList<ICustomizationSettings> SettingsClothes => _settingsClothes;
    public IReadOnlyList<ICustomizationSettings> SettingsSwimsuits => _settingsSwimsuits;

    public void ConstructMyCustomizationNode(CustomizationCharacterPanelUIHandler customizationCharacterPanelUIHandler,
        CustomizationCurtainUIHandler customizationCurtainUIHandler,
        CustomizableCharacter customizableCharacter, WardrobeSeriaData wardrobeSeriaData, Background background, Sound sound,
        IGameStatsProvider gameStatsProvider, Wallet wallet, WardrobeCharacterViewer wardrobeCharacterViewer, int seriaIndex)
    {
        _seriaIndex = seriaIndex;
        _sound = sound;
        _gameStatsProvider = gameStatsProvider;
        _wallet = wallet;
        _background = background;
        _customizableCharacter = customizableCharacter;
        _wardrobeSeriaData = wardrobeSeriaData;
        _customizationCharacterPanelUIHandler = customizationCharacterPanelUIHandler;
        _customizationCurtainUIHandler = customizationCurtainUIHandler;
        _wardrobeCharacterViewer = wardrobeCharacterViewer;
        _customizationEndEvent = new CustomizationEndEvent<CustomizationResult>();
        _gameStatsHandler = new GameStatsHandler(_gameStatsProvider.GetStatsFromCurrentSeria(seriaIndex));_gameStatsProvider.GetStatsFromCurrentSeria(seriaIndex);
        if (IsPlayMode() == false)
        {
            _customizationNodeInitializer = new CustomizationNodeInitializer(_gameStatsHandler);

            if (_wardrobeSeriaData != null)
            {
                ReInitBodiesCustomizationSettings();
                ReinitHairstylesCustomizationSettings();
                ReinitClothesCustomizationSettings();
                ReinitSwimsuitsCustomizationSettings();
            }
        
            TryInitStringsToLocalization(CreateLocalizationArray(_settingsBodies, _settingsClothes, _settingsHairstyles, _settingsSwimsuits));
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
            _sound.SmoothPlayWardrobeAudio(CancellationTokenSource.Token),
            _customizationCurtainUIHandler.CurtainOpens(CancellationTokenSource.Token));
    }

    public override async UniTask Exit()
    {
        CancellationTokenSource = new CancellationTokenSource();
        await UniTask.WhenAll(
            _sound.SmoothAudio.SmoothStopAudio(CancellationTokenSource.Token, AudioSourceType.Music),
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
        _wardrobeCharacterViewer.SetHairstyle(_customizableCharacter.GetHairstyleSprite());
        _wardrobeCharacterViewer.SetLook(_customizableCharacter.GetLookMySprite());
        
        _wardrobeCharacterViewer.SetEmotion(_customizableCharacter.GetEmotionMySprite(SmileEmotionIndex));

        if (IsPlayMode())
        {
            _customizationCharacterPanelUIHandler.ShowCustomizationContentInPlayMode(
                
                _wardrobeCharacterViewer, _selectedCustomizationContentIndexes,
                new CalculatePriceHandler(_wallet.Monets),
                new CalculateStatsHandler(_gameStatsHandler.GetGameStatsForm()));
        }
        else
        {
            _customizationCharacterPanelUIHandler.SetContentInEditMode(_selectedCustomizationContentIndexes);
        }
    }

    private void CustomizationEnd(CustomizationResult customizationResult)
    {
        // PS engage
        _gameStatsProvider.GameStatsHandler.UpdateStat(customizationResult.Stats);
        _wallet.RemoveCash(_wallet.Monets - customizationResult.PreliminaryBalance);
        SwitchToNextNodeEvent.Execute();
    }

    private void ResetBodiesCustomizationSettings()
    {
        _customizationNodeInitializer.InitCustomizationSettings(ref _settingsBodies, _wardrobeSeriaData.GetBodiesSprites(), 1,1);
    }

    private void ResetHairstylesCustomizationSettings()
    {
        _customizationNodeInitializer.InitCustomizationSettings(ref _settingsHairstyles, _customizableCharacter.HairstylesData);
    }

    private void ResetClothesCustomizationSettings()
    {
        _customizationNodeInitializer.InitCustomizationSettings(ref _settingsClothes, _customizableCharacter.ClothesData, 1);
    }

    private void ResetSwimsuitsCustomizationSettings()
    {
        _customizationNodeInitializer.InitCustomizationSettings(ref _settingsSwimsuits, _customizableCharacter.SwimsuitsData, 1);
    }

    private void ReInitBodiesCustomizationSettings()
    {
        _customizationNodeInitializer.ReInitCustomizationSettings(ref _settingsBodies, _wardrobeSeriaData.GetBodiesSprites(), 1,1);
    }

    private void ReinitHairstylesCustomizationSettings()
    {
        _customizationNodeInitializer.ReInitCustomizationSettings(ref _settingsHairstyles, _customizableCharacter.HairstylesData);
    }

    private void ReinitClothesCustomizationSettings()
    {
        _customizationNodeInitializer.ReInitCustomizationSettings(ref _settingsClothes, _customizableCharacter.ClothesData, 1);
    }

    private void ReinitSwimsuitsCustomizationSettings()
    {
        _customizationNodeInitializer.ReInitCustomizationSettings(ref _settingsSwimsuits, _customizableCharacter.SwimsuitsData, 1);
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
    private LocalizationString[] CreateLocalizationArray(params List<CustomizationSettings>[] lists)
    {
        List<LocalizationString> strings = new List<LocalizationString>();
        for (int i = 0; i < lists.Length; i++)
        {
            if (lists[i] != null)
            {
                for (int j = 0; j < lists[i].Count; j++)
                {
                    strings.Add(lists[i][j].LocalizationName);
                }
            }
        }
        return strings.ToArray();
    }
}