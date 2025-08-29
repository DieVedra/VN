using System;
using UnityEngine;

public class SaveServiceProvider
{
    public readonly SaveService SaveService;
    private SaveData _saveData;
    
    public int CurrentStoryIndex;
    
    public SaveData SaveData => _saveData;
    public bool SaveHasBeenLoaded { get; private set; }

    public SaveServiceProvider()
    {
        SaveHasBeenLoaded = false;
        SaveService = new SaveService(new JSonSave());
        SetSaveDataDefault();
    }

    public void SaveProgress(Wallet wallet, GlobalSound globalSound, StoriesProvider storiesProvider,
        MainMenuLocalizationHandler mainMenuLocalizationHandler, MainMenuUIProvider mainMenuUIProvider)
    {
        _saveData.Monets = wallet.GetMonetsCount;
        _saveData.Hearts = wallet.GetHeartsCount;
        _saveData.SoundStatus = globalSound.SoundStatus.Value;
        _saveData.StoryDatas = storiesProvider.GetStoryDatas();
        _saveData.LanguageLocalizationKey = mainMenuLocalizationHandler.GetKey;
        if (mainMenuUIProvider.PlayStoryPanelHandler.GetCurrentStoryName != String.Empty)
        {
            _saveData.NameStartStory = mainMenuUIProvider.PlayStoryPanelHandler.GetCurrentStoryName;
        }

        SaveService.Save(_saveData);
    }
    public void LoadSaveData()
    {
        var result = SaveService.LoadData();
        if (result.Item1 == true)
        {
            Debug.Log($"SaveData: true");
            SaveHasBeenLoaded = true;
            var loadedSave = result.Item2;
            SaveData.SoundStatus = loadedSave.SoundStatus;
            SaveData.Monets = loadedSave.Monets;
            SaveData.Hearts = loadedSave.Hearts;
            SaveData.NameStartStory = loadedSave.NameStartStory;
            SaveData.StoryDatas = loadedSave.StoryDatas;
            _saveData.LanguageLocalizationKey = loadedSave.LanguageLocalizationKey;
        }
        else
        {
            SaveHasBeenLoaded = false;
            Debug.Log($"SaveData: false");
        }
    }

    public void TrySetLanguageLocalizationKey(string key)
    {
        if (SaveHasBeenLoaded == false)
        {
            _saveData.LanguageLocalizationKey = key;
        }
    }

    public void TrySetStoryDatas(StoriesProvider storiesProvider)
    {
        if (SaveHasBeenLoaded == false)
        {
            _saveData.StoryDatas = storiesProvider.GetStoryDatas();
        }
    }
    public void TrySetStartStory(string nameStartStory)
    {
        if (SaveHasBeenLoaded == false)
        {
            _saveData.NameStartStory = nameStartStory;
        }
    }
    private void SetSaveDataDefault()
    {
        _saveData = new SaveData();
        _saveData.Monets = 50;
        _saveData.Hearts = 5;
        _saveData.SoundStatus = true;
    }
}