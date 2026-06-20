using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;

public class SeriaGameStatsProviderBuild : DataProvider<SeriaStatProvider>, IGameStatsProvider
{
    private const string _seriaGameStatsProviderName = "StatProviderSeria";
    private readonly string _fullSeriaGameStatsProviderName;
    private GameStatsHandler _gameStatsHandler;
    public GameStatsHandler GameStatsHandler => _gameStatsHandler;
    public SeriaGameStatsProviderBuild(string storyName)
    {
        _fullSeriaGameStatsProviderName = $"{storyName}{_seriaGameStatsProviderName}";
        BaseCompositeDisposable = new CompositeDisposable();
        _gameStatsHandler = new GameStatsHandler();
        OnLoad.Subscribe(_ =>
        {
            _gameStatsHandler.AddNextSeriaStats(_.Stats);
        }).AddTo(BaseCompositeDisposable);
    }

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
        await CreateNames(_fullSeriaGameStatsProviderName);
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