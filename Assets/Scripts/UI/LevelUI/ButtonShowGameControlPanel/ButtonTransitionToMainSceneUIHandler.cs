using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonTransitionToMainSceneUIHandler : ILocalizable
{
    private const int _mainSceneIndex = 0;
    public const int FontSizeValue = 80;
    public const int HeightPanel = 700;

    public readonly LocalizationString LabelText = "Точно?";
    public readonly LocalizationString TranscriptionText = "Вернуться в главное меню?";
    public readonly LocalizationString ButtonText = "Да";

    private readonly LoadScreenUIHandler _loadScreenUIHandler;
    private readonly Func<UniTask> _preSceneTransitionOperation;

    public ButtonTransitionToMainSceneUIHandler(LoadScreenUIHandler loadScreenUIHandler, Func<UniTask> preSceneTransitionOperation)
    {
        _loadScreenUIHandler = loadScreenUIHandler;
        _preSceneTransitionOperation = preSceneTransitionOperation;
    }

    public async UniTask Press()
    {
        await UniTask.WhenAll(_loadScreenUIHandler.BlackFrameUIHandler.Close(), _preSceneTransitionOperation.Invoke());
        await _loadScreenUIHandler.ShowToMainMenuMove();
        Camera.main.gameObject.SetActive(false);
        await SceneManager.LoadSceneAsync(_mainSceneIndex, LoadSceneMode.Single);
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        List<LocalizationString> localizationStrings = new List<LocalizationString>();
        localizationStrings.AddRange(_loadScreenUIHandler.GetLocalizableContent());
        localizationStrings.AddRange(new[] {LabelText, TranscriptionText, ButtonText});
        return localizationStrings;
    }
}