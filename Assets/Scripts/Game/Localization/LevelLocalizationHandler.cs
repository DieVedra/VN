
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class LevelLocalizationHandler : ILevelLocalizationHandler
{
    public readonly SwitchLocalizationProcessor SwitchLocalizationProcessor;
    private readonly LevelLocalizationProvider _levelLocalizationProvider;
    private readonly CharacterProviderBuildMode _characterProviderBuildMode;
    private IReadOnlyDictionary<string, string> _currentLocalization;

    // public event Action OnStartLoadLocalization;
    public event Action OnEndLoadLocalization;

    public LevelLocalizationHandler(LevelLocalizationProvider levelLocalizationProvider, CharacterProviderBuildMode characterProviderBuildMode,
        ReactiveCommand tryLoadLocalizationOnSwitchLanguage)
    {
        _levelLocalizationProvider = levelLocalizationProvider;
        _characterProviderBuildMode = characterProviderBuildMode;
        SwitchLocalizationProcessor = new SwitchLocalizationProcessor();
        tryLoadLocalizationOnSwitchLanguage.Subscribe(_ =>
        {
            _levelLocalizationProvider.TryLoadLocalizationOnSwitchLanguageFromSettings();
        });
    }

    public void TrySetCurrentLocalization(SeriaNodeGraphsHandler seriaNodeGraphsHandler, GameStatsHandler gameStatsHandler)
    {
        if (_levelLocalizationProvider.ParticipiteInLoad)
        {
            _currentLocalization = _levelLocalizationProvider.GetCurrentLocalization();
            
            if (_currentLocalization != null)
            {
                SetLocalizationToSeriaTexts(seriaNodeGraphsHandler);
                SetLocalizationToStats(gameStatsHandler);
                SetLocalizationToCharacters();
                _currentLocalization = null;
            }
        }
    }

    private void SetLocalizationToSeriaTexts(SeriaNodeGraphsHandler seriaNodeGraphsHandler)
    {
        for (int i = 0; i < seriaNodeGraphsHandler.SeriaPartNodeGraphs.Count; i++)
        {
            for (int j = 0; j < seriaNodeGraphsHandler.SeriaPartNodeGraphs[i].nodes.Count; j++)
            {
                if (seriaNodeGraphsHandler.SeriaPartNodeGraphs[i].nodes[j] is ILocalizable localizable)
                {
                    foreach (var localizationString in localizable.GetLocalizableContent())
                    {
                        SetText(localizationString);
                    }
                }
            }
        }
    }

    private void SetLocalizationToStats(GameStatsHandler gameStatsHandler)
    {
        for (int i = 0; i < gameStatsHandler.Stats.Count; i++)
        {
            SetText(gameStatsHandler.Stats[i].LocalizationName);
        }
    }

    private void SetLocalizationToCharacters()
    {
        foreach (var character in _characterProviderBuildMode.GetCharacters())
        {
            SetText(character.Name);
        }
    }

    private void SetText(LocalizationString localizationString)
    {
        if (_currentLocalization.TryGetValue(localizationString.Key, out string text))
        {
            localizationString.SetText(text);
        }
    }

    public bool IsLocalizationHasBeenChanged()
    {
        return _levelLocalizationProvider.IsLocalizationHasBeenChanged();
    }

    public async UniTask Test()
    {
        Debug.Log($"Test");
        await UniTask.Delay(5000);
        OnEndLoadLocalization?.Invoke();
    }
}