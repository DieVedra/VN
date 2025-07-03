using UnityEngine;

public class LevelLocalizationHandler
{
    private readonly LevelLocalizationProvider _levelLocalizationProvider;

    public LevelLocalizationHandler(LevelLocalizationProvider levelLocalizationProvider)
    {
        _levelLocalizationProvider = levelLocalizationProvider;
    }

    public void TrySetCurrentLocalization(SeriaNodeGraphsHandler seriaNodeGraphsHandler, GameStatsHandler gameStatsHandler)
    {
        if (_levelLocalizationProvider.ParticipiteInLoad)
        {
            SetLocalizationToSeriaTexts(seriaNodeGraphsHandler);
            SetLocalizationToStats(gameStatsHandler);
        }

    }

    private void SetLocalizationToSeriaTexts(SeriaNodeGraphsHandler seriaNodeGraphsHandler)
    {
        for (int i = 0; i < seriaNodeGraphsHandler.SeriaPartNodeGraphs.Count; i++)
        {
            for (int j = 0; j < seriaNodeGraphsHandler.SeriaPartNodeGraphs[i].nodes.Count; j++)
            {
                if (seriaNodeGraphsHandler.SeriaPartNodeGraphs[i].nodes[j] is BaseNode baseNode)
                {
                    if (baseNode is HeaderNode headerNode)
                    {
                        Debug.Log($"headerNode.StringsToLocalization1[0].GetHashCode();   {headerNode.StringsToLocalization1[0].GetHashCode()}");
                        Debug.Log($"headerNode.hash;   {headerNode.hash}");

                    }
                    Debug.Log(11);
                    if (baseNode.StringsToLocalization1 != null && baseNode.StringsToLocalization1.Length > 0)
                    {                   
                        Debug.Log($"{baseNode.namenode}");
                        for (int k = 0; k < baseNode.StringsToLocalization1.Length; k++)
                        {
                            Debug.Log($"22 {k}");
                            if (_levelLocalizationProvider.CurrentLocalization.TryGetValue(baseNode.StringsToLocalization1[k].Key, out string value))
                            {
                                Debug.Log($"22 {k}");

                                baseNode.StringsToLocalization1[k].SetText(value);
                            }
                        }
                    }
                    // if (baseNode.StringsLocalization != null && baseNode.StringsLocalization.Count > 0)
                    // {                   
                    //     Debug.Log($"{baseNode.namenode}");
                    //     for (int k = 0; k < baseNode.StringsLocalization.Count; k++)
                    //     {
                    //         Debug.Log($"22 {k}");
                    //         if (_currentLocalization.TryGetValue(baseNode.StringsLocalization[k].Key, out string value))
                    //         {
                    //             Debug.Log($"22 {k}");
                    //
                    //             baseNode.StringsLocalization[k].SetText(value);
                    //         }
                    //     }
                    // }
                }
            }
        }
    }

    private void SetLocalizationToStats(GameStatsHandler gameStatsHandler)
    {
        for (int i = 0; i < gameStatsHandler.Stats.Count; i++)
        {
            if (_levelLocalizationProvider.CurrentLocalization.TryGetValue(gameStatsHandler.Stats[i].LocalizationName, out string text))
            {
                gameStatsHandler.Stats[i].LocalizationName.SetText(text);
            }
        }
    }
}