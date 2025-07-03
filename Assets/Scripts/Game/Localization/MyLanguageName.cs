using UnityEngine;

[System.Serializable]
public class MyLanguageName
{
    private string _mainMenuLocalizationAssetName = "MainMenuTranslate";
    private string _storyLocalizationAssetName = "StoryProviderTranslate";
    [field: SerializeField] public string Name { get; private set; }

    [field: SerializeField] public string Key { get; private set; }

    public string GetMainMenuLocalizationAssetName => $"{Key}{_mainMenuLocalizationAssetName}";
    public string GetStoryLocalizationAssetName => $"{Key}{_storyLocalizationAssetName}";
}