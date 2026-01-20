using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

public class LevelUISpriteAtlasAssetProvider : AssetLoader<SpriteAtlas>
{
    public const string DialogPanelName = "DialogPanel";
    public const string NarrativePanelName = "NarrativePanel";
    public const string NotificationPanelName = "NotificationPanel";
    public async UniTask LoadSpriteAtlas(string name)
    {
        await Load(name);
    }
    public Sprite GetSprite(string name)
    {
        return GetAsset.GetSprite(name);
    }
    public void Release()
    {
        Unload();
    }
}