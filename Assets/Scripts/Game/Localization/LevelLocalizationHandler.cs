
using System.Collections.Generic;
using UniRx;

public class LevelLocalizationHandler
{
    public ReactiveCommand<bool> LevelLocalizationLoad { get; private set; }

    public Dictionary<string, string> _localizationDictionary;
    
    public LevelLocalizationHandler()
    {
        
    }

    public void LoadLocalization(int seriaIndex)
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
                    if (baseNode.StringsToLocalization != null && baseNode.StringsToLocalization.Length > 0)
                    {
                        for (int k = 0; k < baseNode.StringsToLocalization.Length; k++)
                        {
                            if (_localizationDictionary.TryGetValue(baseNode.StringsToLocalization[k].Key, out string value))
                            {
                                baseNode.StringsToLocalization[k].SetText(value);
                            }
                        }
                    }
                }
            }
        }
    }
}