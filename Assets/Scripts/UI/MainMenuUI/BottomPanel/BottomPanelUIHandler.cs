
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BottomPanelUIHandler
{
    private const int _sublingIndex = 4;
    private readonly ConfirmedPanelUIHandler _confirmedPanelUIHandler;
    private readonly Transform _parent;
    private readonly ExitButtonUIHandler _exitButtonUIHandler;
    private readonly AdvertisingButtonUIHandler _advertisingButtonUIHandler;
    
    private BottomPanelView _bottomPanelView;
    
    public BottomPanelUIHandler(ConfirmedPanelUIHandler confirmedPanelUIHandler, AdvertisingButtonUIHandler advertisingButtonUIHandler, Transform parent)
    {
        _confirmedPanelUIHandler = confirmedPanelUIHandler;
        _exitButtonUIHandler = new ExitButtonUIHandler();
        _advertisingButtonUIHandler = advertisingButtonUIHandler;
        _parent = parent;
    }

    public void Dispose()
    {
        _advertisingButtonUIHandler.Dispose();
    }
    public void Init(BottomPanelView bottomPanelView)
    {
        _bottomPanelView = bottomPanelView;
    }

    public void SubscribeButtons()
    {
        _bottomPanelView.GameExitButton.onClick.AddListener(() =>
        {
            _confirmedPanelUIHandler.Show(
                _exitButtonUIHandler.LabelTextToConfirmedPanel, _exitButtonUIHandler.TranscriptionTextToConfirmedPanel,
                _exitButtonUIHandler.ButtonText, _exitButtonUIHandler.HeightPanel,
                _exitButtonUIHandler.FontSizeValue,
                _exitButtonUIHandler.Press).Forget();
        });
        _bottomPanelView.ShowAdvertisingButton.onClick.AddListener(() =>
        {
            
            _confirmedPanelUIHandler.Show(
                _advertisingButtonUIHandler.LabelTextToConfirmedPanel, _advertisingButtonUIHandler.TranscriptionTextToConfirmedPanel,
                _advertisingButtonUIHandler.ButtonText, AdvertisingButtonUIHandler.HeightPanel,
                AdvertisingButtonUIHandler.FontSizeValue,
                () => { _advertisingButtonUIHandler.Press().Forget();},
                true).Forget();
        });
        _bottomPanelView.transform.SetSiblingIndex(_sublingIndex);
        _bottomPanelView.gameObject.SetActive(true);
    }
}