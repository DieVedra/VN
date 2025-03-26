using UnityEngine;

public class SpriteRendererCreatorEditor : SpriteRendererCreator
{
    private readonly SpriteRenderer _prefab;

    public SpriteRendererCreatorEditor(SpriteRenderer prefab)
    {
        _prefab = prefab;
    }

    public override SpriteRenderer CreateAddContent(Transform parent)
    {
        return Object.Instantiate(_prefab, parent: parent);
    }
}