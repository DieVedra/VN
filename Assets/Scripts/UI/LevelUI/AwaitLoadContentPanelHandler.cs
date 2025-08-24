using Cysharp.Threading.Tasks;

public class AwaitLoadContentPanelHandler
{
    private const float _delay = 1f;
    private readonly BlackFrameUIHandler _blackFrameUIHandler;
    private readonly LoadIndicatorUIHandler _loadIndicatorUIHandler;
    private readonly LoadAssetsPercentHandler _loadAssetsPercentHandler;

    public AwaitLoadContentPanelHandler(BlackFrameUIHandler blackFrameUIHandler, LoadIndicatorUIHandler loadIndicatorUIHandler,
        LoadAssetsPercentHandler loadAssetsPercentHandler, OnAwaitLoadContentEvent<AwaitLoadContentPanel> onAwaitLoadContentEvent)
    {
        _blackFrameUIHandler = blackFrameUIHandler;
        _loadIndicatorUIHandler = loadIndicatorUIHandler;
        _loadAssetsPercentHandler = loadAssetsPercentHandler;
        onAwaitLoadContentEvent.Subscribe(_ =>
        {
            if (_ == AwaitLoadContentPanel.Show)
            {
                Show();
            }
            else
            {
                Hide();
            }
        });
    }

    private void Show()
    {
        if (_blackFrameUIHandler.IsOpen == true)
        {
            _blackFrameUIHandler.CloseTranslucent().Forget();
        }
        _loadAssetsPercentHandler.StartCalculatePercent();
        _loadIndicatorUIHandler.SetPercentIndicateMode(_loadAssetsPercentHandler.CurrentLoadPercentReactiveProperty);
        _loadIndicatorUIHandler.StartIndicate();
    }

    private void Hide()
    {
        _loadAssetsPercentHandler.StopCalculatePercent();
        _loadIndicatorUIHandler.StopIndicate();
        if (_blackFrameUIHandler.IsOpen == true)
        {
            _blackFrameUIHandler.OpenTranslucent(_delay).Forget();
        }
    }
}