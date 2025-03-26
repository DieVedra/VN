using UnityEngine;

public class ViewerCreatorBuildMode : ViewerCreator
{
    public override SpriteViewer CreateViewer(Transform parent)
    {
        return PrefabsProvider.SpriteViewerAssetProvider.CreateSpriteViewer(parent: parent);
    }
}