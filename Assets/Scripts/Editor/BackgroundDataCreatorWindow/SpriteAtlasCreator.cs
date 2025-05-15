
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteAtlasCreator
{
    public const string Format = ".spriteatlas";
    public const string Name = "Atlas";
    
    public SpriteAtlas CreateAtlas(List<Sprite> list, SpriteAtlasPackingSettings settings, string path, bool crunchedCompression ,bool includeInBuild = false)
    {
        var spriteAtlasAsset = new SpriteAtlas();
        spriteAtlasAsset.SetPackingSettings(settings);
        spriteAtlasAsset.SetIncludeInBuild(includeInBuild);
        TextureImporterPlatformSettings settings1 = new TextureImporterPlatformSettings
        {
            crunchedCompression = crunchedCompression
        };
        spriteAtlasAsset.SetPlatformSettings(settings1);
        SpriteAtlasTextureSettings spriteAtlasTextureSettings = new SpriteAtlasTextureSettings();
        spriteAtlasTextureSettings.readable = true;
        spriteAtlasAsset.SetTextureSettings(spriteAtlasTextureSettings);
        for (int i = 0; i < list.Count; ++i)
        {
            spriteAtlasAsset.Add(new Object[] {list[i]});
        }
        
        AssetDatabase.CreateAsset(spriteAtlasAsset, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return spriteAtlasAsset;
    }
}