using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

public class LevelUISpriteAtlasAssetProvider : AssetLoader<SpriteAtlas>
{
    public const string DialogPanelName = "DialogPanel";
    public const string NarrativePanelName = "NarrativePanel";
    public const string NotificationPanelName = "NotificationPanel";
    
    
    public const string BodyIconName = "BodyIcon";
    public const string ClothesIconName = "ClothesIcon";
    public const string HairstyleIconName = "HairstyleIcon";
    public const string PanelWardrobeName = "PanelWardrobe";
    public const string ArrowName = "Arrow";

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