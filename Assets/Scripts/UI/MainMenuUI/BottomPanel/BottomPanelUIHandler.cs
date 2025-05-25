
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class BottomPanelUIHandler
{
    private const int _sublingIndex = 4;
    private readonly ConfirmedPanelUIHandler _confirmedPanelUIHandler;
    private readonly Transform _parent;
    private readonly ReactiveCommand _languageChanged;
    private readonly ExitButtonUIHandler _exitButtonUIHandler;
    private readonly AdvertisingButtonUIHandler _advertisingButtonUIHandler;
    private BlackFrameUIHandler _darkeningBackgroundFrameUIHandler;
    private BottomPanelView _bottomPanelView;
    
    public BottomPanelUIHandler(ConfirmedPanelUIHandler confirmedPanelUIHandler, AdvertisingButtonUIHandler advertisingButtonUIHandler,
        Transform parent, ReactiveCommand languageChanged)
    {
        _confirmedPanelUIHandler = confirmedPanelUIHandler;
        _exitButtonUIHandler = new ExitButtonUIHandler();
        _advertisingButtonUIHandler = advertisingButtonUIHandler;
        _parent = parent;
        _languageChanged = languageChanged;
    }

    public void Dispose()
    {
        _advertisingButtonUIHandler.Dispose();
    }
    public void Init(BottomPanelView bottomPanelView, BlackFrameUIHandler darkeningBackgroundFrameUIHandler)
    {
        _bottomPanelView = bottomPanelView;
        _bottomPanelView.GameExitButtonText.text = _exitButtonUIHandler.ExitText;
        _bottomPanelView.ShowAdvertisingButtonText.text = _exitButtonUIHandler.ExitText;
        _darkeningBackgroundFrameUIHandler = darkeningBackgroundFrameUIHandler;
        SubscribeButtons();
    }

    private void SubscribeButtons()
    {
        _bottomPanelView.GameExitButton.onClick.AddListener(() =>
        {
            _confirmedPanelUIHandler.Show(
                _exitButtonUIHandler.LabelTextToConfirmedPanel, _exitButtonUIHandler.TranscriptionTextToConfirmedPanel,
                _exitButtonUIHandler.ButtonText, ExitButtonUIHandler.HeightPanel,
                ExitButtonUIHandler.FontSizeValue,
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