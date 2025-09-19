
public class PhoneUIHandler
{
    private readonly TopPanelHandler _topPanelHandler;
    // private int _subling =
    public PhoneUIHandler(PhoneUIView phoneUIView)
    {
        _topPanelHandler = new TopPanelHandler(phoneUIView.SignalIndicatorImage, phoneUIView.TimeText, phoneUIView.ButteryText, phoneUIView.ButteryImage, phoneUIView.ButteryIndicatorImage);
        
    }

    public void Init()
    {
        
    }
    public void Dispose()
    {
        
    }
    
}