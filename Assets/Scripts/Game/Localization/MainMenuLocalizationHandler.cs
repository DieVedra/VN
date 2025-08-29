using System;
using System.Collections.Generic;
using System.Globalization;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class MainMenuLocalizationHandler : ILocalizationChanger
{
    private const string _defaultLanguageKey = "en";
    // private const string DefaultLanguageKey = "ru";
    private readonly LocalizationFileProvider _loader;
    private MainMenuLocalizationInfoHolder _mainMenuLocalizationInfoHolder;
    private SaveData _saveData;
    private StoriesProvider _storiesProvider;
    private MyLanguageName _currentMyLanguageName;
    private ReactiveProperty<int> _currentLanguageKeyIndex;
    private Dictionary<int, Dictionary<string, string>> _dictionariesMainMenuTranslates;
    private Dictionary<int, Dictionary<string, string>> _dictionaryStoryTranslates;


    // private List<LocalizationString> _localizationStrings;

    public ReactiveCommand LanguageChanged { get; private set; }

    public MyLanguageName CurrentLanguageName => _mainMenuLocalizationInfoHolder.LanguageNames[_currentLanguageKeyIndex.Value];
    public string GetName => _mainMenuLocalizationInfoHolder.LanguageNames[_currentLanguageKeyIndex.Value].Name;
    public string GetKey => _mainMenuLocalizationInfoHolder.LanguageNames[_currentLanguageKeyIndex.Value].Key;
    public int GetMyLanguageNamesCount => _mainMenuLocalizationInfoHolder.LanguageNames.Count;
    public IReactiveProperty<int> CurrentLanguageKeyIndex => _currentLanguageKeyIndex;
    
    public MainMenuLocalizationHandler()
    {
        // _currentLanguageKeyIndex = new ReactiveProperty<int>();
        // LanguageChanged = new ReactiveCommand();
        _loader = new LocalizationFileProvider();
        Debug.Log($"Constructor MainMenuLocalizationHandler");

    }

    public async UniTask Init(SaveData saveData, StoriesProvider storiesProvider)
    {
        Debug.Log($"MainMenuLocalizationHandler  Init");
        _currentLanguageKeyIndex?.Dispose();
        _currentLanguageKeyIndex = new ReactiveProperty<int>();
        LanguageChanged?.Dispose();
        LanguageChanged = new ReactiveCommand();

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
        Debug.Log($"TryDefineLanguageKey1111   {_mainMenuLocalizationInfoHolder.LanguageNames.Count}");

        for (int i = 0; i < _mainMenuLocalizationInfoHolder.LanguageNames.Count; i++)
        {
            _currentLanguageKeyIndex.Value = i;

            if (_mainMenuLocalizationInfoHolder.LanguageNames[i].Key == systemLanguageKey)
            {
                _currentMyLanguageName = _mainMenuLocalizationInfoHolder.LanguageNames[i];
                break;
            }

            Debug.Log($"TryDefineLanguageKey() {_currentLanguageKeyIndex.Value}");
        }
        Debug.Log($"systemLanguageKey: {systemLanguageKey}  _currentLanguageKeyIndex.Value: {_currentLanguageKeyIndex.Value} _currentMyLanguageName: {_currentMyLanguageName.Name}");

    }

    private async UniTask LoadCurrentLanguage()
    {
        _dictionariesMainMenuTranslates = new Dictionary<int, Dictionary<string, string>>();
        _dictionariesMainMenuTranslates[_currentLanguageKeyIndex.Value] = await LoadLanguageAsset(_currentMyLanguageName.GetMainMenuLocalizationAssetName);
        
        _dictionaryStoryTranslates = new Dictionary<int, Dictionary<string, string>>();
        _dictionaryStoryTranslates[_currentLanguageKeyIndex.Value] = await LoadLanguageAsset(_currentMyLanguageName.GetStoryLocalizationAssetName);
        
        Debug.Log($"LoadCurrentLanguage   _currentLanguageKeyIndex.Value {_currentLanguageKeyIndex.Value}");
        foreach (var VARIABLE in _dictionariesMainMenuTranslates)
        {
            Debug.Log($"key {VARIABLE.Key}  ");

        }
        Debug.Log($"-------------");
        foreach (var VARIABLE in _dictionaryStoryTranslates)
        {
            Debug.Log($"key {VARIABLE.Key}  ");

        }
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
        Dictionary<string, string> dictionaryMainMenuTranslate = null;
        Dictionary<string, string> dictionaryStoryTranslates = null;
        if (_dictionariesMainMenuTranslates.TryGetValue(_currentLanguageKeyIndex.Value, out var value1))
        {
            dictionaryMainMenuTranslate = value1;
        }
        else
        {
            Debug.LogError($"DictionariesMainMenuTranslates.TryGetValue: False   key: {_currentLanguageKeyIndex.Value}");
        }
        
        if (_dictionaryStoryTranslates.TryGetValue(_currentLanguageKeyIndex.Value, out var value2))
        {
            dictionaryStoryTranslates = value2;
        }
        else
        {
            Debug.LogError($"DictionaryStoryTranslates.TryGetValue: False   key: {_currentLanguageKeyIndex.Value}");
        }
        
        // var dictionaryMainMenuTranslate = _dictionariesMainMenuTranslates[_currentLanguageKeyIndex.Value];
        // var dictionaryStoryTranslates = _dictionaryStoryTranslates[_currentLanguageKeyIndex.Value];
        
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

        if (dictionaryMainMenuTranslate == null)
        {
            Debug.Log($"44 dictionaryMainMenuTranslate == null");

        }


        foreach (var VARIABLE in dictionaryMainMenuTranslate)
        {
            Debug.Log($"test1  {VARIABLE.Value}  {VARIABLE.Key}");

        }
        
        
        foreach (var localizationString in LocalizationString.LocalizationStrings)
        {
            Debug.Log($"test2  {localizationString.Key}  {localizationString.DefaultText}");

            if (dictionaryMainMenuTranslate.TryGetValue(localizationString.Key, out string text))
            {
                localizationString.SetText(text);
            }
        }
    }
}