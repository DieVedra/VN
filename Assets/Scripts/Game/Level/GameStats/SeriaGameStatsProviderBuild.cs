using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;

public class SeriaGameStatsProviderBuild : DataProvider<SeriaStatProvider>, IGameStatsProvider
{
    private const string SeriaGameStatsProviderName = "StatProviderSeria";
    private GameStatsHandler _gameStatsHandler;
    public List<T> GetEmptyTStat<T>(int seriaIndex) where T : BaseStat
    {
        int seriaNumber = ++seriaIndex;
        List<T> stats = new List<T>();
        Stat stat;
        for (int i = 0; i < GetDatas.Count; i++)
        {
            if (GetDatas[i].SeriaNumber > 0 && GetDatas[i].SeriaNumber <= seriaNumber)
            {
                for (int j = 0; j < GetDatas[i].Stats.Count; j++)
                {
                    stat = GetDatas[i].Stats[j];
                    var newStat = new Stat(stat.NameText, stat.NameKey, stat.Value, stat.ColorField);
                    T type = (T)(object)newStat;
                    stats.Add(type);
                }
            }
            else
            {
                break;
            }
        }
        return stats;
    }

    public GameStatsHandler GameStatsHandler => _gameStatsHandler;
    public SeriaGameStatsProviderBuild()
    {
        BaseCompositeDisposable = new CompositeDisposable();
        _gameStatsHandler = new GameStatsHandler();
        OnLoad.Subscribe(_ =>
        {
            _gameStatsHandler.AddNextSeriaStats(_.Stats);
        }).AddTo(BaseCompositeDisposable);
    }

    public List<Stat> GetEmptyStatsFromCurrentSeria(int seriaIndex)
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