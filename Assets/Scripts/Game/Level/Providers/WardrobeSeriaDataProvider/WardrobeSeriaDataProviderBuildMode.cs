
using Cysharp.Threading.Tasks;

public class WardrobeSeriaDataProviderBuildMode : DataProvider<WardrobeSeriaData>, IWardrobeSeriaDataProvider
{
    private const string _wardrobeDataSeriaNameAsset = "WardrobeDataSeria";
    public WardrobeSeriaData GetWardrobeSeriaData(int index)
    {
        if (GetDatas.Count > index)
        {
            return GetDatas[index];
        }
        else
        {
            return null;
        }
    }

    public async UniTask Init()
    {
        await CreateNames(_wardrobeDataSeriaNameAsset);
    }
}