
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteAtlasCreator
{
    public const string Format = ".spriteatlas";
    public const string Name = "Atlas";
    
    public SpriteAtlas CreateAtlas(SerializedProperty listSerializedProperty, string path)
    {
        var spriteAtlasAsset = new SpriteAtlas();
        for (int i = 0; i < listSerializedProperty.arraySize; i++)
        {
            Sprite sprite = listSerializedProperty.GetArrayElementAtIndex(i).objectReferenceValue as Sprite;
            spriteAtlasAsset.Add(new Object[] {sprite});
        }
        AssetDatabase.CreateAsset(spriteAtlasAsset, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return spriteAtlasAsset;
    }
}