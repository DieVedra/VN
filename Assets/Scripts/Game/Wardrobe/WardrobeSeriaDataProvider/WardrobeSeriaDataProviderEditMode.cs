using System.Collections.Generic;
using UnityEngine;

public class WardrobeSeriaDataProviderEditMode : MonoBehaviour, IWardrobeSeriaDataProvider
{
    [SerializeField] private List<WardrobeSeriaData> _wardrobeSeriaDatas;

    public WardrobeSeriaData GetWardrobeSeriaData(int index)
    {
        int returnIndex = -1;
        for (int i = 0; i < _wardrobeSeriaDatas.Count; ++i)
        {
            if (_wardrobeSeriaDatas[i].MySeriaIndex == index)
            {
                returnIndex = index;
                break;
            }
        }

        if (returnIndex >= 0 )
        {
            return _wardrobeSeriaDatas[index];
        }
        else
        {
            return null;
        }
    }
}