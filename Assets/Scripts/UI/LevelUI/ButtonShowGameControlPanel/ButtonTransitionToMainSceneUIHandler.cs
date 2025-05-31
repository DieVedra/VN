
using Cysharp.Threading.Tasks;
using UniRx;
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
    private readonly ReactiveCommand _onSceneTransition;

    public ButtonTransitionToMainSceneUIHandler(LoadScreenUIHandler loadScreenUIHandler, ReactiveCommand onSceneTransition)
    {
        _loadScreenUIHandler = loadScreenUIHandler;
        _onSceneTransition = onSceneTransition;
    }

    public async UniTask Press()
    {
        await _loadScreenUIHandler.BlackFrameUIHandler.Close();
        Camera.main.gameObject.SetActive(false);
        await _loadScreenUIHandler.ShowToMainMenuMove();
        await SceneManager.LoadSceneAsync(_mainSceneIndex, LoadSceneMode.Single);
        _onSceneTransition.Execute();
    }
}