using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class PanelsLocalizationHandler : ILocalizationChanger
{
    private const string _defaultLanguageKey = "en";
    // private const string DefaultLanguageKey = "ru";
    private readonly LocalizationFileProvider _loader;
    private LocalizationInfoHolder _localizationInfoHolder;
    private SaveData _saveData;
    private MyLanguageName _currentMyLanguageName;
    private ReactiveProperty<int> _currentLanguageKeyIndex;
    private ReactiveCommand _languageChanged;
    private CompositeDisposable _compositeDisposable;
    private Dictionary<int, Dictionary<string, string>> _dictionariesPanelsTranslates;
    private Dictionary<int, Dictionary<string, string>> _dictionaryMenuStoryTranslates;

    private List<LocalizationString> _localizableContent;
    private bool _inMainMenu;
    public MyLanguageName CurrentLanguageName => _localizationInfoHolder.LanguageNames[_currentLanguageKeyIndex.Value];
    public string GetName => _localizationInfoHolder.LanguageNames[_currentLanguageKeyIndex.Value].Name;
    public string GetKey => _localizationInfoHolder.LanguageNames[_currentLanguageKeyIndex.Value].Key;
    public int GetMyLanguageNamesCount => _localizationInfoHolder.LanguageNames.Count;
    public IReactiveProperty<int> CurrentLanguageKeyIndex => _currentLanguageKeyIndex;
    public ReactiveCommand LanguageChanged => _languageChanged;
    public PanelsLocalizationHandler()
    {
        _loader = new LocalizationFileProvider();
        _languageChanged = new ReactiveCommand();
        _currentLanguageKeyIndex = new ReactiveProperty<int>();
        _inMainMenu = true;
    }

    public async UniTask Init(SaveData saveData)
    {
        _saveData = saveData;
        _localizationInfoHolder = await new LocalizationHandlerAssetProvider().LoadLocalizationHandlerAsset();
        TryDefineLanguageKey();
        await LoadCurrentLanguage();
        SetLanguagePanelsAndMenuStory();
    }

    public void SubscribeChangeLanguage()
    {
        _compositeDisposable = new CompositeDisposable();
        _currentLanguageKeyIndex.Skip(1).Subscribe(_ =>
        {
            ChangeLanguage();
        }).AddTo(_compositeDisposable);
    }
    public void UnsubscribeChangeLanguage()
    {
        _compositeDisposable?.Clear();
    }
    public void SetPanelsLocalizableContentFromMainMenu(IReadOnlyList<LocalizationString> localizableContent)
    {
        _localizableContent = localizableContent.ToList();
        _inMainMenu = true;
    }
    public void AddLocalizableContentFromLevel(IReadOnlyList<LocalizationString> localizableContent)
    {
        _localizableContent.AddRange(localizableContent);
        _inMainMenu = false;
    }

    public async UniTask LoadAllLanguagesForPanels()
    {
        Debug.Log($"LoadAllLanguagesForPanels");

        for (int i = 0; i < _localizationInfoHolder.LanguageNames.Count; i++)
        {
            Debug.Log($"_localizationInfoHolder.LanguageNames[{i}]  {_localizationInfoHolder.LanguageNames[i].GetPanelsLocalizationAssetName}");
            if (i == _currentLanguageKeyIndex.Value)
            {
                continue;
            }
            _dictionariesPanelsTranslates[i] = await LoadLanguageAsset(_localizationInfoHolder.LanguageNames[i].GetPanelsLocalizationAssetName);
            _dictionaryMenuStoryTranslates[i] = await LoadLanguageAsset(_localizationInfoHolder.LanguageNames[i].GetMenuStoryLocalizationAssetName);
        }
    }

    public void SetLanguagePanelsAndMenuStory()
    {
        Debug.Log($"SetLanguagePanelsAndMenuStory    {_dictionariesPanelsTranslates.Count}   {_currentLanguageKeyIndex.Value}");

        if (_saveData != null)
        {
            _saveData.LanguageLocalizationKey = _localizationInfoHolder.LanguageNames[_currentLanguageKeyIndex.Value].Key;
        }

        Dictionary<string, string> dictionaryTranslate = new Dictionary<string, string>();
        if (_dictionariesPanelsTranslates.TryGetValue(_currentLanguageKeyIndex.Value, out var dictionaryMainMenuTranslate))
        {
            dictionaryTranslate.AddRange(dictionaryMainMenuTranslate);
        }
        else
        {
            Debug.LogError($"DictionariesMainMenuTranslates.TryGetValue: False   key: {_currentLanguageKeyIndex.Value}");
        }
        Debug.Log($"_inMainMenu {_inMainMenu}");

        if (_inMainMenu == true)
        {
            if (_dictionaryMenuStoryTranslates.TryGetValue(_currentLanguageKeyIndex.Value, out var dictionaryStoryTranslates))
            {
                dictionaryTranslate.AddRange(dictionaryStoryTranslates);
            }
            else
            {
                Debug.LogError($"DictionaryStoryTranslates.TryGetValue: False   key: {_currentLanguageKeyIndex.Value}");
            }
        }

        foreach (var localizationString in _localizableContent)
        {
            if (dictionaryTranslate.TryGetValue(localizationString.Key, out string text))
            {
                localizationString.SetText(text);
            }
        }
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
        for (int i = 0; i < _localizationInfoHolder.LanguageNames.Count; i++)
        {
            _currentLanguageKeyIndex.Value = i;
            Debug.Log($"TryDefineLanguageKey()  {_currentLanguageKeyIndex.Value}");
            if (_localizationInfoHolder.LanguageNames[i].Key == systemLanguageKey)
            {
                _currentMyLanguageName = _localizationInfoHolder.LanguageNames[i];
                break;
            }
        }
    }

    private async UniTask LoadCurrentLanguage()
    {
        _dictionariesPanelsTranslates = new Dictionary<int, Dictionary<string, string>>
        {
            [_currentLanguageKeyIndex.Value] =
                await LoadLanguageAsset(_currentMyLanguageName.GetPanelsLocalizationAssetName)
        };

        if (_inMainMenu == true)
        {
            _dictionaryMenuStoryTranslates = new Dictionary<int, Dictionary<string, string>>
            {
                [_currentLanguageKeyIndex.Value] =
                    await LoadLanguageAsset(_currentMyLanguageName.GetMenuStoryLocalizationAssetName)
            };
        }
    }

    private void ChangeLanguage()
    {
        Debug.Log($"ChangeLanguage");
        SetLanguagePanelsAndMenuStory();
        _languageChanged.Execute();
    }

    private async UniTask<Dictionary<string, string>> LoadLanguageAsset(string localizationAssetName)
    {
        return await _loader.LoadLocalizationFile(localizationAssetName);
    }
}