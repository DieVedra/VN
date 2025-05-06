using System.Collections.Generic;
using UnityEngine;

public class BackgroundDataCreatorWindowList : ScriptableObject
{
    public const string MyPathFolder = "Assets/Plugins/BackgroundDataCreatorWindow";
    public const string MyFileName = "BackgroundDataCreatorWindowList.asset";
    public const string MySpritePropertyName = "_sprites";
    public const string ContentValuesPropertyName = "_backgroundContentValues";
    
    [SerializeField] private List<Sprite> _sprites;
    [SerializeField] private List<BackgroundContentValues> _backgroundContentValues;

}