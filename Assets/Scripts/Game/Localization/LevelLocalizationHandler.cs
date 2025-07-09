using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LevelLocalizationHandler : ILevelLocalizationHandler
{
    private readonly LevelLocalizationProvider _levelLocalizationProvider;
    private readonly CharacterProviderBuildMode _characterProviderBuildMode;
    private readonly SetLocalizationChangeEvent _setLocalizationChangeEvent;
    private IReadOnlyDictionary<string, string> _currentLocalization;
    public event Action OnTrySwitchLocalization;
    public event Action OnEndSwitchLocalization;

    public LevelLocalizationHandler(LevelLocalizationProvider levelLocalizationProvider, CharacterProviderBuildMode characterProviderBuildMode,
        SetLocalizationChangeEvent setLocalizationChangeEvent)
    {
        _levelLocalizationProvider = levelLocalizationProvider;
        _characterProviderBuildMode = characterProviderBuildMode;
        _setLocalizationChangeEvent = setLocalizationChangeEvent;
    }

    public void TrySetCurrentLocalization(SeriaNodeGraphsHandler seriaNodeGraphsHandler, GameStatsHandler gameStatsHandler)
    {
        Debug.Log($"ParticipiteInLoad {_levelLocalizationProvider.ParticipiteInLoad}");

        _currentLocalization = _levelLocalizationProvider.GetCurrentLocalization();
        if (_currentLocalization != null)
        {
            Debug.Log($"TrySetCurrentLocalization");
            SetLocalizationToSeriaTexts(seriaNodeGraphsHandler);
            SetLocalizationToStats(gameStatsHandler);
            SetLocalizationToCharacters();
            Debug.Log($"TrySetCurrentLocalization                                   Execute");

            _setLocalizationChangeEvent.Execute();
            _currentLocalization = null;
        }
    }

    private void SetLocalizationToSeriaTexts(SeriaNodeGraphsHandler seriaNodeGraphsHandler)
    {
        Debug.Log($"SetLocalizationToSeriaTexts");

        for (int i = 0; i < seriaNodeGraphsHandler.SeriaPartNodeGraphs.Count; i++)
        {
            for (int j = 0; j < seriaNodeGraphsHandler.SeriaPartNodeGraphs[i].nodes.Count; j++)
            {
                if (seriaNodeGraphsHandler.SeriaPartNodeGraphs[i].nodes[j] is ILocalizable localizable)
                {
                    foreach (var localizationString in localizable.GetLocalizableContent())
                    {
                        Debug.Log($"localizationString1 {localizationString.DefaultText} {localizationString.Key}");
                        SetText(localizationString);

                        if (localizationString.Key == "str_1AA4873C")
                        {
                            Debug.Log($"   KEY");

                        }
                        Debug.Log($"localizationString2 {localizationString.DefaultText}");
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
            // Debug.Log($"character {character.name}  {character.Name}");
            SetText(character.Name);
        }
    }

    private void SetText(LocalizationString localizationString)
    {
        if (localizationString.Key != null)
        {
            if (_currentLocalization.TryGetValue(localizationString.Key, out string text))
            {
                localizationString.SetText(text);
            }
        }
    }

    public bool IsLocalizationHasBeenChanged()
    {
        return _levelLocalizationProvider.IsLocalizationHasBeenChanged();
    }

    public async UniTaskVoid TrySwitchLanguageFromSettingsChange()
    {
        await _levelLocalizationProvider.TryLoadLocalizationOnSwitchLanguageFromSettings();
        OnTrySwitchLocalization?.Invoke();
        OnEndSwitchLocalization?.Invoke();
    }
}