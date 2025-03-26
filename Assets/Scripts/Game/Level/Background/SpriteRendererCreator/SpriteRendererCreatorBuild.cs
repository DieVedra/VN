
using UnityEngine;

public class SpriteRendererCreatorBuild : SpriteRendererCreator
{
    public override SpriteRenderer CreateAddContent(Transform parent)
    {
        return PrefabsProvider.SpriteRendererAssetProvider.CreateSpriteRenderer(parent);
    }
}