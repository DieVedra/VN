
using System.Collections.Generic;
using UniRx;

public class LevelLocalizationHandler
{
    public ReactiveCommand<bool> LevelLocalizationLoad { get; private set; }

    public Dictionary<string, string> _localizationDictionary;
    
    public LevelLocalizationHandler()
    {
        
    }

    public void LoadLocalization(int seriaIndex, string language)
    {
        
    }

    public void SetLocalizationToSeriaTexts(SeriaNodeGraphsHandler seriaNodeGraphsHandler)
    {
        for (int i = 0; i < seriaNodeGraphsHandler.SeriaPartNodeGraphs.Count; i++)
        {
            for (int j = 0; j < seriaNodeGraphsHandler.SeriaPartNodeGraphs[i].nodes.Count; j++)
            {
                if (seriaNodeGraphsHandler.SeriaPartNodeGraphs[i].nodes[j] is BaseNode baseNode)
                {
                    if (baseNode.StringsLocalization != null && baseNode.StringsLocalization.Count > 0)
                    {
                        for (int k = 0; k < baseNode.StringsLocalization.Count; k++)
                        {
                            if (_localizationDictionary.TryGetValue(baseNode.StringsLocalization[k].Key, out string value))
                            {
                                baseNode.StringsLocalization[k].SetText(value);
                            }
                        }
                    }
                }
            }
        }
    }
}