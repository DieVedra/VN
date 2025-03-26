
using UnityEngine;

public class BackgroundContentBuildMode : BackgroundContent
{
    protected override SpriteRenderer CreateAddContent()
    {
        return PrefabsProvider.SpriteRendererAssetProvider.CreateSpriteRenderer(SpriteRenderer.transform);
    }
}