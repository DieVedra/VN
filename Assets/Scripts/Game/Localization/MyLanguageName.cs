using UnityEngine;

[System.Serializable]
public class MyLanguageName
{
    private string _mainMenuLocalizationAssetName = "MainMenuTranslate";
    private string _storyLocalizationAssetName = "StoryProviderTranslate";
    [field: SerializeField] public string Name { get; private set; }

    [field: SerializeField] public string Key { get; private set; }

    public string GetPanelsLocalizationAssetName => $"{Key}{_mainMenuLocalizationAssetName}";
    public string GetMenuStoryLocalizationAssetName => $"{Key}{_storyLocalizationAssetName}";
}