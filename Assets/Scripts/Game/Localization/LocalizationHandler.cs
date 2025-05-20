
using System.Collections.Generic;
using System.Globalization;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class LocalizationHandler : ILocalizationChanger
{
    private const string _defaultLanguageKey = "ru";
    private MainMenuLocalizationInfoHolder _mainMenuLocalizationInfoHolder;
    private SaveData _saveData;
    private string _currentLanguageKey;
    private ReactiveProperty<int> _currentLanguageKeyIndex;

    public MyLanguageName CurrentLanguageName => _mainMenuLocalizationInfoHolder.LanguageNames[_currentLanguageKeyIndex.Value];
    // public IReadOnlyList<MyLanguageName> MyLanguageNames => _mainMenuLocalizationInfoHolder.LanguageNames;
    public string GetName => _mainMenuLocalizationInfoHolder.LanguageNames[_currentLanguageKeyIndex.Value].Name;
    public int GetMyLanguageNamesCount => _mainMenuLocalizationInfoHolder.LanguageNames.Count;
    public IReactiveProperty<int> CurrentLanguageKeyIndex => _currentLanguageKeyIndex;
    
    public LocalizationHandler()
    {
        _currentLanguageKeyIndex = new ReactiveProperty<int>();
    }

    public async UniTask Init(SaveData saveData)
    {
        _saveData = saveData;
        _mainMenuLocalizationInfoHolder = await new LocalizationHandlerAssetProvider().LoadLocalizationHandlerAsset();

        TryDefineLanguageKey();

        _currentLanguageKeyIndex.Subscribe(ChangeLanguage);
    }

    private void TryDefineLanguageKey()
    {
        string systemLanguageKey = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        if (_saveData != null && string.IsNullOrEmpty(_saveData.LanguageLocalizationKey) == false )
        {
            systemLanguageKey = _saveData.LanguageLocalizationKey;
        }
        for (int i = 0; i < _mainMenuLocalizationInfoHolder.LanguageNames.Count; i++)
        {
            _currentLanguageKeyIndex.Value = i;
            if (_mainMenuLocalizationInfoHolder.LanguageNames[i].Key == systemLanguageKey)
            {
                _currentLanguageKey = _mainMenuLocalizationInfoHolder.LanguageNames[i].Key;
                break;
            }
            else
            {
                _currentLanguageKey = _defaultLanguageKey;
            }
        }
    }

    private void ChangeLanguage(int index)
    {
        //пройдет по всем строкам локализации и заменит текст
        // Debug.Log($"CurrentLanguageName: {CurrentLanguageName.Name}");
    }
}