using Cysharp.Threading.Tasks;
using UnityEngine;

public class AwaitLoadContentPanelHandler
{
    private readonly BlackFrameUIHandler _blackFrameUIHandler;
    private readonly LoadIndicatorUIHandler _loadIndicatorUIHandler;
    private readonly LoadAssetsPercentHandler _loadAssetsPercentHandler;

    public AwaitLoadContentPanelHandler(BlackFrameUIHandler blackFrameUIHandler, LoadIndicatorUIHandler loadIndicatorUIHandler, LoadAssetsPercentHandler loadAssetsPercentHandler)
    {
        _blackFrameUIHandler = blackFrameUIHandler;
        _loadIndicatorUIHandler = loadIndicatorUIHandler;
        _loadAssetsPercentHandler = loadAssetsPercentHandler;
    }

    public void Show()
    {
        _blackFrameUIHandler.CloseTranslucent().Forget();
        _loadAssetsPercentHandler.StartCalculatePercent();
        _loadIndicatorUIHandler.SetPercentIndicateMode(_loadAssetsPercentHandler.CurrentLoadPercentReactiveProperty);
        _loadIndicatorUIHandler.StartIndicate();
    }

    public void Hide()
    {
        _loadAssetsPercentHandler.StopCalculatePercentOnComplete();
        Debug.Log(6656565656565);

        _loadIndicatorUIHandler.StopIndicate();
        _blackFrameUIHandler.OpenTranslucent().Forget();
    }
}