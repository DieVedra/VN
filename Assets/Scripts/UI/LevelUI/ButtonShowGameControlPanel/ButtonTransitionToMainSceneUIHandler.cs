
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class ButtonTransitionToMainSceneUIHandler
{
    private const int _mainSceneIndex = 0;
    public const int FontSizeValue = 80;
    public const float HeightPanel = 700;

    public readonly LocalizationString LabelText = "Точно?";
    public readonly LocalizationString TranscriptionText = "Вернуться в главное меню?";
    public readonly LocalizationString ButtonText = "Да";

    private readonly ReactiveCommand _onSceneTransition;
    private readonly BlackFrameUIHandler _blackFrameUIHandler;
    private readonly MainSceneLoader _mainSceneLoader;
    private bool _firstPartLoadComplete;

    public ButtonTransitionToMainSceneUIHandler(ReactiveCommand onSceneTransition, BlackFrameUIHandler blackFrameUIHandler, SaveServiceProvider saveServiceProvider)
    {
        _onSceneTransition = onSceneTransition;
        _mainSceneLoader = new MainSceneLoader(saveServiceProvider);
        _blackFrameUIHandler = blackFrameUIHandler;
    }

    public async UniTask Press()
    {
        await _blackFrameUIHandler.Close();
        _blackFrameUIHandler.SetAsLastSibling();
        _mainSceneLoader.StartLoad(_mainSceneIndex).Forget();
        _mainSceneLoader.OnCompleteLoad += x => { StartLoadPart2(x).Forget();};
        _firstPartLoadComplete = true;
    }
    
    private async UniTask StartLoadPart2(AsyncOperation operation)
    {
        await UniTask.WaitUntil(() => _firstPartLoadComplete == true);
        _onSceneTransition.Execute();
        _mainSceneLoader.Activate(operation);
    }
}