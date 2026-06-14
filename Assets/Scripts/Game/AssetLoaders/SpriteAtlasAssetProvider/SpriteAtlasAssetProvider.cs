using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteAtlasAssetProvider : AssetLoader<SpriteAtlas>
{
    private Dictionary<string, Sprite> _spritesCashed;
    private KeyNameSpriteFinder _keyNameSpriteFinder = new KeyNameSpriteFinder();
    public async UniTask LoadSpriteAtlas(string name)
    {
        await Load(name);
    }
    public Sprite GetSpriteByKey(string key)
    {
        return GetSprite(_keyNameSpriteFinder.GetNameWithKey(GetAsset, key));
    }
    public Sprite GetSprite(string name)
    {
        string newName = _keyNameSpriteFinder.GetNameWithoutKey(GetAsset, name);
        
        if (_spritesCashed == null)
        {
            _spritesCashed = new Dictionary<string, Sprite>();
        }

        if (_spritesCashed.TryGetValue(newName, out Sprite sprite))
        {
            return sprite;
        }
        else
        {
            Sprite newSprite = GetAsset.GetSprite(newName);
            _spritesCashed.Add(newName, newSprite);
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