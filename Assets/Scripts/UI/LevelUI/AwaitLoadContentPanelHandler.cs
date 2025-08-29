using Cysharp.Threading.Tasks;

public class AwaitLoadContentPanelHandler
{
    private const float _delay = 1f;
    private readonly LocalizationString _awaitLoadText = "Пожалуйста, дождитесь окончания загрузки контента...";
    private readonly BlackFrameUIHandler _blackFrameUIHandler;
    private readonly LoadScreenUIHandler _loadScreenUIHandler;
    private readonly LoadAssetsPercentHandler _loadAssetsPercentHandler;

    public AwaitLoadContentPanelHandler(BlackFrameUIHandler blackFrameUIHandler, LoadScreenUIHandler loadScreenUIHandler,
        LoadAssetsPercentHandler loadAssetsPercentHandler, OnAwaitLoadContentEvent<AwaitLoadContentPanel> onAwaitLoadContentEvent)
    {
        _blackFrameUIHandler = blackFrameUIHandler;
        _loadScreenUIHandler = loadScreenUIHandler;
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

        _loadScreenUIHandler.ShowToAwaitLoadContent(_loadAssetsPercentHandler, _awaitLoadText.DefaultText).Forget();
    }

    private void Hide()
    {
        _loadScreenUIHandler.HideToAwaitLoadContent(_loadAssetsPercentHandler);
        if (_blackFrameUIHandler.IsOpen == true)
        {
            _blackFrameUIHandler.OpenTranslucent(_delay).Forget();
        }
    }
}