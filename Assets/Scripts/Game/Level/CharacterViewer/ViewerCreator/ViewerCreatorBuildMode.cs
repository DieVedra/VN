using UnityEngine;

public class ViewerCreatorBuildMode : ViewerCreator
{
    private readonly SpriteViewerAssetProvider _spriteViewerAssetProvider;

    public ViewerCreatorBuildMode(SpriteViewerAssetProvider spriteViewerAssetProvider)
    {
        _spriteViewerAssetProvider = spriteViewerAssetProvider;
    }

    public override SpriteViewer CreateViewer(Transform parent)
    {
        return _spriteViewerAssetProvider.CreateSpriteViewer(parent: parent);
    }
}