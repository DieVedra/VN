using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonTransitionToMainSceneUIHandler
{
    private const int _mainSceneIndex = 0;
    public const int FontSizeValue = 80;
    public const int HeightPanel = 700;

    public readonly LocalizationString LabelText = "Точно?";
    public readonly LocalizationString TranscriptionText = "Вернуться в главное меню?";
    public readonly LocalizationString ButtonText = "Да";

    private readonly LoadScreenUIHandler _loadScreenUIHandler;
    private readonly OnSceneTransitionEvent _onSceneTransition;
    private readonly SmoothAudio _smoothAudio;

    public ButtonTransitionToMainSceneUIHandler(LoadScreenUIHandler loadScreenUIHandler, OnSceneTransitionEvent onSceneTransition, SmoothAudio smoothAudio)
    {
        _loadScreenUIHandler = loadScreenUIHandler;
        _onSceneTransition = onSceneTransition;
        _smoothAudio = smoothAudio;
    }

    public async UniTask Press()
    {
        var token = new CancellationTokenSource();
        await UniTask.WhenAll(_loadScreenUIHandler.BlackFrameUIHandler.Close(),
            _smoothAudio.SmoothStopAudio(token.Token, AudioSourceType.Music),
            _smoothAudio.SmoothStopAudio(token.Token, AudioSourceType.Ambient));
        Camera.main.gameObject.SetActive(false);
        await _loadScreenUIHandler.ShowToMainMenuMove();
        await SceneManager.LoadSceneAsync(_mainSceneIndex, LoadSceneMode.Single);
        _onSceneTransition.Execute();
    }
}