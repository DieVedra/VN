
using System.Collections.Generic;
using System.Globalization;
using Cysharp.Threading.Tasks;
using UniRx;

public class MainMenuLocalizationHandler : ILocalizationChanger
{
    private const string _defaultLanguageKey = "en";
    // private const string _defaultLanguageKey = "ru";
    private MainMenuLocalizationInfoHolder _mainMenuLocalizationInfoHolder;
    private SaveData _saveData;
    private MyLanguageName _currentMyLanguageName;
    private ReactiveProperty<int> _currentLanguageKeyIndex;
    private List<Dictionary<string, string>> _listTranslates;
    private List<LocalizationString> _localizationStrings;
    public ReactiveCommand LanguageChanged { get; private set; }

    public MyLanguageName CurrentLanguageName => _mainMenuLocalizationInfoHolder.LanguageNames[_currentLanguageKeyIndex.Value];
    public string GetName => _mainMenuLocalizationInfoHolder.LanguageNames[_currentLanguageKeyIndex.Value].Name;
    public int GetMyLanguageNamesCount => _mainMenuLocalizationInfoHolder.LanguageNames.Count;
    public IReactiveProperty<int> CurrentLanguageKeyIndex => _currentLanguageKeyIndex;
    
    public MainMenuLocalizationHandler()
    {
        _currentLanguageKeyIndex = new ReactiveProperty<int>();
        LanguageChanged = new ReactiveCommand();
    }

    public async UniTask Init(SaveData saveData)
    {
        _saveData = saveData;
        _mainMenuLocalizationInfoHolder = await new LocalizationHandlerAssetProvider().LoadLocalizationHandlerAsset();
        TryDefineLanguageKey();
        await LoadCurrentLanguage();
        SetLanguage();

        _currentLanguageKeyIndex.Skip(1).Subscribe(_ =>
        {
            ChangeLanguage();
        });
    }

    private void TryDefineLanguageKey()
    {
        // string systemLanguageKey = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        string systemLanguageKey;
        
        
        if (_saveData != null && string.IsNullOrEmpty(_saveData.LanguageLocalizationKey) == false )
        {
            systemLanguageKey = _saveData.LanguageLocalizationKey;
        }
        else
        {
            systemLanguageKey = _defaultLanguageKey;
        }

        for (int i = 0; i < _mainMenuLocalizationInfoHolder.LanguageNames.Count; i++)
        {
            _currentLanguageKeyIndex.Value = i;
            if (_mainMenuLocalizationInfoHolder.LanguageNames[i].Key == _defaultLanguageKey)
            {
                _currentMyLanguageName = _mainMenuLocalizationInfoHolder.LanguageNames[i];
            }
            
            if (_mainMenuLocalizationInfoHolder.LanguageNames[i].Key == systemLanguageKey)
            {
                _currentMyLanguageName = _mainMenuLocalizationInfoHolder.LanguageNames[i];
                break;
            }
        }
    }

    private async UniTask LoadCurrentLanguage()
    {
        _listTranslates = new List<Dictionary<string, string>>(_mainMenuLocalizationInfoHolder.LanguageNames.Count);
        await LoadLanguage(new MainMenuLocalizationFileProvider(), _currentLanguageKeyIndex.Value, _currentMyLanguageName.MainMenuLocalizationAssetName);
    }

    public async UniTask LoadAllLanguages()
    {
        for (int i = 0; i < _mainMenuLocalizationInfoHolder.LanguageNames.Count; i++)
        {
            if (i == _currentLanguageKeyIndex.Value)
            {
                continue;
            }
            await LoadLanguage(new MainMenuLocalizationFileProvider(), i, _mainMenuLocalizationInfoHolder.LanguageNames[i].MainMenuLocalizationAssetName);
        }
    }

    private async UniTask<Dictionary<string, string>> LoadLanguage(MainMenuLocalizationFileProvider provider, int index, string localizationAssetName)
    {
        var result = await provider.LoadMainMenuLocalizationFile(localizationAssetName);
        _listTranslates.Insert(index, result);
        return result;
    }
    private void ChangeLanguage()
    {
        SetLanguage();
        LanguageChanged.Execute();
    }

    private void SetLanguage()
    {
        var dic = _listTranslates[_currentLanguageKeyIndex.Value];
        if (_saveData != null)
        {
            _saveData.LanguageLocalizationKey = _mainMenuLocalizationInfoHolder.LanguageNames[_currentLanguageKeyIndex.Value].Key;
        }
        
        foreach (var localizationString in LocalizationString.LocalizationStrings)
        {
            if (dic.TryGetValue(localizationString.Key, out string text))
            {
                localizationString.SetText(text);
            }
        }
    }
}