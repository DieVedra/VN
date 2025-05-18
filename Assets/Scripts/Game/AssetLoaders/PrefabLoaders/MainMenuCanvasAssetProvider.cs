
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MainMenuCanvasAssetProvider : PrefabLoader
{
    private const string _name = "MainMenuCanvas";

    public async UniTask<MainMenuUIView> CreateAsset()
    {
        return await InstantiatePrefab<MainMenuUIView>(_name);
    }
}