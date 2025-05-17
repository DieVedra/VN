
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MainMenuCanvasAssetProvider : PrefabLoader
{
    private const string _name = "MainMenuCanvas";

    public async UniTask<MainMenuUIView> CreateAsset()
    {
        GameObject instantiated = await InstantiatePrefab(_name);
        if (instantiated.TryGetComponent(out MainMenuUIView mainMenuUIView))
        {
            return mainMenuUIView;
        }
        else
        {
            return default;
        }
    }
}