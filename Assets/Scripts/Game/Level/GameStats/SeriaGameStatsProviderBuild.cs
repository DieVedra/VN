
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class SeriaGameStatsProviderBuild : DataProvider<SeriaStatProvider>, IGameStatsProvider
{
    private const string SeriaGameStatsProviderName = "StatProviderSeria";
    private GameStatsHandler _gameStatsHandler;
    public GameStatsHandler GameStatsHandler => _gameStatsHandler;

    public SeriaGameStatsProviderBuild()
    {
        BaseCompositeDisposable = new CompositeDisposable();
        OnLoad.Subscribe(_ =>
        {
            if (_gameStatsHandler == null)
            {
                _gameStatsHandler = new GameStatsHandler();
            }
            _gameStatsHandler.AddNextSeriaStats(_.Stats.ToList());
        }).AddTo(BaseCompositeDisposable);
    }

    public List<Stat> GetStatsFromCurrentSeria(int seriaIndex)
    {
        int seriaNumber = ++seriaIndex;
        List<Stat> stats = new List<Stat>();
        for (int i = 0; i < GetDatas.Count; i++)
        {
            if (GetDatas[i].SeriaNumber > 0)
            {
                if (GetDatas[i].SeriaNumber <= seriaNumber)
                {
                    stats.AddRange(GetDatas[i].Stats);
                }
                else
                {
                    break;
                }
            }
        }
        return stats;
    }

    public async UniTask<int> Init()
    {
        await CreateNames(SeriaGameStatsProviderName);
        if (AssetsFinded == true)
        {
            return NamesCount;
        }
        else
        {
            return 0;
        }
    }
}