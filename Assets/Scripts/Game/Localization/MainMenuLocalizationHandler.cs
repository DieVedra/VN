using System.Collections.Generic;
using System.Globalization;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class MainMenuLocalizationHandler : ILocalizationChanger
{
    private const string _defaultLanguageKey = "en";
    // private const string DefaultLanguageKey = "ru";
    private MainMenuLocalizationInfoHolder _mainMenuLocalizationInfoHolder;
    private SaveData _saveData;
    private StoriesProvider _storiesProvider;
    private MyLanguageName _currentMyLanguageName;
    private ReactiveProperty<int> _currentLanguageKeyIndex;
    private Dictionary<int, Dictionary<string, string>> _dictionariesMainMenuTranslates;
    private Dictionary<int, Dictionary<string, string>> _dictionaryStoryTranslates;
    // private List<LocalizationString> _localizationStrings;
    private LocalizationFileProvider _loader;
    
    public ReactiveCommand LanguageChanged { get; private set; }

    public MyLanguageName CurrentLanguageName => _mainMenuLocalizationInfoHolder.LanguageNames[_currentLanguageKeyIndex.Value];
    public string GetName => _mainMenuLocalizationInfoHolder.LanguageNames[_currentLanguageKeyIndex.Value].Name;
    public string GetKey => _mainMenuLocalizationInfoHolder.LanguageNames[_currentLanguageKeyIndex.Value].Key;
    public int GetMyLanguageNamesCount => _mainMenuLocalizationInfoHolder.LanguageNames.Count;
    public IReactiveProperty<int> CurrentLanguageKeyIndex => _currentLanguageKeyIndex;
    
    public MainMenuLocalizationHandler()
    {
        _currentLanguageKeyIndex = new ReactiveProperty<int>();
        LanguageChanged = new ReactiveCommand();
        _loader = new LocalizationFileProvider();
    }

    public async UniTask Init(SaveData saveData, StoriesProvider storiesProvider)
    {
        _saveData = saveData;
        _storiesProvider = storiesProvider;
        _mainMenuLocalizationInfoHolder = await new LocalizationHandlerAssetProvider().LoadLocalizationHandlerAsset();
        TryDefineLanguageKey();
        await LoadCurrentLanguage();
        SetLanguageMainMenuAndStory();

        _currentLanguageKeyIndex.Skip(1).Subscribe(_ =>
        {
            ChangeLanguage();
        });
    }

    private void TryDefineLanguageKey()
    {
        string systemLanguageKey = "";
        if (_saveData != null && string.IsNullOrEmpty(_saveData.LanguageLocalizationKey) == false )
        {
            systemLanguageKey = _saveData.LanguageLocalizationKey;
        }
        else
        {
            // systemLanguageKey = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

            if (string.IsNullOrEmpty(systemLanguageKey) == true)
            {
                systemLanguageKey = _defaultLanguageKey;
            }
        }

        for (int i = 0; i < _mainMenuLocalizationInfoHolder.LanguageNames.Count; i++)
        {
            _currentLanguageKeyIndex.Value = i;
            if (_mainMenuLocalizationInfoHolder.LanguageNames[i].Key == systemLanguageKey)
            {
                _currentMyLanguageName = _mainMenuLocalizationInfoHolder.LanguageNames[i];
                break;
            }
        }
    }

    private async UniTask LoadCurrentLanguage()
    {
        _dictionariesMainMenuTranslates = new Dictionary<int, Dictionary<string, string>>();
        _dictionariesMainMenuTranslates[_currentLanguageKeyIndex.Value] = await LoadLanguageAsset(_currentMyLanguageName.GetMainMenuLocalizationAssetName);
        
        _dictionaryStoryTranslates = new Dictionary<int, Dictionary<string, string>>();
        _dictionaryStoryTranslates[_currentLanguageKeyIndex.Value] = await LoadLanguageAsset(_currentMyLanguageName.GetStoryLocalizationAssetName);
    }

    public async UniTask LoadAllLanguagesForMenu()
    {
        for (int i = 0; i < _mainMenuLocalizationInfoHolder.LanguageNames.Count; i++)
        {
            if (i == _currentLanguageKeyIndex.Value)
            {
                continue;
            }
            _dictionariesMainMenuTranslates[i] = await LoadLanguageAsset(_mainMenuLocalizationInfoHolder.LanguageNames[i].GetMainMenuLocalizationAssetName);
            _dictionaryStoryTranslates[i] = await LoadLanguageAsset(_mainMenuLocalizationInfoHolder.LanguageNames[i].GetStoryLocalizationAssetName);
        }
    }
    private void ChangeLanguage()
    {
        SetLanguageMainMenuAndStory();
        LanguageChanged.Execute();
    }
    private async UniTask<Dictionary<string, string>> LoadLanguageAsset(string localizationAssetName)
    {
        return await _loader.LoadLocalizationFile(localizationAssetName);
    }
    private void SetLanguageMainMenuAndStory()
    {
        var dictionaryMainMenuTranslate = _dictionariesMainMenuTranslates[_currentLanguageKeyIndex.Value];
        var dictionaryStoryTranslates = _dictionaryStoryTranslates[_currentLanguageKeyIndex.Value];
        
        if (_saveData != null)
        {
            _saveData.LanguageLocalizationKey = _mainMenuLocalizationInfoHolder.LanguageNames[_currentLanguageKeyIndex.Value].Key;
        }

        foreach (var story in _storiesProvider.Stories)
        {
            if (dictionaryStoryTranslates.TryGetValue(story.Description.Key, out string text))
            {
                story.Description.SetText(text);
            }
        }
        foreach (var localizationString in LocalizationString.LocalizationStrings)
        {
            if (dictionaryMainMenuTranslate.TryGetValue(localizationString.Key, out string text))
            {
                localizationString.SetText(text);
            }
        }
    }
}