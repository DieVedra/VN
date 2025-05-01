
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class WardrobeSeriaDataProviderBuildMode : IWardrobeSeriaDataProvider
{
    private readonly List<WardrobeSeriaData> _wardrobeSeriaDatas;

    public WardrobeSeriaDataProviderBuildMode()
    {
        _wardrobeSeriaDatas = new List<WardrobeSeriaData>();
    }

    public WardrobeSeriaData GetWardrobeSeriaData(int index)
    {
        if (_wardrobeSeriaDatas.Count > index)
        {
            return _wardrobeSeriaDatas[index];
        }
        else
        {
            return null;
        }
    }

    public UniTask LoadFirstSeriaWardrobeContent()
    {
        
        
        return default;
    }
}