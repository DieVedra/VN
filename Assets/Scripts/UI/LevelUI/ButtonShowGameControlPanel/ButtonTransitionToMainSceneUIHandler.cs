
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class ButtonTransitionToMainSceneUIHandler
{
    public readonly string LabelText = "Точно?";
    public readonly string TranscriptionText = "вернуться в главное меню?";
    public readonly string ButtonText = "Да";
    public readonly int FontSizeValue = 80;
    public readonly float HeightPanel = 700;
    private readonly ReactiveCommand _onSceneTransition;
    private readonly BlackFrameUIHandler _blackFrameUIHandler;
    private readonly SceneLoader _sceneLoader;
    private bool _firstPartLoadComplete;

    public ButtonTransitionToMainSceneUIHandler(ReactiveCommand onSceneTransition, BlackFrameUIHandler blackFrameUIHandler, SaveServiceProvider saveServiceProvider)
    {
        _onSceneTransition = onSceneTransition;
        _sceneLoader = new SceneLoader(saveServiceProvider);
        _blackFrameUIHandler = blackFrameUIHandler;
    }

    public async UniTask Press()
    {
        await _blackFrameUIHandler.Close();
        _blackFrameUIHandler.SetAsLastSibling();
        _sceneLoader.StartLoad(0).Forget();
        _sceneLoader.OnCompleteLoad += x => { StartLoadPart2(x).Forget();};
        _firstPartLoadComplete = true;
    }
    
    private async UniTask StartLoadPart2(AsyncOperation operation)
    {
        await UniTask.WaitUntil(() => _firstPartLoadComplete == true);
        _onSceneTransition.Execute();
        _sceneLoader.Activate(operation);
    }
}