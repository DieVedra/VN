using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveServiceProvider
{
    public readonly SaveService SaveService;
    private SaveData _saveData;
    
    public string CurrentStoryKey;
    
    public SaveData SaveData => _saveData;
    public bool SaveHasBeenLoaded { get; private set; }

    public SaveServiceProvider()
    {
        SaveHasBeenLoaded = false;
        SaveService = new SaveService(new JSonSave());
    }

    public void SaveLevelProgress()
    {
        SaveService.Save(_saveData);
    }
    public void SaveProgress(Wallet wallet, GlobalSound globalSound, StoriesProvider storiesProvider,
        PanelsLocalizationHandler panelsLocalizationHandler, MainMenuUIProvider mainMenuUIProvider)
    {
        _saveData.Monets = wallet.GetMonetsCount;
        _saveData.Hearts = wallet.GetHeartsCount;
        _saveData.SoundStatus = globalSound.SoundStatus.Value;
        storiesProvider.TryUpdateStoryDatas(_saveData.StoryDatas);
        
        _saveData.LanguageLocalizationKey = panelsLocalizationHandler.GetKey;
        if (mainMenuUIProvider.PlayStoryPanelHandler.GetCurrentStoryName != String.Empty)
        {
            _saveData.NameStartStory = mainMenuUIProvider.PlayStoryPanelHandler.GetCurrentStoryName;
        }

        SaveService.Save(_saveData);
    }
    public bool LoadSaveData(StartConfig sc)
    {
        if (SaveService.LoadData(out _saveData) == true)
        {
            Debug.Log($"SaveData: true");
            SaveHasBeenLoaded = true;
            // _saveData.SoundStatus = loadedSave.SoundStatus;
            // _saveData.Monets = loadedSave.Monets;
            // _saveData.Hearts = loadedSave.Hearts;
            // _saveData.NameStartStory = loadedSave.NameStartStory;
            // _saveData.StoryDatas = loadedSave.StoryDatas;
            

            // _saveData.LanguageLocalizationKey = loadedSave.LanguageLocalizationKey;
        }
        else
        {
            SaveHasBeenLoaded = false;
            SetSaveDataDefault(sc);
            Debug.Log($"SaveData: false");
        }

        return SaveHasBeenLoaded;
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
            storiesProvider.TryUpdateStoryDatas(_saveData.StoryDatas);
        }
    }
    public void TrySetStartStory(string nameStartStory)
    {
        if (SaveHasBeenLoaded == false)
        {
            _saveData.NameStartStory = nameStartStory;
        }
    }

    private void SetSaveDataDefault(StartConfig sc)
    {
        _saveData = new SaveData
        {
            Monets = sc.Monets,
            Hearts = sc.Hearts,
            SoundStatus = sc.SoundStatus,
            StoryDatas = new Dictionary<string, StoryData>()
        };
    }
}