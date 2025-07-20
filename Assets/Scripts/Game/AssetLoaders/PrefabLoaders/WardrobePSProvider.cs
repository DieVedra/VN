using Cysharp.Threading.Tasks;
using UnityEngine;

public class WardrobePSProvider : PrefabLoader
{
    private const string _name = "WardrobePS";
    
    public async UniTask LoadWardrobePSPrefab()
    {
        await Load(_name);
    }
    public ParticleSystem CreateWardrobePS(Transform parent)
    {
        GameObject gameObject = Object.Instantiate(CashedPrefab, parent: parent);
        gameObject.transform.SetAsFirstSibling();
        return gameObject.GetComponent<ParticleSystem>();
    }
}