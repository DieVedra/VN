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
    private MyLanguageName _currentMyLanguageName;
    private ReactiveProperty<int> _currentLanguageKeyIndex;
    private ReactiveCommand _languageChanged;
    private Dictionary<int, Dictionary<string, string>> _dictionariesMainMenuTranslates;
    private Dictionary<int, Dictionary<string, string>> _dictionaryStoryTranslates;

    private IReadOnlyList<LocalizationString> _localizableContentMainMenuUI;
    private IReadOnlyList<LocalizationString> _localizableContentStories;

    public MyLanguageName CurrentLanguageName => _mainMenuLocalizationInfoHolder.LanguageNames[_currentLanguageKeyIndex.Value];
    public string GetName => _mainMenuLocalizationInfoHolder.LanguageNames[_currentLanguageKeyIndex.Value].Name;
    public string GetKey => _mainMenuLocalizationInfoHolder.LanguageNames[_currentLanguageKeyIndex.Value].Key;
    public int GetMyLanguageNamesCount => _mainMenuLocalizationInfoHolder.LanguageNames.Count;
    public IReactiveProperty<int> CurrentLanguageKeyIndex => _currentLanguageKeyIndex;
    
    public MainMenuLocalizationHandler()
    {
        _loader = new LocalizationFileProvider();
    }

    public async UniTask Init(SaveData saveData, ReactiveCommand languageChanged,
        IReadOnlyList<LocalizationString> localizableContentMainMenuUI, IReadOnlyList<LocalizationString> localizableContentStories)
    {
        Debug.Log($"MainMenuLocalizationHandler  Init");
        _currentLanguageKeyIndex?.Dispose();
        _currentLanguageKeyIndex = new ReactiveProperty<int>();
        _languageChanged?.Dispose();
        _languageChanged = languageChanged;
        _localizableContentMainMenuUI = localizableContentMainMenuUI;
        _localizableContentStories = localizableContentStories;
        
        _saveData = saveData;
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
        _dictionariesMainMenuTranslates = new Dictionary<int, Dictionary<string, string>>
        {
            [_currentLanguageKeyIndex.Value] =
                await LoadLanguageAsset(_currentMyLanguageName.GetMainMenuLocalizationAssetName)
        };

        _dictionaryStoryTranslates = new Dictionary<int, Dictionary<string, string>>
        {
            [_currentLanguageKeyIndex.Value] =
                await LoadLanguageAsset(_currentMyLanguageName.GetStoryLocalizationAssetName)
        };

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
        _languageChanged.Execute();
    }
    private async UniTask<Dictionary<string, string>> LoadLanguageAsset(string localizationAssetName)
    {
        return await _loader.LoadLocalizationFile(localizationAssetName);
    }
    private void SetLanguageMainMenuAndStory()
    {
        if (_saveData != null)
        {
            _saveData.LanguageLocalizationKey = _mainMenuLocalizationInfoHolder.LanguageNames[_currentLanguageKeyIndex.Value].Key;
        }

        if (_dictionariesMainMenuTranslates.TryGetValue(_currentLanguageKeyIndex.Value, out var dictionaryMainMenuTranslate))
        {
            foreach (var localizationString in _localizableContentMainMenuUI)
            {
                if (dictionaryMainMenuTranslate.TryGetValue(localizationString.Key, out string text))
                {
                    localizationString.SetText(text);
                    Debug.Log($" dictionaryMainMenuTranslate test2  {localizationString.Key}  {localizationString.DefaultText}");
                }
            }
        }
        else
        {
            Debug.LogError($"DictionariesMainMenuTranslates.TryGetValue: False   key: {_currentLanguageKeyIndex.Value}");
        }

        if (_dictionaryStoryTranslates.TryGetValue(_currentLanguageKeyIndex.Value, out var dictionaryStoryTranslates))
        {
            foreach (var localizationString in _localizableContentStories)
            {
                if (dictionaryStoryTranslates.TryGetValue(localizationString.Key, out string text))
                {
                    localizationString.SetText(text);
                    Debug.Log($"dictionaryStoryTranslates test3  {localizationString.Key}  {localizationString.DefaultText}");
                }
            }
        }
        else
        {
            Debug.LogError($"DictionaryStoryTranslates.TryGetValue: False   key: {_currentLanguageKeyIndex.Value}");
        }

        // var dictionaryMainMenuTranslate = _dictionariesMainMenuTranslates[_currentLanguageKeyIndex.Value];

        // var dictionaryStoryTranslates = _dictionaryStoryTranslates[_currentLanguageKeyIndex.Value];


        // foreach (var story in _storiesProvider.Stories)
        // {
        //     if (dictionaryStoryTranslates.TryGetValue(story.Description.Key, out string text))
        //     {
        //         story.Description.SetText(text);
        //     }
        // }

        // if (dictionaryMainMenuTranslate == null)
        // {
        //     Debug.Log($"44 dictionaryMainMenuTranslate == null");
        //
        // }
        //
        //
        // foreach (var VARIABLE in dictionaryMainMenuTranslate)
        // {
        //     Debug.Log($"test1  {VARIABLE.Value}  {VARIABLE.Key}");
        //
        // }
        
        
        // foreach (var localizationString in _localizableContent)
        // {
        //     Debug.Log($"test2  {localizationString.Key}  {localizationString.DefaultText}");
        //
        //     if (dictionaryMainMenuTranslate.TryGetValue(localizationString.Key, out string text))
        //     {
        //         localizationString.SetText(text);
        //     }
        // }
    }
}