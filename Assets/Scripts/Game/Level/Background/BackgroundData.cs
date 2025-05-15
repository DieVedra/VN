using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;


[CreateAssetMenu(fileName = "BackgroundDataSeria", menuName = "Background/BackgroundDataSeria", order = 51)]
public class BackgroundData : ScriptableObject
{
    public const string AssetName = "BackgroundDataSeria";
    public const string Format = ".asset";
    public const string SpriteAtlasPropertyName = "_spriteAtlas";
    public const string ContentValuesPropertyName = "_backgroundContentValues";
    [SerializeField] private SpriteAtlas _spriteAtlas;
    [SerializeField] private List<BackgroundContentValues> _backgroundContentValues;
    
    public Sprite GetSprite(string name) => _spriteAtlas.GetSprite(name);
    public IReadOnlyList<BackgroundContentValues> BackgroundContentValues => _backgroundContentValues;
}