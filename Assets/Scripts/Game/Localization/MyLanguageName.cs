using UnityEngine;

[System.Serializable]
public class MyLanguageName
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string Key { get; private set; }
    [field: SerializeField] public string MainMenuLocalizationAssetName { get; private set; }
}