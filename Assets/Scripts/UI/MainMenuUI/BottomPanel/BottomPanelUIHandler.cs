
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BottomPanelUIHandler
{
    private readonly int _sublingIndex = 4;
    private readonly BottomPanelAssetProvider _bottomPanelAssetProvider;
    private readonly ConfirmedPanelUIHandler _confirmedPanelUIHandler;
    private readonly Transform _parent;
    private readonly ExitButtonUIHandler _exitButtonUIHandler;
    private readonly AdvertisingButtonUIHandler _advertisingButtonUIHandler;
    
    private BottomPanelView _bottomPanelView;
    
    public BottomPanelUIHandler(ConfirmedPanelUIHandler confirmedPanelUIHandler, AdvertisingButtonUIHandler advertisingButtonUIHandler, Transform parent)
    {
        _bottomPanelAssetProvider = new BottomPanelAssetProvider();
        _confirmedPanelUIHandler = confirmedPanelUIHandler;
        _exitButtonUIHandler = new ExitButtonUIHandler();
        _advertisingButtonUIHandler = advertisingButtonUIHandler;
        _parent = parent;
    }

    public async UniTask Init()
    {
        _bottomPanelView = await _bottomPanelAssetProvider.LoadBottomPanelAsset(_parent);
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
                _advertisingButtonUIHandler.ButtonText, _advertisingButtonUIHandler.HeightPanel,
                _advertisingButtonUIHandler.FontSizeValue,
                () => { _advertisingButtonUIHandler.Press().Forget();},
                true).Forget();
        });
        _bottomPanelView.transform.SetSiblingIndex(_sublingIndex);
        _bottomPanelView.gameObject.SetActive(true);
    }
}