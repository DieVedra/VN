using Cysharp.Threading.Tasks;
using UnityEngine;

public class PhoneUIPrefabAssetProvider : PrefabLoader
{
    private const string _name = "PhoneView";

    public async UniTask<PhoneUIView> CreatePhoneUIView(Transform parent = null)
    {
        return await InstantiatePrefab<PhoneUIView>(_name, parent);
    }
    public void UnloadAsset()
    {
        base.Unload();
    }  
}