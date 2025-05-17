
using UnityEngine;

public class SpriteRendererCreatorBuild : SpriteRendererCreator
{
    private readonly SpriteRendererAssetProvider _spriteRendererAssetProvider;

    public SpriteRendererCreatorBuild(SpriteRendererAssetProvider spriteRendererAssetProvider)
    {
        _spriteRendererAssetProvider = spriteRendererAssetProvider;
    }

    public override SpriteRenderer CreateAddContent(Transform parent)
    {
        return _spriteRendererAssetProvider.CreateSpriteRenderer(parent);
        
    }
}