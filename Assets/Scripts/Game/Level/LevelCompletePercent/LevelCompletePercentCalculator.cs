using System.Collections.Generic;

public class LevelCompletePercentCalculator: ILevelPercentProvider
{
    private const int _maxPercentValue = 100;
    private const int _minPercentValue = 0;
    private readonly GameSeriesHandler _gameSeriesHandler;
    private readonly int _allSeriesCount;

    public LevelCompletePercentCalculator(GameSeriesHandler gameSeriesHandler, int allSeriesCount)
    {
        _gameSeriesHandler = gameSeriesHandler;
        _allSeriesCount = allSeriesCount;
    }

    public int GetCalculateLevelProgressPercent()
    {
        return CalculateLevelProgressPercent(
             _gameSeriesHandler.GetCurrentSeriaNodeGraphs, 
             _gameSeriesHandler.CurrentNodeIndex,
             _gameSeriesHandler.CurrentNodeGraphIndex,
             _gameSeriesHandler.CurrentSeriaIndex);
    }
    private int CalculateLevelProgressPercent(IReadOnlyList<SeriaPartNodeGraph> seriaPartNodeGraphs, int currentNodeIndex, int currentGraphIndex, int currentSeriaIndex)
    {
        int generalArifmetic = 0;
        for (int i = 0; i < _allSeriesCount; i++)
        {
            if (i < currentSeriaIndex)
            {
                generalArifmetic += _maxPercentValue;
            }
            else if(i > currentSeriaIndex)
            {
                generalArifmetic += _minPercentValue;
            }
            else if(i == currentSeriaIndex)
            {
                generalArifmetic += GetSeriaProgressPercent(seriaPartNodeGraphs, currentNodeIndex, currentGraphIndex);
            }
        }
        return generalArifmetic / _allSeriesCount;
    }
    private int GetSeriaProgressPercent(IReadOnlyList<SeriaPartNodeGraph> seriaPartNodeGraphs, int currentNodeIndex, int currentGraphIndex)
    {
        int graphCount = seriaPartNodeGraphs.Count;
        int generalArifmetic = 0;
        for (int i = 0; i < graphCount; i++)
        {
            if (i < currentGraphIndex)
            {
                generalArifmetic += _maxPercentValue;
            }
            else if(i > currentGraphIndex)
            {
                generalArifmetic += _minPercentValue;
            }
            else if(i == currentGraphIndex)
            {
                generalArifmetic += (_maxPercentValue / seriaPartNodeGraphs[i].nodes.Count) * currentNodeIndex;
            }
        }
        return generalArifmetic / graphCount;
    }
}