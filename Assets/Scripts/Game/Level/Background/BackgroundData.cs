using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.U2D;


[CreateAssetMenu(fileName = "BackgroundDataSeria", menuName = "Background/BackgroundDataSeria", order = 51)]
public class BackgroundData : ScriptableObject
{
    public const string AssetName = "BackgroundDataSeria";
    public const string Format = ".asset";
    public const string SpriteAtlasPropertyName = "_spriteAtlas";
    public const string ContentValuesPropertyName = "_backgroundContentValues";
    [SerializeField/*, ReadOnly*/] private SpriteAtlas _spriteAtlas;
    [SerializeField] private List<BackgroundContentValues> _backgroundContentValues;
}