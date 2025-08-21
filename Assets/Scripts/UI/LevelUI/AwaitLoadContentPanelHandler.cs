using Cysharp.Threading.Tasks;

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
        _loadIndicatorUIHandler.StopIndicate();
        _loadAssetsPercentHandler.StopCalculatePercent();
        _blackFrameUIHandler.OpenTranslucent().Forget();
    }
}