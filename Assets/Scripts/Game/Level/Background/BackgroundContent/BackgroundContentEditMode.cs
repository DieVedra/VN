
using UnityEngine;

public class BackgroundContentEditMode : BackgroundContent
{
    private SpriteRenderer _spriteRendererPrefab;

    public void SetPrefab(SpriteRenderer spriteRendererPrefab)
    {
        _spriteRendererPrefab = spriteRendererPrefab;
    }

    protected override SpriteRenderer CreateAddContent()
    {
        return Instantiate(_spriteRendererPrefab, SpriteRenderer.transform, true);
    }
}