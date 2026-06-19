using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SaveServiceProvider
{
    public readonly SaveService SaveService;
    private SaveData _saveData;
    private Wallet _wallet;
    private GlobalSound _globalSound;
    private StoriesProvider _storiesProvider;
    private PanelsLocalizationHandler _panelsLocalizationHandler;
    private MainMenuUIProvider _mainMenuUIProvider;
    private StartConfig _startConfig;
    public string CurrentStoryKey;

    private bool _isInitedKey;
    public SaveData SaveData => _saveData;
    public bool SaveHasBeenLoaded { get; private set; }

    public SaveServiceProvider(StartConfig sc)
    {
        SaveHasBeenLoaded = false;
        _startConfig = sc;
        switch (sc.SaveMethod)
        {
            case SaveMethod.JsonSave:
                SaveService = new SaveService(new JSonSave());
                break;
            
            case SaveMethod.BinarySave:
                SaveService = new SaveService(new BinarySave());
                break;
            
            case SaveMethod.UnityCloudSave:
                SaveService = new SaveService(new UnityCloudSaveMethod());
                break;
        }

        _isInitedKey = false;
    }

    public void Init(Wallet wallet, GlobalSound globalSound, StoriesProvider storiesProvider,
        PanelsLocalizationHandler panelsLocalizationHandler, MainMenuUIProvider mainMenuUIProvider)
    {
        _wallet =  wallet;
        _globalSound = globalSound;
        _storiesProvider = storiesProvider;
        _panelsLocalizationHandler = panelsLocalizationHandler;
        _mainMenuUIProvider = mainMenuUIProvider;
        _isInitedKey = true;
    }
    public async UniTask SaveLevelProgress()
    {
        await SaveService.Save(_saveData);
    }
    public async UniTask SaveFromMainMenu()
    {
        if (_isInitedKey)
        {
            _saveData.Monets = _wallet.GetMonetsCount;
            _saveData.Hearts = _wallet.GetHeartsCount;
            _saveData.SoundStatus = _globalSound.SoundStatus.Value;
            _storiesProvider.TryUpdateStoryDatas(_saveData.StoryDatas);
            _saveData.LanguageLocalizationKey = _panelsLocalizationHandler.GetKey;
            if (_mainMenuUIProvider.PlayStoryPanelHandler?.GetCurrentStoryName != String.Empty)
            {
                _saveData.NameStartStory = _mainMenuUIProvider.PlayStoryPanelHandler.GetCurrentStoryName;
            }

            await SaveService.Save(_saveData);
        }
    }
    public async UniTask<bool> LoadSaveData()
    {
        _saveData = await SaveService.LoadData();
        if (_saveData != null)
        {
            Debug.Log($"SaveData: true");
            SaveHasBeenLoaded = true;
        }
        else
        {
            SaveHasBeenLoaded = false;
            SetSaveDataDefault();
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

    private void SetSaveDataDefault()
    {
        _saveData = new SaveData
        {
            Monets = _startConfig.Monets,
            Hearts = _startConfig.Hearts,
            SoundStatus = _startConfig.SoundStatus,
            StoryDatas = new Dictionary<string, StoryData>()
        };
    }

    // public void DeleteProgressByStory(string key)
    // {
    //     if (_saveData.StoryDatas.TryGetValue(key, out StoryData storyData))
    //     {
    //         _saveData.StoryDatas[key] = GetSkippedStoryData(storyData);
    //     }
    // }

    public void DeleteAllProgress()
    {
        foreach (var story in _storiesProvider.Stories)
        {
            story.ResetProgress();
        }
    }

    // private StoryData GetSkippedStoryData(StoryData oldStoryData)
    // {
    //     return new StoryData
    //     {
    //         StoryName = oldStoryData.StoryName, NameUISpriteAtlas = oldStoryData.NameUISpriteAtlas, CurrentNodeGraphIndex = 0,
    //         CurrentNodeIndex = 0, PutOnSwimsuitKey = false, StoryIndex = oldStoryData.StoryIndex
    //     };
    // }
}