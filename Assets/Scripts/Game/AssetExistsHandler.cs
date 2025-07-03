using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetExistsHandler
{
    private const int _attemptsCount = 3;
    private const int _startNameNumber = 1;
    private readonly int _assetsCount;

    private bool[] _resultCheckAssetsExists;
    
    public AssetExistsHandler(int assetsCount = 20)
    {
        _assetsCount = assetsCount;
    }

    public async UniTask<List<string>> CheckExistsAssetsNames(string assetName)
    {
        List<string> resultsExistNames = new List<string>();
        
        List<string> names;
        int startNameNumber = _startNameNumber;
        int currentAssetCount = _assetsCount;
        do
        {
            names = GetCreatedNames(assetName, startNameNumber, currentAssetCount);
            resultsExistNames.AddRange(await CheckAssetsExists(names));
            startNameNumber += _assetsCount;
            currentAssetCount += _assetsCount;

        } while (CheckExistenceByResult(_resultCheckAssetsExists));
        _resultCheckAssetsExists = null;
        return resultsExistNames;
    }

    private async UniTask<List<string>> CheckAssetsExists(List<string> names)
    {
        List<UniTask<bool>> tasks = new List<UniTask<bool>>(names.Count);
        for (int i = 0; i < names.Count; ++i)
        {
            tasks.Add(CheckAssetExists(names[i]));
        }
        _resultCheckAssetsExists = await UniTask.WhenAll(tasks);
        List<string> existNames = new List<string>(names.Count);
        for (int i = 0; i < names.Count; ++i)
        {
            if (_resultCheckAssetsExists[i] == true)
            {
                existNames.Add(names[i]);
            }
        }
        return existNames;
    }
    public async UniTask<bool> CheckAssetExists(string assetId)
    {
        bool result = false;
        for (int i = 0; i < _attemptsCount; i++)
        {
            var locationsHandle = Addressables.LoadResourceLocationsAsync(assetId);
            await locationsHandle.Task;
            if (locationsHandle.Status != AsyncOperationStatus.Succeeded || locationsHandle.Result.Count == 0)
            {
                result = false;
            }
            else
            {
                result = true;
            }
            Addressables.Release(locationsHandle);
            if (result == true)
            {
                break;
            }
        }
        return result;
    }

    private List<string> GetCreatedNames(string assetName, int startNameNumber, int assetsCount)
    {
        List<string> names = new List<string>();
        for (int i = startNameNumber; i <= assetsCount; ++i)
        {
            names.Add($"{assetName}{i}");
        }
        return names;
    }

    private bool CheckExistenceByResult(bool[] result)
    {
        bool returnResult = false;
        for (int i = 0; i < result.Length; i++)
        {
            if (result[i] == true)
            {
                returnResult = true;
            }
        }
        return returnResult;
    }
}