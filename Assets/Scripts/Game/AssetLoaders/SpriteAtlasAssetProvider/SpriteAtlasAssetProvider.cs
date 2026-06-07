using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteAtlasAssetProvider : AssetLoader<SpriteAtlas>
{
    private Dictionary<string, Sprite> _spritesCashed;

    public async UniTask LoadSpriteAtlas(string name)
    {
        await Load(name);
    }
    public Sprite GetSprite(string name)
    {
        if (_spritesCashed == null)
        {
            _spritesCashed = new Dictionary<string, Sprite>();
        }

        if (_spritesCashed.TryGetValue(name, out Sprite sprite))
        {
            return sprite;
        }
        else
        {
            Sprite newSprite = GetAsset.GetSprite(name);
            _spritesCashed.Add(name, newSprite);
            return newSprite;
        }
    }
    public void Release()
    {
        _spritesCashed.Clear();
        _spritesCashed = null;
        Unload();
    }
}