
public class LevelLocalizationHandler
{
    private readonly LevelLocalizationProvider _levelLocalizationProvider;
    private readonly CharacterProviderBuildMode _characterProviderBuildMode;

    public LevelLocalizationHandler(LevelLocalizationProvider levelLocalizationProvider, CharacterProviderBuildMode characterProviderBuildMode)
    {
        _levelLocalizationProvider = levelLocalizationProvider;
        _characterProviderBuildMode = characterProviderBuildMode;
    }

    public void TrySetCurrentLocalization(SeriaNodeGraphsHandler seriaNodeGraphsHandler, GameStatsHandler gameStatsHandler)
    {
        if (_levelLocalizationProvider.ParticipiteInLoad)
        {
            SetLocalizationToSeriaTexts(seriaNodeGraphsHandler);
            SetLocalizationToStats(gameStatsHandler);
            SetLocalizationToCharacters();
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
        if (_levelLocalizationProvider.CurrentLocalization.TryGetValue(localizationString.Key, out string text))
        {
            localizationString.SetText(text);
        }
    }
}