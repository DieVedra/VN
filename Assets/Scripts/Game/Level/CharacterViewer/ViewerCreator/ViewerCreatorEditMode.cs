
using UnityEngine;

public class ViewerCreatorEditMode : ViewerCreator
{
    private SpriteViewer _prefab;

    public ViewerCreatorEditMode(SpriteViewer prefab)
    {
        _prefab = prefab;
    }

    public override SpriteViewer CreateViewer(Transform parent)
    {
        return Object.Instantiate(_prefab, parent: parent);
    }
}